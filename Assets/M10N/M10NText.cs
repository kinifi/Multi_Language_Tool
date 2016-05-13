using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[AddComponentMenu("UI/Text Reference", 10)]
public class M10NText : UnityEngine.UI.Text
{
	[SerializeField]
	private M10NStringReference m_reference;

	private GameObject m_keyLabel;

	private static bool s_showKeyLabel;

	public static bool displayKeyLabel {
		get {
			return s_showKeyLabel;
		}
		set {
			s_showKeyLabel = value;
			M10NText[] texts = GameObject.FindObjectsOfType<M10NText>();
			foreach(M10NText t in texts) {
				t.ShowKeyLabel(value);
			}
		}
	}

	protected M10NText()
    {
	}

	public M10NStringDatabase database {
		get {
			return m_reference.database;
		}
	}

	public M10NStringReference stringReference {
		get {
			return m_reference;
		}
	}

    public override string text
    {
        get
        {
			return m_reference.text;
        }
        set
        {
//            if (String.IsNullOrEmpty(value))
//            {
//                if (String.IsNullOrEmpty(m_Text))
//                    return;
//                m_Text = "";
//                SetVerticesDirty();
//            }
//            else if (m_Text != value)
//            {
//                m_Text = value;
//                SetVerticesDirty();
//                SetLayoutDirty();
//            }
        }
    }

	public void SetArgs(params object[] args) {
		m_reference.args = args;
		SetVerticesDirty();
	}

    protected override void OnEnable()
    {
        base.OnEnable();

		if(displayKeyLabel) {
			ShowKeyLabel(true);
		}
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
		base.Reset();
    }
#endif
    public override float minWidth
    {
        get { return 0; }
    }

	public override float preferredWidth
    {
        get
        {
            var settings = GetGenerationSettings(Vector2.zero);
			return cachedTextGeneratorForLayout.GetPreferredWidth(m_reference.text, settings) / pixelsPerUnit;
        }
    }

	public override float flexibleWidth { get { return -1; } }

	public override float minHeight
    {
        get { return 0; }
    }

	public override float preferredHeight
    {
        get
        {
            var settings = GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0.0f));
			return cachedTextGeneratorForLayout.GetPreferredHeight(m_reference.text, settings) / pixelsPerUnit;
        }
    }

	public override float flexibleHeight { get { return -1; } }

	public override int layoutPriority { get { return 0; } }

	void OnLanguageChanged() {
		SetVerticesDirty();
	}

	public void ShowKeyLabel(bool show) {
		//TODO:
		if(m_keyLabel != null) {
			m_keyLabel.SetActive(show);
		} else {
			GameObject labelObj = new GameObject();
			labelObj.name = "__keylabel";
			Text t = labelObj.AddComponent<Text>();
			t.text = m_reference.key;
			Outline ol = labelObj.AddComponent<Outline>() as Outline;
			//labelObj.transform.hideFlags = HideFlags.DontSave;
			labelObj.transform.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

			t.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			t.fontSize = 20;
			t.verticalOverflow = VerticalWrapMode.Overflow;
			t.horizontalOverflow = HorizontalWrapMode.Overflow;
			t.alignment = TextAnchor.MiddleCenter;
			t.color = Color.yellow;
			ol.effectColor = Color.black;

			m_keyLabel = labelObj;
			RectTransform rt = m_keyLabel.GetComponent<RectTransform>();
			rt.SetParent(transform, false);
			rt.localPosition = Vector3.zero; 
			m_keyLabel.SetActive(show);
		}
	}

	public override void SetVerticesDirty ()
	{
		if(m_keyLabel != null) {
			Text t = m_keyLabel.GetComponent<Text>();
			if(t.text != m_reference.key) {
				t.text = m_reference.key;
			}
		}
		base.SetVerticesDirty ();
	}
}
