using System;

namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// All Definitions for the Time and Calendar System
    /// </summary>
    public static class TSDef
    {
        //Time based behaviors:
        public enum B_BehaviorPriority { None, Regular, High, Imperative };

        //Game Clock definitions:
        public enum C_PreferredTimeDelta { Fives, Tens, Fifteens, Twenties, Thirties }
        public enum C_SystemClockType { Display, System_1, System_2, System_3, System_4, System_5 }


        /////////////////////////////////////CALENDAR/////////////////////////////////////////////////

        [Flags]
        public enum Day { None = 0, Monday = 1, Tuesday = 2, Wednesday = 4, Thursday = 8, Friday = 16, Saturday = 32, Sunday = 64, All = ~0 };
        [Flags]
        public enum Season { None = 0, Spring = 1, Summer = 2, Autumn = 4, Winter = 8, All = ~0 };
        public enum WorldHoliday { None, Halloween, Christmas, GameBirthday, PlayerBirthday, NewYears };
        [Flags]
        public enum CalendarWeather { None = 0, Sunny = 1, Cloudy = 2, Fog = 4, Rain = 8, Storm = 16, Wind = 32, Snow = 64, Blizzard = 128, All = ~0 };
        public enum CalendarEvent
        {
            None,

            //Astronomical Events
            Solstice_Winter,
            Solstice_Summer,
            Equinox_Spring,
            Equinox_Autumn,
            FullMoon,
            BlueMoon,
            BloodMoon,
            PlanetaryAlignment,

            //Placeholder Events
            Festival_Spring1,
            Festival_Spring2,
            Festival_Summer1,
            Festival_Summer2,
            Festival_Autumn1,
            Festival_Autumn2,
            Festival_Winter1,
            Festival_Winter2,

            //Expected Events
            Halloween,
            HarvestFestival,
            FishingFestival,
            SakuraFestival,
            MelonFestival,
            LightFestival,
            DarkFestival,
            DayoftheDead,

            //Story Events
            Story_DeathDay,
            Story_NightmareDay,
            Story_Rememberance,
            Story_Deliverance,
            Story_End,

            //Environmental Events
            Env_SpringBerries,
            Env_SpringMushrooms,
            Env_SummerBerries,
            Env_SummerMushrooms,
            Env_AutumnBerries,
            Env_AutumnMushrooms,

        }
    }
}

