using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenuAttribute( fileName = "Languages", menuName = "Multi-Language/Languages", order = 1000)]
public class LanguageDatabase : ScriptableObject {

	[SerializeField]
	public List<Language> database;

	void OnEnable() {

		if( database == null )
		{
			database = new List<Language>();
		}

	}

	public void Add( Language _language ) {
		database.Add( _language );
	}
	public void Remove( Language _language ) {
		database.Remove( _language );
	}

}