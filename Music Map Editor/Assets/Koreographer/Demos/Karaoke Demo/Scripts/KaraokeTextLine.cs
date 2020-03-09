//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2016 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Karaoke/Karaoke Text Line")]
	public class KaraokeTextLine : MonoBehaviour
	{
		public Text				baseText;
		public Text				overlayText;
		public RectTransform	maskRectTrans;

		public int TextPos
		{
			get
			{
				return textFillIdx;
			}
		}

		int textFillIdx = 0;

		public void SetTextState(string textLine, float maskRightPos, int textPos)
		{
			baseText.text = textLine;
			overlayText.text = textLine;

			maskRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskRightPos);

			textFillIdx = textPos;
		}

		public bool FillKaraokeSection(string lyric, float eventPercent)
		{
			bool bDidFill = false;

			int startIdx = baseText.text.IndexOf(lyric, textFillIdx);

			if (startIdx >= 0)
			{
				int endIdx = startIdx + (lyric.Length - 1);
				
				UICharInfo[] charInfos = baseText.cachedTextGenerator.GetCharactersArray();

				// In theory the character array should always be filled if the text is set.  This 
				//  can fail, however, if the character array has not had a chance to update yet.
				if (charInfos.Length > 0)
				{
					// NOTE: Switching between UI Scale Modes will likely require that you resize the overlayText
					//  UI component.  This is because that size must match the size of the Canvas for this system
					//  to work as written.

					// The following line is a *partial* fix for glyph positioning and should work in most
					//  simple cases.  To see a more precise calculation, check out UnityEngine.UI.Text.OnFillVBO().
					//  The OnFillVBO shows just how the cached character mesh is perterbed.
					// To see the entire process, check out UnityEngine.UI.Graphic.UpdateGeometry().  Text can
					//  be decorated by VertexEffects or MeshEffects (depending on your Unity version).  A more
					//  comprehensive solution would involve creating your own Vertex/Mesh Effect and adding it to
					//  the end of the chain.  You would not modify the vertices there, but would rather copy all
					//  vertex positions into a list/array that this component could inspect to get positioning
					//  information from.
					float scaleOffset = 1f / baseText.pixelsPerUnit;

					float startPos = charInfos[startIdx].cursorPos.x * scaleOffset;
					float endPos = (charInfos[endIdx].cursorPos.x + charInfos[endIdx].charWidth) * scaleOffset;

					float maskRightPosX = Mathf.Lerp(startPos, endPos, eventPercent);
					
					maskRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskRightPosX);

					// We're at the end of this subsegment.  Increment our position.
					if (eventPercent >= 1f)
					{
						textFillIdx = endIdx + 1;
					}

					bDidFill = true;
				}
			}

			return bDidFill;
		}

		public bool IsLineFull()
		{
			return textFillIdx >= baseText.text.Length;
		}
	}
}
