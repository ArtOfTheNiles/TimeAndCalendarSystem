using UnityEngine;
using UnityEngine.UI;

namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// Add to a UI object to display the game clock.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class GameClockDisplay : MonoBehaviour
    {
        private Text text;

        private void Start()
        {
            text = gameObject.GetComponent<Text>();
        }

        /// <summary>
        /// Updates the clock display.
        /// </summary>
        /// <param name="gameTime">Units of game time; 1-1440.</param>
        public void UpdateDisplay(int gameTime)
        {
            string textToDisplay = TimeTranslator.GameTimeToStringTime(gameTime);
            text.text = textToDisplay;
        }
    }
}