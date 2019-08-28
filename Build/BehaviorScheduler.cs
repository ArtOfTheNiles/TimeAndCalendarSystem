using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FigmentForge.PHCC.TimeSystem
{
    [RequireComponent(typeof(GameClockListener))]
    public class BehaviorScheduler : MonoBehaviour
    {
        #region Debug Properties
        [ReadOnly]
        [ShowInInspector]
        [PropertyOrder(-1)]
        public BehaviorSchedule CurrentBehaviorSchedule { get; private set; }

        #endregion

        /// <summary>
        /// The collection of behavior schedules for this object / character.
        /// </summary>
        public List<BehaviorSchedule> behaviorSchedules = new List<BehaviorSchedule>();

        private GameClockListener myGameClockListener;

        #region Real Time Functions
        private void Start()
        {
            AttachLocalComponents();
        }
        #endregion

        #region Implementation Functions
        //TODO Implement this method, Listens to game calendar
        public void SetCurrentBehaviorSchedule()
        {
            throw new UnityException("Method not implemented.");
        }

        //TODO Implement this method, Listens to game clock
        public void ProcessCurrentBehaviorSchedule(int totalElapsedTime)
        {
            throw new UnityException("Method not implemented.");
        }
        #endregion

        #region Initialization Functions
        ////////////////////////INITIALIZATION//////////////////////////

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.8f, 1f, 0.8f)]
        private void AttachLocalComponents()
        {
            if (myGameClockListener == null)
            {
                myGameClockListener = gameObject.GetComponent<GameClockListener>();
                if (myGameClockListener == null)
                {
                    gameObject.AddComponent<GameClockListener>();
                    myGameClockListener = gameObject.GetComponent<GameClockListener>();
                }
            }
            AttachEvents();
        }

        private void AttachEvents()
        {
            myGameClockListener.OnGameTick.AddListener(ProcessCurrentBehaviorSchedule);
        }

        #endregion
    }
}

