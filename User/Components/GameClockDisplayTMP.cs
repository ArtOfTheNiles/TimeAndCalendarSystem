using UnityEngine;
using TMPro;

namespace FigmentForge.PHCC.TimeSystem
{
    /// <summary>
    /// Add to a TextMeshPro object to display the game clock.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GameClockDisplayTMP : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private void Start()
        {
            text = gameObject.GetComponent<TextMeshProUGUI>();
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