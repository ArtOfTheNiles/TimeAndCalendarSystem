using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FigmentForge.PHCC.TimeSystem
{
    [CreateAssetMenu(fileName = "BehaviorSchedule", menuName = "FigmentForge/Time/BehaviorSchedule")]
    public class BehaviorSchedule : ScriptableObject
    {
        [Title("Schedule Parameters: ")]
        [ReadOnly]
        public string behaviorScheduleName;

        [SerializeField]
        private TSDef.B_BehaviorPriority _priority = TSDef.B_BehaviorPriority.Regular;
        public TSDef.B_BehaviorPriority SchedulePriority
        {
            get { return _priority; }
        }

        [SerializeField]
        [Tooltip("Is this behavior schedule defined by a certain event?")]
        [HorizontalGroup("Events", Width = 0.5f)]
        private TSDef.CalendarEvent calendarEvent = TSDef.CalendarEvent.None;

        [SerializeField]
        [Tooltip("What weather conditions can this behavior proceed under?")]
        [HorizontalGroup("Events", Width = 0.5f)]
        private TSDef.CalendarWeather acceptableWeather;

        [SerializeField]
        [Tooltip("What days can this behavior be active during?")]
        [HorizontalGroup("Events2", Width = 0.5f)]
        private TSDef.Day applicableDay = TSDef.Day.All;

        [SerializeField]
        [Tooltip("What Seasons can this behavior be active during?")]
        [HorizontalGroup("Events2", Width = 0.5f)]
        private TSDef.Season applicableSeason = TSDef.Season.All;

        [Space(20)]

        public List<ScheduleItem> behaviorSchedule = new List<ScheduleItem>();

        /// <summary>
        /// Initializes the behavior list. NOT TO BE USED IN REAL TIME.
        /// </summary>
        [Button(ButtonSizes.Large), GUIColor(0.7f, 1f, 0.7f), PropertyOrder(-1)]
        private void InitializeBehaviorList()
        {
            behaviorScheduleName = this.name;
            foreach (ScheduleItem behaviorScheduleItem in behaviorSchedule)
            {

                string h = behaviorScheduleItem.hours;
                string m = behaviorScheduleItem.minutes;
                string a = behaviorScheduleItem.ampm;
                string scheduleTime = (h + ":" + m + a);

                behaviorScheduleItem.scheduledTimeHasPassed = false;
                behaviorScheduleItem.internalGameTime = TimeTranslator.StringTimeToGameTime(scheduleTime);
            }

            List<ScheduleItem> sortedList = behaviorSchedule.OrderBy(o => o.internalGameTime).ToList();
            behaviorSchedule = sortedList;

        }

    }
}


