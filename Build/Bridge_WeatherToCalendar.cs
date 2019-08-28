using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FigmentForge.PHCC.TimeSystem
{
    public class Bridge_WeatherToCalendar : MonoBehaviour
    {
        [HorizontalGroup("Main")]
        private TSDef.CalendarWeather _none = TSDef.CalendarWeather.None;
        private TSDef.CalendarWeather _sunny = TSDef.CalendarWeather.Sunny;
        private TSDef.CalendarWeather _cloudy = TSDef.CalendarWeather.Cloudy;
        private TSDef.CalendarWeather _fog = TSDef.CalendarWeather.Fog;
        private TSDef.CalendarWeather _rain = TSDef.CalendarWeather.Rain;
        private TSDef.CalendarWeather _storm = TSDef.CalendarWeather.Storm;
        private TSDef.CalendarWeather _wind = TSDef.CalendarWeather.Wind;
        private TSDef.CalendarWeather _snow = TSDef.CalendarWeather.Snow;
        private TSDef.CalendarWeather _blizzard = TSDef.CalendarWeather.Blizzard;

    }
}

