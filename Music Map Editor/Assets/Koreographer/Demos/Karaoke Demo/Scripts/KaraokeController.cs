//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2016 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Karaoke/Karaoke Controller")]
	public class KaraokeController : MonoBehaviour
	{
		[EventID]
		public string lineEventID;
		[EventID]
		public string lyricEventID;

		public List<KaraokeTextLine> textLines;

		Animator animCom;

		int lineAddIdx = 0;

		void Start()
		{
			Koreographer.Instance.RegisterForEventsWithTime(lyricEventID, OnLyricEvent);
			Koreographer.Instance.RegisterForEvents(lineEventID, OnFeedLine);

			animCom = GetComponent<Animator>();

			if (animCom == null)
			{
				Debug.LogWarning("No Animator Component on KaraokeController GameObject!  This will cause errors instead of animation!");
			}
		}

		void OnLineFeedComplete()
		{
			// Run through the text lines to shift all the contents.
			for (int i = 0; i < textLines.Count - 1; ++i)
			{
				KaraokeTextLine copyTarget = textLines[i + 1];
				textLines[i].SetTextState(copyTarget.baseText.text, copyTarget.maskRectTrans.rect.width, copyTarget.TextPos);
			}
		}

		void OnLyricEvent(KoreographyEvent koreoEvt, int sampleTime, int sampleDelta, DeltaSlice slice)
		{
			string lyric = koreoEvt.GetTextValue();

			float percent = koreoEvt.GetEventDeltaAtSampleTime(sampleTime);

			for (int i = 0; i < textLines.Count; ++i)
			{
				KaraokeTextLine curLine = textLines[i];
				if (!curLine.IsLineFull())
				{
					curLine.FillKaraokeSection(lyric, percent);
					break;
				}
			}
		}

		void OnFeedLine(KoreographyEvent koreoEvt)
		{
			string lyrics = koreoEvt.GetTextValue();

			// Only animate when we're adding to the last slot.
			//  This allows us to pre-load a few lines so they
			//  appear immediately.
			if (lineAddIdx == textLines.Count - 1)
			{
				textLines[lineAddIdx].SetTextState(lyrics, 0f, 0);
				animCom.Play("KaraokeLineFeed", 0, 0f);
			}
			else
			{
				string[] lyricLines = lyrics.Split(new string[] {"\\n"}, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string line in lyricLines)
				{
					textLines[lineAddIdx].SetTextState(line, 0f, 0);
					lineAddIdx++;
				}
			}
		}
	}
}
