
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityEditor
{
	public class M10NStringTableKeyGUI
	{
		M10NStringDatabase m_db;
		PopupList.InputData m_Keys;
		string m_ChangedKey;
		bool m_CurrentChanged = false;
		bool m_ChangeWasAdd = false;
		bool m_IgnoreNextAssetLabelsChangedCall = false;

//		private static int s_MaxShownLabels = 10;

		public Action<int, string> selectionChangedDelegate;

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
			SaveKey();
		}
		
		public void OnLostFocus()
		{
			SaveKey();
		}
		
		public void KeyChangedForObject(Object asset)
		{
			if (!m_IgnoreNextAssetLabelsChangedCall && m_db != null)
			{
				m_Keys = null; // someone else changed the labels for one of our selected assets, so invalidate cache
			}
			m_IgnoreNextAssetLabelsChangedCall = false;
		}
		
		public void SaveKey()
		{
			if (m_CurrentChanged && m_Keys != null && m_db != null)
			{
				int index = m_db.IndexOfKey(m_ChangedKey);
				if(index < 0) {

					Debug.Log("Adding new key:" + m_ChangedKey + " ChangeWasAdd:" + m_ChangeWasAdd);

					m_db.AddTextEntry(m_ChangedKey);
					EditorUtility.SetDirty(m_db);

					index = m_db.IndexOfKey(m_ChangedKey);
				}

				if (selectionChangedDelegate != null) {
					selectionChangedDelegate(index, m_ChangedKey);
				}

				m_CurrentChanged = false;
			}
		}

		public void KeyListSelectionCallback (PopupList.ListElement element)
		{
			m_Keys.DeselectAll();

			m_ChangedKey = element.text.ToLower();
			element.selected = !element.selected;
			m_ChangeWasAdd = element.selected;
			m_CurrentChanged = true;
			SaveKey();
			InspectorWindow.RepaintAllInspectors();
		}

		public void InitKeynameCache(M10NStringDatabase db, string currentKey)
		{			
			// Init only if new asset
			if (m_db != db)
			{
				m_Keys = new PopupList.InputData
								{
									m_CloseOnSelection = true,
									m_AllowCustom = true,
									m_OnSelectCallback = KeyListSelectionCallback,
									m_MaxCount = 15,
									m_EnableAutoCompletion = true,
									m_SortAlphabetically = true
				                };

				for (int i=0; i < db.Count; ++i)
				{
					PopupList.ListElement element = m_Keys.NewOrMatchingElement(db[i]);
//					if ( element.filterScore < pair.Value )
//					{
//						element.filterScore = pair.Value;
//					}
					element.selected = (db[i] == currentKey);
				}
			}
		
			m_db = db;
			m_CurrentChanged = false;
		}
		
		public void OnKeyGUI(M10NStringDatabase db, string currentKey)
		{
			if(db == null) {
				return;
			}

			InitKeynameCache(db, currentKey);
			
			// For the label list as a whole
			// The previous layouting means we've already lost a pixel to the left and couple at the top, so it is an attempt at horizontal padding: 3, verical padding: 5
			// (the rounded sides of labels makes this look like the horizontal and vertical padding is the same)
			float leftPadding = 1.0f;
//			float rightPadding = 2.0f;
			float topPadding = 3.0f;
			float bottomPadding = 5.0f;
			
//			GUIStyle labelButton = EditorStyles.assetLabelIcon;
			GUIStyle labelButton = EditorStyles.boldLabel;
			
//			float buttonWidth = labelButton.margin.left + labelButton.fixedWidth + rightPadding;
			
			// Assumes we are already in a vertical layout
			GUILayout.Space (topPadding);
			
//			// Create a rect to test how wide the label list can be
//			Rect widthProbeRect = GUILayoutUtility.GetRect (0, 10240, 0, 0);
//			widthProbeRect.width -= buttonWidth; // reserve some width for the button
			
			EditorGUILayout.BeginHorizontal();
			
			// Left padding
			GUILayoutUtility.GetRect (leftPadding, leftPadding, 0, 0);
			
			// Draw labels (fully selected)
//			DrawKeyList (widthProbeRect.xMax);

			GUILayout.FlexibleSpace();

//			r.x = widthProbeRect.xMax + labelButton.margin.left;
//			r.x = labelButton.margin.left;
			if (GUILayout.Button(currentKey, labelButton))
//			if (EditorGUI.ButtonMouseDown(r, GUIContent.none, FocusType.Passive, labelButton))
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				Rect r = GUILayoutUtility.GetRect(labelButton.fixedWidth, labelButton.fixedWidth, labelButton.fixedHeight + bottomPadding, labelButton.fixedHeight + bottomPadding);
				r.xMin += lastRect.x;
				PopupWindow.Show(r, new PopupList(m_Keys));
			}

			EditorGUILayout.EndHorizontal();
		}		
	}
}
