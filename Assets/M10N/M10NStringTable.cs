using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class M10NStringTable {

	[SerializeField]
	public SystemLanguage language;

	[SerializeField]
	public List<M10NString> values = new List<M10NString>();

	[SerializeField]
	public bool isEditing = false;

	public M10NStringTable(SystemLanguage lang) {
		language = lang;
	}

	public void EnsureValuesForKeys(int keyCount) {

		if(values.Count < keyCount) {
			while(values.Count < keyCount) {
				values.Add(new M10NString());
			}
		}
	}
}