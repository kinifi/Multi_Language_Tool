using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[CreateAssetMenuAttribute( fileName = "Languages", menuName = "Multi-Language/Languages", order = 1000)]
public class M10NStringDatabase : ScriptableObject {

	// 
	[SerializeField]
	public List<string> keys;

	// TODO: make it flat
	[SerializeField]
	private M10NStringTable[] m_database;

	public int languageCount {
		get {
			int c = 0;
			for(int i =0; i < m_database.Length;++i) {
				if(m_database[i]!=null) ++c;
			}
			return c;
		}
	}

	public SystemLanguage[] languages {
		get {
			SystemLanguage[] l = new SystemLanguage[languageCount];
			int c = 0;
			for(int i =0; i < m_database.Length;++i) {
				if(m_database[i]!=null){ 
					l[c++] = (SystemLanguage)i;
				}
			}
			return l;
		}
	}

	void OnEnable() {
		if( m_database == null ) {
			int langMax = 0; 
			Array a = Enum.GetValues(typeof(SystemLanguage));
			foreach(object o in a) {
				int v = (int)o;
				langMax = UnityEngine.Mathf.Max(v, langMax);
			}
			m_database = new M10NStringTable[langMax];

//			for(int i=0; i<m_database.Length; ++i) {
//				if( m_database[i] == null ) Debug.Log("M10NStringTable is null on:" + (SystemLanguage) i);
//				m_database[i].language = (SystemLanguage) i;
//			}
		}

		if( keys == null )
		{
			keys = new List<string>();
		}
	}

	public void AddLanguage( SystemLanguage lang ) {
		if( m_database[(int)lang] == null ) {
			m_database[(int)lang] = new M10NStringTable(lang);
		}
	}

	public void RemoveLanguage( SystemLanguage lang ) {
		m_database[(int)lang] = null;
	}

	public M10NStringTable GetStringTable( SystemLanguage lang ) {
		return m_database[(int)lang];
	}

	public void AddTextEntry(string key) {
		Assert.IsNotNull(key);

		if( !keys.Contains(key) ) {
			keys.Add(key);
		}

		for(int i = 0; i < m_database.Length; ++i) {

			if( m_database[i] == null ) {
				continue;
			}
			m_database[i].EnsureValuesForKeys(keys.Count);
		}
	}

	public void SetTextEntry(SystemLanguage lang, string key, string value) {
		Assert.IsNotNull(key);
		Assert.IsNotNull(value);
		Assert.IsNotNull(m_database[(int)lang]);

		int index = 0;

		if( !keys.Contains(key) ) {
			index = keys.Count;
			keys.Add(key);
		} else {
			index = keys.IndexOf(key);
		}

		for(int i = 0; i < m_database.Length; ++i) {

			if( m_database[i] == null ) {
				continue;
			}
			m_database[i].EnsureValuesForKeys(keys.Count);
		}

		m_database[(int)lang].values[index].text = value;
	}

	public void RemoveTextEntry(string key) {
		Assert.IsNotNull(key);

		if( !keys.Contains(key) ) {
			return;
		}

		int index = keys.IndexOf(key);
		keys.RemoveAt(index);

		for(int i = 0; i < m_database.Length; ++i) {

			if( m_database[i] == null ) {
				continue;
			}
			m_database[i].values.RemoveAt(index);
		}
	}

	public void RenameTextEntryKey( string oldKey, string newKey ) {
		Assert.IsNotNull(oldKey);
		Assert.IsNotNull(newKey);

		if( !keys.Contains(oldKey) ) {
			return;
		}

		int index = keys.IndexOf(oldKey);

		keys[index] = newKey;
	}
}