using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BitWave_Labs.AnimatedTextReveal
{
    /// <summary>
    /// Drives the sequence of text lines, fading them in and/or out according to the selected mode.
    /// </summary>
    public class AnimateText : MonoBehaviour
    {

        // Added by Adriel - Controls whether the cycling starts automatically on scene load.
        [SerializeField] private bool autoStart = true;
        // Added by Adriel - Controls whether the cycling repeats indefinitely.
        [SerializeField] private bool repeat = true;


        /// <summary>
        /// Defines which fade operations should be applied to each line.
        /// </summary>
        private enum FadeMode { FadeIn, FadeOut, FadeInAndOut }

        // Reference to the text reveal utility that handles per-character fades.
        [SerializeField] private AnimatedTextReveal animatedTextReveal;

        // The list of strings to display and animate.
        [SerializeField] private List<string> lines;

        // Controls whether to fade in, fade out, or do both for each line.
        [SerializeField] private FadeMode fadeMode;

        // If false, the last line will remain visible instead of being faded out.
        [SerializeField] private bool fadeLastLine;

        // Delay after fade-in before starting fade-out (seconds).
        [SerializeField] private float delayBeforeFadeOut = 1f;

        // Delay after fade-out before starting fade-in of next line (seconds).
        [SerializeField] private float delayBeforeFadeIn = 1f;

        // Holds the running sequence coroutine so we don't start it twice.
        private Coroutine _cycleCoroutine;

        /// <summary>
        /// Listens for the trigger key and starts the text cycling once.
        /// </summary>
        private void Start()
        {
            // On Space, begin cycling if not already started
            if (autoStart && _cycleCoroutine == null)
                _cycleCoroutine = StartCoroutine(CycleThroughLines());
        }

        /// <summary>
        /// Coroutine that iterates over each line, applies fadeIn/fadeOut according to settings
        /// </summary>
        private IEnumerator CycleThroughLines()
        {
            // Set consistent text alignment for all lines
            animatedTextReveal.TextMesh.alignment = TextAlignmentOptions.TopLeft;

            do
            {
                int index = 0;

                // Iterate through each provided line
                foreach (string line in lines)
                {
                    // Update the text content
                    animatedTextReveal.TextMesh.text = line;
                    // Clear any previous alpha settings
                    animatedTextReveal.SetAllCharactersAlpha(0);

                    // Fade in if enabled or required by combined mode
                    if (fadeMode is FadeMode.FadeIn or FadeMode.FadeInAndOut)
                        yield return StartCoroutine(animatedTextReveal.FadeText(true));

                    // If we should not fade out the last line, exit after fading in
                    // if (!fadeLastLine && index == lines.Count - 1)
                    //     yield break;

                    // If we should not fade out the last line, skip fading out
                    if (!fadeLastLine && index == lines.Count - 1)
                    {
                        // Wait before restarting the loop if repeat is true
                        yield return new WaitForSeconds(delayBeforeFadeIn);
                        continue;
                    }

                    // Fade out if enabled or required by combined mode
                    if (fadeMode is FadeMode.FadeOut or FadeMode.FadeInAndOut)
                    {
                        // Wait before starting fade-out
                        yield return new WaitForSeconds(delayBeforeFadeOut);
                        // Execute fade-out
                        yield return StartCoroutine(animatedTextReveal.FadeText(false));
                    }

                    // Wait before moving to next line
                    yield return new WaitForSeconds(delayBeforeFadeIn);

                    index++;
                }
            } while (repeat); // Restart the loop if repeat is true
        }

        private void OnDisable()
        {
            if (_cycleCoroutine != null)
            {
                StopCoroutine(_cycleCoroutine);
                _cycleCoroutine = null;
            }
        }
    }


}