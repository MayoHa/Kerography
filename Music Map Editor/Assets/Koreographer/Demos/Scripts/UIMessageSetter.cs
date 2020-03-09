//----------------------------------------------
//            	   Koreographer
//    Copyright © 2014-2016 Sonic Bloom, LLC
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace SonicBloom.Koreo.Demos
{
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Koreographer/Demos/UI Message Setter")]
    public class UIMessageSetter : MonoBehaviour
    {
        [EventID]
        public string eventID;

        private Text guiTextCom;
        private KoreographyEvent curTextEvent;

        private void Start()
        {
            guiTextCom = GetComponent<Text>();

            // Register for Koreography Events.  This sets up the callback.
            Koreographer.Instance.RegisterForEventsWithTime(eventID, UpdateText);
        }

        private void OnDestroy()
        {
            // Sometimes the Koreographer Instance gets cleaned up before hand.
            //  No need to worry in that case.
            if (Koreographer.Instance != null)
            {
                Koreographer.Instance.UnregisterForAllEvents(this);
            }
        }

        private void UpdateText(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            // Verify that we have Text in the Payload.
            if (evt.HasTextPayload())
            {
                // Set the text if we have a text event!
                // We can get multiple events called at the same time (if they overlap in the track).
                //  In this case, we prefer the event with the most recent start sample.
                if (curTextEvent == null ||
                    (evt != curTextEvent && evt.StartSample > curTextEvent.StartSample))
                {
                    guiTextCom.text = evt.GetTextValue();

                    // Store for later comparison.
                    curTextEvent = evt;
                }

                // Clear out the text if our event ended this musical frame.
                if (curTextEvent.EndSample < sampleTime)
                {
                    guiTextCom.text = string.Empty;

                    // Remove so that the above timing logic works when the audio loops/jumps.
                    curTextEvent = null;
                }
            }
        }
    }
}