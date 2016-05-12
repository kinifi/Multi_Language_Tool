using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CompleteProject
{
    public class ScoreManager : MonoBehaviour
    {
        public static int score;        // The player's score.


		M10NText text;                      // Reference to the Text component.

        void Awake ()
        {
            // Set up the reference.
			text = GetComponent <M10NText> ();

            // Reset the score.
            score = 0;

			text.SetArgs(score);
        }


        void Update ()
        {
			text.SetArgs(score);
            // Set the displayed text to be the word "Score" followed by the score value.
//            text.text += (": " + score);
//			text.SetAllDirty();
        }
    }
}