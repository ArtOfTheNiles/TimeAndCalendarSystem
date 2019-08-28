using UnityEngine;
namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// This class exists to collect time data from the PC/Console.
    /// </summary>

    public class SystemTimeRetriever : MonoBehaviour
    {
        private string fullDateAndTime;
        public string GetCurrentDateAndTime
        {
            get
            {
                return System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        public SystemCalendarEvent systemCalendarEvent = new SystemCalendarEvent();


        private void Awake()
        {
            fullDateAndTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            CheckForRealWorldHoliday();
        }

        public void CheckForRealWorldHoliday()
        {
            string dayOfMonth = System.DateTime.Now.ToString("MM/dd");

            switch (dayOfMonth)
            {
                case "10/31":
                    systemCalendarEvent.Invoke(TSDef.WorldHoliday.Halloween);
                    print("Samhain is upon us.");
                    break;
                case "12/25":
                    systemCalendarEvent.Invoke(TSDef.WorldHoliday.Christmas);
                    print("Merry Christmas ya filthy animal.");
                    break;
                case "07/08":
                    systemCalendarEvent.Invoke(TSDef.WorldHoliday.PlayerBirthday);
                    print("Cheers!");
                    break;
                case "03/24":
                    systemCalendarEvent.Invoke(TSDef.WorldHoliday.PlayerBirthday);
                    print("Cheers!");
                    break;
                case "01/01":
                    systemCalendarEvent.Invoke(TSDef.WorldHoliday.NewYears);
                    print("Different clock, same you.");
                    break;
                default:

                    break;
            }
        }
    }
}
