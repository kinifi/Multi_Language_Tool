using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Language {

	[SerializeField]
	public string languageName;

	[SerializeField]
	public List<string> keys = new List<string>();

	[SerializeField]
	public List<string> values = new List<string>();

	[SerializeField]
	public bool isEditing = false;

}