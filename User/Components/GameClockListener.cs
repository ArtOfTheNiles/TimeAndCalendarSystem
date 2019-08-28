using UnityEngine;

namespace FigmentForge.PHCC.TimeSystem
{
    public class GameClockListener : MonoBehaviour
    {
        [Tooltip("Which System Clock will this listener listen to?")]
        [SerializeField]
        private TSDef.C_SystemClockType assignedClock = TSDef.C_SystemClockType.Display;

        [Tooltip("Attach this event to whichever methods you need to be affected by the system clock.")]
        [SerializeField]
        public GameClockEvent OnGameTick = new GameClockEvent();

        public void ReceiveUpdateTick(int gameTime, TSDef.C_SystemClockType clockType)
        {
            if (clockType == assignedClock)
            {
                OnGameTick.Invoke(gameTime);
            }
        }

        private void OnEnable()
        {
            GameClock.Instance.AddTickListener(this);
        }

        private void OnDisable()
        {
            GameClock.Instance.RemoveTickListener(this);
        }
        //private void OnDestroy()//Catch in case of failure.
        //{
        //    GameClock.Instance.RemoveTickListener(this);
        //}

    }
}
