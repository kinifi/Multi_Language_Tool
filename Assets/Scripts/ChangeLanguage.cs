using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLanguage : MonoBehaviour {

	public SystemLanguage mLanguage;

	public void changeLanguageNow()
	{
		Application.currentLanguage = mLanguage;

		//TODO: this will be hidden under hood...
		M10NText[] texts = FindObjectsOfType(typeof(M10NText)) as M10NText[];
		foreach(M10NText t in texts) {
			t.SetVerticesDirty();
		}
	}

}
