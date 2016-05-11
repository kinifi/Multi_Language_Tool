using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[AddComponentMenu("UI/M10N Text", 10)]
public class M10NText : UnityEngine.UI.Text
{
	[SerializeField]
	private M10NStringReference m_reference;

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

    protected override void OnEnable()
    {
        base.OnEnable();
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
}
