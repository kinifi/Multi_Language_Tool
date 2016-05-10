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

	// TODO: Editor Only
//	[SerializeField]
//	private string m_key;

	public M10NStringReference(M10NStringDatabase db, ref string key) {
		m_db = db;
//		m_key= key;

		// TODO: get index of key from db
		m_index = -1; 

		// TODO: register delegate event to keep track of db change (if editor)
	}

	public string text {
		get {
			Assert.IsNotNull(m_db);
			//TODO: return appropriate text of current language
			return m_db.GetStringTable(Application.currentLanguage).values[m_index].text;
		}
	}

	public string GetPluralString(long n) {
		//TODO: return string in plural-aware form
		return null;
	}

	public string Format(object[] args) {
		//TODO: return string in formatted form
		return null;
	}
}
