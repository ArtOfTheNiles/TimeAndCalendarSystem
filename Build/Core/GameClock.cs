using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FigmentForge.PHCC.Utilities;

namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// The Game Clock is how the game views time, entirely separate from real-time.
    /// </summary>
    [RequireComponent(typeof(GameCalendar))]
    [RequireComponent(typeof(SystemTimeRetriever))]
    public class GameClock : MonoBehaviour
    {

        #region Singleton:
        private static GameClock gameClock;
        public static GameClock Instance
        {
            get
            {
                if (!gameClock)
                {
                    gameClock = FindObjectOfType(typeof(GameClock)) as GameClock;
                    if (!gameClock)
                    {
                        Debug.LogError("There needs to be one active GameClock script on a GameObject in your scene.");
                    }
                }
                return gameClock;
            }
        }
        #endregion


        #region Debug Properties
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private GameCalendar gameCalendar;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private SystemTimeRetriever systemTimeRetriever;

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private List<GameClockListener> clockListeners = new List<GameClockListener>();

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private int _displayTickLength;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private int _displayTickDelta;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private int _gameDayTimeElapsed; //1-1440 value. 0 means it hasn't been initialized.

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private float _baseTick, _baseTickDelta;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private float _systemDebugMultiplier = 1f;

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private bool _hardPause;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private bool _softPause;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private float _gamePauseDelta;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private bool _inTimedSloMo;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debugs")]
        private float _sloMoDelta;
        #endregion

        #region Properties
        /// <summary>
        /// (Read Only) Get the current 1-1440 value of game time.
        /// </summary>
        public int GameDayTimeElapsed
        {
            get
            {
                if (_gameDayTimeElapsed == 0)
                {
                    _gameDayTimeElapsed = LoadSaveTime();
                }
                return _gameDayTimeElapsed;
            }
        }
        #endregion

        #region Game Designer Fields
        /////////////////////////////////DESIGNER FIELDS/////////////////////////////////////


        [SerializeField]
        [Tooltip("How many minutes will be in a game day?")]
        [RangeStep(48, 144, 12)]
        private int _dayLengthInMinutes = 48;

        [SerializeField]
        [Tooltip("DEBUG OVERRIDE; Default to 0 to ignore.\n    Not recommended for frame rates under 48fps.")]
        [RangeStep(0, 48, 6)]
        private int _dayLengthDebug = 0; //Currently no intention to set at runtime.

        [SerializeField]
        [Tooltip("How many game-minutes apart to update displays?")]
        private TSDef.C_PreferredTimeDelta _preferredDisplayTimeDelta = TSDef.C_PreferredTimeDelta.Fifteens;

        [FoldoutGroup("System Clocks")]
        [SerializeField]
        private bool _useSystemTime1 = true;

        [FoldoutGroup("System Clocks")]
        [Tooltip("How long, in real-time seconds, to wait to update System 1")]
        [SerializeField]
        private float _systemTime1 = 0.5f;
        private float _systemTime1Delta;

        [Space(5)]
        [FoldoutGroup("System Clocks")]
        [SerializeField]
        private bool _useSystemTime2 = true;

        [FoldoutGroup("System Clocks")]
        [Tooltip("How long, in real-time seconds, to wait to update System 2")]
        [SerializeField]
        private float _systemTime2 = 1f;
        private float _systemTime2Delta;

        [Space(5)]
        [FoldoutGroup("System Clocks")]
        [SerializeField]
        private bool _useSystemTime3 = true;

        [FoldoutGroup("System Clocks")]
        [Tooltip("How long, in real-time seconds, to wait to update System 3")]
        [SerializeField]
        private float _systemTime3 = 2f;
        private float _systemTime3Delta;

        [Space(5)]
        [FoldoutGroup("System Clocks")]
        [SerializeField]
        private bool _useSystemTime4 = true;

        [FoldoutGroup("System Clocks")]
        [Tooltip("How long, in real-time seconds, to wait to update System 4")]
        [SerializeField]
        private float _systemTime4 = 5f;
        private float _systemTime4Delta;

        [Space(5)]
        [FoldoutGroup("System Clocks")]
        [SerializeField]
        private bool _useSystemTime5 = true;

        [FoldoutGroup("System Clocks")]
        [Tooltip("How long, in real-time seconds, to wait to update System 5")]
        [SerializeField]
        private float _systemTime5 = 10f;
        private float _systemTime5Delta;

        #endregion

        #region System Events
        [FoldoutGroup("System Events")]
        public UnityEvent OnGamePaused = new UnityEvent();
        [FoldoutGroup("System Events")]
        public UnityEvent OnGameUnPaused = new UnityEvent();
        [FoldoutGroup("System Events")]
        public UnityEvent OnGameSlowed = new UnityEvent();
        [FoldoutGroup("System Events")]
        public UnityEvent OnGameUnSlowed = new UnityEvent();
        #endregion

        #region Real Time Functions
        ////////////////////////REAL-TIME//////////////////////////
        private void Awake()
        {
            InitializeClocks();

            AttachLocalComponents();

            Invoke("WakeClocks", 0.02f); //Need a slight delay to ensure that OnEnable has been called on listeners
        }



        void Update()
        {
            float realTimeElapsed = Time.deltaTime;

            if (!_softPause && !_hardPause)
            {
                _baseTickDelta -= realTimeElapsed;
                if (_baseTickDelta <= 0f)
                {
                    _gameDayTimeElapsed++;
                    _displayTickDelta--;
                    if (_displayTickDelta <= 0)
                    {
                        UpdateDisplayListeners(TSDef.C_SystemClockType.Display);
                        _displayTickDelta = _displayTickLength;
                    }
                    _baseTickDelta = _baseTick;
                }
                ProcessSystemClocks(realTimeElapsed);
            }
            else if (_softPause && !_hardPause)
            {
                ProcessGamePause(realTimeElapsed);
            }
            if (_inTimedSloMo)
            {
                _sloMoDelta -= realTimeElapsed;
                if (_sloMoDelta <= 0) { UnSloMo(); }
            }
        }

        private void ProcessSystemClocks(float realTimeElapsed)
        {
            //Go through each clock, if enabled, then update listeners and reset deltas

            realTimeElapsed *= _systemDebugMultiplier; //if in debug, use the appropriate multiplier

            if (_useSystemTime1)
            {
                _systemTime1Delta -= realTimeElapsed;
                if (_systemTime1Delta <= 0f)
                {
                    UpdateDisplayListeners(TSDef.C_SystemClockType.System_1);
                    _systemTime1Delta = _systemTime1;
                }
            }
            if (_useSystemTime2)
            {
                _systemTime2Delta -= realTimeElapsed;
                if (_systemTime2Delta <= 0f)
                {
                    UpdateDisplayListeners(TSDef.C_SystemClockType.System_2);
                    _systemTime2Delta = _systemTime2;
                }
            }
            if (_useSystemTime3)
            {
                _systemTime3Delta -= realTimeElapsed;
                if (_systemTime3Delta <= 0f)
                {
                    UpdateDisplayListeners(TSDef.C_SystemClockType.System_3);
                    _systemTime3Delta = _systemTime3;
                }
            }
            if (_useSystemTime4)
            {
                _systemTime4Delta -= realTimeElapsed;
                if (_systemTime4Delta <= 0f)
                {
                    UpdateDisplayListeners(TSDef.C_SystemClockType.System_4);
                    _systemTime4Delta = _systemTime4;
                }
            }
            if (_useSystemTime5)
            {
                _systemTime5Delta -= realTimeElapsed;
                if (_systemTime5Delta <= 0f)
                {
                    UpdateDisplayListeners(TSDef.C_SystemClockType.System_5);
                    _systemTime5Delta = _systemTime5;
                }
            }
        }

        private void ProcessGamePause(float elapsedTime)
        {
            _gamePauseDelta -= elapsedTime;
            if (_gamePauseDelta <= 0f)
            {
                _softPause = false;
                OnGameUnPaused.Invoke();
            }
        }
        #endregion

        #region Messaging System
        ////////////////////////MESSAGING SYSTEM//////////////////////////

        private void UpdateDisplayListeners(TSDef.C_SystemClockType clockType)
        {
            int elapsedTime = _gameDayTimeElapsed;
            foreach (GameClockListener gameClockListener in clockListeners)
            {
                //Debug.Log("A clock listener will be notified: " + gameClockListener);
                if (!gameClockListener) { return; }
                gameClockListener.ReceiveUpdateTick(elapsedTime, clockType);
            }
        }

        public void AddTickListener(GameClockListener listener)
        {
            GameClockListener newListener = new GameClockListener();
            newListener = listener;
            clockListeners.Add(newListener);
        }

        public void RemoveTickListener(GameClockListener listener)
        {
            if (clockListeners.Contains(listener))
            {
                clockListeners.Remove(listener);
            }
            else
            {
                Debug.LogError("Attempted to remove GameClockListener that does not exist.");
            }
        }
        #endregion

        #region Pause Calls
        ////////////////////////PAUSE FUNCTIONS//////////////////////////
        /// <summary>
        /// Pause the game for a certain period of time, like to pause systems during an animation or cutscene.
        /// </summary>
        /// <param name="pauseTime">The amount of time to pause in seconds.</param>
        public void PauseGameForSeconds(float pauseTime)
        {
            _softPause = true;
            _gamePauseDelta = pauseTime;
            OnGamePaused.Invoke();
        }
        /// <summary>
        /// Completely pause the game clock.
        /// </summary>
        public void PauseGame()
        {
            _hardPause = true;
            OnGamePaused.Invoke();
        }
        /// <summary>
        /// Un-pause the game clock.
        /// </summary>
        public void UnPauseGame()
        {
            _hardPause = false;
            OnGameUnPaused.Invoke();
        }
        /// <summary>
        /// Slow down time for a set period, like the length of an animation.
        /// </summary>
        /// <param name="timeToSlow">How long in seconds to slow down time. If comparing to an animation, use the real-time values as all of Unity's time parameters will scale accordingly.</param>
        /// <param name="timeScale">0-1 scalar offset of time. Default is half-speed. Values above 1 will speed up time.</param>
        public void SloMoTime(float timeToSlow, float timeScale = 0.5f)
        {
            _inTimedSloMo = true;
            _sloMoDelta = timeToSlow;
            SloMo(timeScale);
        }
        /// <summary>
        /// Slow down time
        /// </summary>
        /// <param name="timeScale">0-1 scalar offset of time. Default is half-speed. Values above 1 will speed up time.</param>
        public void SloMo(float timeScale = 0.5f)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale;
            OnGameSlowed.Invoke();
        }
        /// <summary>
        /// Undo Slo-Mo effects / reset timescale to normal.
        /// </summary>
        public void UnSloMo()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            OnGameUnSlowed.Invoke();
        }

        #endregion

        #region Data I/O
        ////////////////////////DATA MANAGEMENT//////////////////////////


        private int LoadSaveTime()
        {
            //TODO Save/load Data
            object savedData = null;
            if (savedData == null)
            {
                return 360; //new day starts at 6:00AM.
            }
            else
            {
                //Return savedData.savedTime or the like.
                return 100;//TODO Erase this for real code.
            }

        }

        public void SaveGameTime()
        {
            //TODO Save/load Data
        }
        #endregion

        #region Initialization Functions
        ////////////////////////INITIALIZATION//////////////////////////
        private void WakeClocks()
        {
            UpdateDisplayListeners(TSDef.C_SystemClockType.Display);
            UpdateDisplayListeners(TSDef.C_SystemClockType.System_1);
            UpdateDisplayListeners(TSDef.C_SystemClockType.System_2);
            UpdateDisplayListeners(TSDef.C_SystemClockType.System_3);
            UpdateDisplayListeners(TSDef.C_SystemClockType.System_4);
            UpdateDisplayListeners(TSDef.C_SystemClockType.System_5);
        }

        private int CalculateDisplayClockTickLength(TSDef.C_PreferredTimeDelta timeDelta)
        {
            int displayClockTickLength = 1;
            if (timeDelta == TSDef.C_PreferredTimeDelta.Fives)
            {
                displayClockTickLength *= 5;
            }
            if (timeDelta == TSDef.C_PreferredTimeDelta.Tens)
            {
                displayClockTickLength *= 10;
            }
            if (timeDelta == TSDef.C_PreferredTimeDelta.Fifteens)
            {
                displayClockTickLength *= 15;
            }
            if (timeDelta == TSDef.C_PreferredTimeDelta.Twenties)
            {
                displayClockTickLength *= 20;
            }
            if (timeDelta == TSDef.C_PreferredTimeDelta.Thirties)
            {
                displayClockTickLength *= 30;
            }
            return displayClockTickLength;
        }

        private float CalculateSystemDebugMultiplier()
        {
            float calcMultiplier;
            if (_dayLengthDebug != 0)
            {
                calcMultiplier = _dayLengthInMinutes / _dayLengthDebug;
                return calcMultiplier;
            }
            else
            {
                return 1f;
            }
        }

        private float CalculateGameTickLength()
        {
            float gameMinute = 0f;
            //Check for Debug values:
            if (_dayLengthDebug > 0)
            {
                gameMinute = ((_dayLengthDebug * 60) / 1440f);//24 hours in a day, 60 minutes in an hour, that's 1440 total units of game time.
            }
            else
            {
                gameMinute = ((_dayLengthInMinutes * 60) / 1440f);
            }
            return gameMinute;
        }

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.8f, 1f, 0.8f)]
        private void AttachLocalComponents()
        {
            if (gameCalendar == null)
            {
                gameCalendar = gameObject.GetComponent<GameCalendar>();
                if (gameCalendar == null)
                {
                    gameObject.AddComponent<GameCalendar>();
                    gameCalendar = gameObject.GetComponent<GameCalendar>();
                }
            }
            if (systemTimeRetriever == null)
            {
                systemTimeRetriever = gameObject.GetComponent<SystemTimeRetriever>();
                if (systemTimeRetriever == null)
                {
                    gameObject.AddComponent<SystemTimeRetriever>();
                    systemTimeRetriever = gameObject.GetComponent<SystemTimeRetriever>();
                }
            }
        }

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.8f, 1f)]
        private void InitializeClocks()
        {
            _gameDayTimeElapsed = LoadSaveTime();
            _baseTick = CalculateGameTickLength();
            _baseTickDelta = _baseTick;
            _displayTickLength = CalculateDisplayClockTickLength(_preferredDisplayTimeDelta);
            _displayTickDelta = _displayTickLength;
            _systemDebugMultiplier = CalculateSystemDebugMultiplier();
        }
        #endregion
    }
}

