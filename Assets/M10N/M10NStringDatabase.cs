using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[CreateAssetMenuAttribute( fileName = "StringTable", menuName = "StringTable", order = 600)]
public class M10NStringDatabase : ScriptableObject {

	// 
	[SerializeField]
	private List<string> m_keys;

	// TODO: make it flat
	[SerializeField]
	private M10NStringTable[] m_database;

	public string this[int i] {
		get {
			return m_keys[i];
		}
		set {
			m_keys[i] = value;
		}
	}

	public int Count {
		get {
			return m_keys.Count;
		}
	}

	public M10NStringTable this[SystemLanguage l] {
		get {
			return m_database[(int)l];
		}
	}

	public M10NStringTable current {
		get {
			return m_database[(int)Application.currentLanguage];
		}
	}

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

			for(int i=0; i<m_database.Length; ++i) {
				m_database[i] = new M10NStringTable((SystemLanguage)i);
			}
		}

		if( m_keys == null )
		{
			m_keys = new List<string>();
		}
	}

	public bool ContainsKey(string key) {
		return m_keys.Contains(key.ToLower());
	}

	public int IndexOfKey(string key) {
		return m_keys.IndexOf(key.ToLower());
	}

	public void AddLanguage( SystemLanguage lang ) {
		if( m_database[(int)lang] == null ) {
			m_database[(int)lang] = new M10NStringTable(lang);
		}
	}

	public void RemoveLanguage( SystemLanguage lang ) {
		m_database[(int)lang] = null;
	}

//	public M10NStringTable GetStringTable( SystemLanguage lang ) {
//		return m_database[(int)lang];
//	}
//
//	public M10NStringTable GetStringTable() {
//		return m_database[(int)Application.currentLanguage];
//	}

	public void AddTextEntry(string key) {
		Assert.IsNotNull(key);

		key = key.ToLower();

		if( !m_keys.Contains(key) ) {
			m_keys.Add(key);
		}

		for(int i = 0; i < m_database.Length; ++i) {

			if( m_database[i] == null ) {
				continue;
			}
			m_database[i].EnsureValuesForKeys(m_keys.Count);
		}
	}

	public void SetTextEntry(SystemLanguage lang, string key, string value) {
		Assert.IsNotNull(key);
		Assert.IsNotNull(value);
		Assert.IsNotNull(m_database[(int)lang]);

		int index = 0;
		key = key.ToLower();

		if( !m_keys.Contains(key) ) {
			index = m_keys.Count;
			m_keys.Add(key);
		} else {
			index = m_keys.IndexOf(key);
		}

		for(int i = 0; i < m_database.Length; ++i) {

			if( m_database[i] == null ) {
				continue;
			}
			m_database[i].EnsureValuesForKeys(m_keys.Count);
		}

		m_database[(int)lang].values[index].text = value;
	}

	public void RemoveTextEntry(string key) {
		Assert.IsNotNull(key);

		key = key.ToLower();

		if( !m_keys.Contains(key) ) {
			return;
		}

		int index = m_keys.IndexOf(key);
		m_keys.RemoveAt(index);

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

		oldKey = oldKey.ToLower();
		newKey = newKey.ToLower();

		if( !m_keys.Contains(oldKey) ) {
			return;
		}

		int index = m_keys.IndexOf(oldKey);

		m_keys[index] = newKey;
	}
}