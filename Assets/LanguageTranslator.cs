using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//require a text component
public class LanguageTranslator : MonoBehaviour {

	//public variable for LanguageDatabase
	public LanguageDatabase mDatabase;

	// Use this for initialization
	void Start () {

		//check if mDatabase if null
		if(mDatabase == null)
		{
			Debug.LogError("Language Database Is Not Assigned");
		}

		//get the system current language

		//check if the database has a value for that language

		//if it does have a value for that language then change the text

		//if not. Default to the text that is entered in the text box

	}

}
