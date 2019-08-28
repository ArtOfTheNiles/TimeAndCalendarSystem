using UnityEngine;

namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// Translates time values between the 1-1440 the game reads, and 
    /// time legible by people ie: 9:30AM. Errors will return 0.
    /// </summary>
    public static class TimeTranslator
    {
        /// <summary>Converts the following string formats to numbers the game can read.
        /// <para>09:30AM: Standard Time with leading zeroes (Preferred)</para>
        /// <para>9:30AM : Standard Time</para>
        /// <para>23:00  : Military Time</para>
        /// <para>2300   : Military Time</para>
        /// <para>All errors will return 0.</para>
        /// </summary>
        public static int StringTimeToGameTime(string readableTime)
        {

            int inputLength = readableTime.Length;
            int sendTime;
            char pad = '0'; // \u0030 in UTF-16;


            // 1.) Weed out range exceptions:
            if (inputLength > 7 || inputLength < 4)
            {
                Debug.LogError("String format length outside of acceptable range. Double check your spelling.");
                return 0;
            }


            // 2.) Convert Semi-Explicit Time to Explicit Time.
            if (inputLength == 6)
            {
                readableTime = readableTime.PadLeft(7, pad);
                inputLength = readableTime.Length;
            }


            // 3.) Calculate Explicit Time; ie 09:30AM or 12:00PM:
            if (inputLength == 7)
            {
                //First giveaway that formatting is incorrect
                if (readableTime.Substring(2, 1) != ":")
                {
                    SendGenericRT2GTError();
                    return 0;
                }

                //Attempt to parse formatting:
                int hour = -1;
                int minute = -1;
                string ampm = "XX";


                hour = int.Parse(readableTime.Substring(0, 2));
                minute = int.Parse(readableTime.Substring(3, 2));
                if (readableTime.Substring(5) == "AM"
                    || readableTime.Substring(5) == "am"
                    || readableTime.Substring(5) == "Am"
                    || readableTime.Substring(5) == "aM")
                {
                    ampm = "AM";
                }
                if (readableTime.Substring(5) == "PM"
                    || readableTime.Substring(5) == "pm"
                    || readableTime.Substring(5) == "Pm"
                    || readableTime.Substring(5) == "pM") //being really lenient with your misspellings.
                {
                    ampm = "PM";
                }

                //Catch parsing errors:
                if (hour < 1 || hour > 12) { Debug.LogError("Hours out of range."); return 0; }
                if (minute < 0 || minute > 59) { Debug.LogError("Minutes out of range."); return 0; }
                if (ampm == "XX") { Debug.LogError("Misspelled your AM/PM."); return 0; }


                //Parsing assumed correct, calculate the time:
                if (ampm == "AM" && hour == 12) { hour = 0; }
                if (ampm == "PM" && hour == 12) { hour = 12; }
                if (ampm == "PM" && hour < 12) { hour += 12; }


                sendTime = (hour * 60) + minute;
                if (sendTime == 0) { sendTime = 1440; } //1440 is midnight.
                return sendTime;
            }


            // 4.) Calculate based on Military Time with a colon:
            if (inputLength == 5)
            {
                //First giveaway that formatting is incorrect
                if (readableTime.Substring(2, 1) != ":")
                {
                    SendGenericRT2GTError();
                    return 0;
                }
                readableTime = readableTime.Remove(2, 1);
                inputLength = readableTime.Length;
            }

            // 5.) Calculate based on Military Time of 4 digits:
            if (inputLength == 4)
            {
                int hours, minutes;

                hours = int.Parse(readableTime.Substring(0, 2));
                minutes = int.Parse(readableTime.Substring(2, 2));

                //Catch parsing errors:
                if (hours < 0 || hours > 24) { Debug.LogError("Hours out of range."); return 0; }
                if (minutes < 0 || minutes > 59) { Debug.LogError("Minutes out of range."); return 0; }

                if (readableTime == "0000" || readableTime == "2400") { return 1440; }
                sendTime = (hours * 60) + minutes;
                return sendTime;
            }

            Debug.LogError("String does not conform to standards, and current filters are insufficient.");
            return 0;
        }

        /// <summary>Converts game time to the following string format:
        /// <para>Standard Time with leading zeroes; ie: "09:30AM"</para>
        /// </summary>
        public static string GameTimeToStringTime(int gameTime)
        {
            string sendTime = "_Error_";
            int hour, minute;
            string ampm = "AM";

            // 1.) Weed out range exceptions:
            if (gameTime < 1 || gameTime > 1440)
            {
                SendGenericGT2RTError();
                return "_Error_";
            }

            // 2.) Calculate time:
            if (gameTime == 1440) //Midnight scenario 1
            {
                return "12:00AM";
            }
            if (gameTime == 720) //Noon scenario 1
            {
                return "12:00PM";
            }
            if (gameTime < 720) //AM
            {
                hour = gameTime / 60;
                if (hour == 0) { hour = 12; }
                minute = gameTime % 60;
                sendTime = WriteTimeString(hour, minute, ampm);
                return sendTime;
            }
            if (gameTime >= 720) //PM
            {
                hour = (gameTime / 60) - 12;
                if (hour == 0) { hour = 12; }
                minute = gameTime % 60;
                ampm = "PM";
                sendTime = WriteTimeString(hour, minute, ampm);
                return sendTime;
            }

            SendGenericGT2RTError();
            return "_Error_";
        }

        private static string WriteTimeString(int hour, int minute, string ampm)
        {
            char pad = '0';
            return (hour.ToString().PadLeft(2, pad) + ":" + minute.ToString().PadLeft(2, pad) + ampm);
        }

        private static void SendGenericRT2GTError()
        {
            Debug.LogError("String does not conform to time standards. Return is 0. Use only 2200 | 22:00 | 9:00AM | 09:00PM");
        }

        private static void SendGenericGT2RTError()
        {
            Debug.LogError("Something went wrong converting Game Time to Readable Time. Only Ints 1-1440 accepted.");
        }
    }
}
