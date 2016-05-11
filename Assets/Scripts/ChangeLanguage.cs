using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLanguage : MonoBehaviour {

	public SystemLanguage mLanguage;

	public void changeLanguageNow()
	{
		Application.currentLanguage = mLanguage;
	}

}
