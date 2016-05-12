using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct M10NStringReference {

	[SerializeField]
	private M10NStringDatabase m_db;

	[SerializeField]
	private int m_index;

	[SerializeField]
	private string m_selectedKey;

	private object[] m_args;

	// TODO: Editor Only
//	[SerializeField]
//	private string m_key;

	public M10NStringReference(M10NStringDatabase db, ref string key) {
		m_db = db;
		this.key = key;
		// TODO: register delegate event to keep track of db change (if editor)
	}

	public string text {
		get {
			if(m_db == null || m_db.keys.Count < m_index || m_index < 0) return string.Empty;

			if( m_args != null ) {
				return m_db.GetStringTable(Application.currentLanguage).values[m_index].Format(args);
			} else {
				return m_db.GetStringTable(Application.currentLanguage).values[m_index].text;
			}
		}
	}

	public object[] args {
		get {
			return m_args;
		}
		set {
			m_args = value;
		}
	}

	public M10NStringDatabase database {
		get {
			return m_db;
		}
		set {
			m_db = value;
		}
	}

	public string key {
		get {
			if(m_db == null || m_db.keys.Count < m_index || m_index < 0) return string.Empty;
			return m_db.keys[m_index];
		}
		set {
			Assert.IsTrue(m_db.keys.Contains(value));
			m_index = m_db.keys.IndexOf(value);
			m_selectedKey = value;
		}
	}

	public void SetArgs(params object[] args) {
		this.args = args;
	}

	public string GetPluralString(long n) {
		return m_db.GetStringTable(Application.currentLanguage).values[m_index].GetPluralString(n);
	}

	public string Format(object[] args) {
		return m_db.GetStringTable(Application.currentLanguage).values[m_index].Format(args);
	}

	public string PluralFormat(long n) {
		return m_db.GetStringTable(Application.currentLanguage).values[m_index].PluralFormat(n);
	}
}
