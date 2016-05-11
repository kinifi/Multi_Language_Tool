using UnityEngine;
using UnityEditor;
using System.Collections;

public class Splitter {

	public enum SplitDirection {
		Vertical,
		Horizontal
	}

	private SplitDirection m_direction;
	private int m_minSize;
	private int m_scrollViewSizeA;
	private int m_scrollViewSizeB;
	private bool m_onResizeNow;

	public int head {
		get {
			return m_scrollViewSizeA;
		}
	}

	public int tail {
		get {
			return m_scrollViewSizeB;
		}
	}

	public Splitter(SplitDirection d, int min, int initialAmount) {
		m_direction = d;
		m_minSize = min;
		m_scrollViewSizeA = (int) Mathf.Max(min, initialAmount);
		m_onResizeNow = false;
	}

	private Rect _GetRegionRect(Rect r) {
		if(m_direction == SplitDirection.Horizontal) {
			return new Rect(m_scrollViewSizeA - 4f,0, 4f, r.height);
		} else {
			return new Rect(0,m_scrollViewSizeA - 4f, r.width, 4f);
		}
	}

	public void DoResizeScrollView(Rect r){

		Rect region = _GetRegionRect(r);

//		GUI.DrawTexture(region,EditorGUIUtility.whiteTexture);

		if(m_direction == SplitDirection.Horizontal) {
			EditorGUIUtility.AddCursorRect(region,MouseCursor.ResizeHorizontal);
		} else {
			EditorGUIUtility.AddCursorRect(region,MouseCursor.ResizeVertical);
		}

		if( Event.current.type == EventType.mouseDown && region.Contains(Event.current.mousePosition)){
			m_onResizeNow = true;
		}
		if(m_onResizeNow){
			if(m_direction == SplitDirection.Horizontal) {
				EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition, new Vector2(1.0f,1.0f)), 
					MouseCursor.ResizeHorizontal);
			} else {
				EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition, new Vector2(1.0f,1.0f)), 
					MouseCursor.ResizeVertical);
			}

			if(m_direction == SplitDirection.Horizontal) {
				m_scrollViewSizeA = (int)Mathf.Max(m_minSize, Event.current.mousePosition.x);
				m_scrollViewSizeB = (int)Mathf.Max(0, r.width - m_scrollViewSizeA);

			} else {
				m_scrollViewSizeA = (int) Mathf.Max(m_minSize, Event.current.mousePosition.y);
				m_scrollViewSizeB = (int)Mathf.Max(0, r.height - m_scrollViewSizeA);
			}

//			region.Set(region.x,m_scrollViewSizeA,region.width,region.height);
		}
		if(Event.current.type == EventType.MouseUp) {
			m_onResizeNow = false;        
		}
	}
}
