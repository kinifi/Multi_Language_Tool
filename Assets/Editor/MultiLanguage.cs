using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MultiLanguage : EditorWindow {

	//the language database that is selected
	public LanguageDatabase mLanguages;

	//The current path 
	public string mCurrentDataPath;

	//currently editing Languages
	public string mCurrentLanguage;

	//enum of the avaliable languages
	public SystemLanguage mSystemLanguage;
	public SystemLanguage mNewSystemLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey, m_NewLanguageValue;

	[MenuItem("Window/Multi-Language")]
	static void ShowEditor() {

		//create the editor window
		MultiLanguage editor = EditorWindow.GetWindow<MultiLanguage>();
		//the editor window must have a min size
		editor.minSize = new Vector2 (800, 600);
		//call the init method after we create our window
		editor.Init();

	}

	// Use this for initialization
	public void Init() 
	{
		//get the directory we are in so we can make our paths relative
		//Assetdatabase.LoadAsset requires a relative path
		mCurrentDataPath = System.Environment.CurrentDirectory + "/";
	}


	void OnGUI()
	{

		//draw the main area 
		GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));

		//check if the current language is loaded or not
		if(mCurrentLanguage != null)
		{
			//the menu that will always display on top
			LanguageMenuBar();

			//display the buttons and text boxes so you can add key and values
			AddLanguageKeyValues();
		}
		else
		{
			//the language is not loaded
			GUILayout.Label("No Language Asset Selected", "CN EntryError");

		}

		GUILayout.EndArea();

	}

	private void AddLanguageKeyValues ()
	{

		GUILayout.BeginHorizontal("HelpBox");
		EditorGUILayout.TextField("Key: ", m_NewLanguageKey);
		EditorGUILayout.TextField("Value: ", m_NewLanguageValue);

		//add the language you have selected
		if(GUILayout.Button("Add"))
		{
			Debug.Log("Adding: Key" + m_NewLanguageKey + " | Value: " + m_NewLanguageValue);
		}

		GUILayout.EndHorizontal();
	}

	//create the top menu bar
	private void LanguageMenuBar ()
	{

		//the menu bar 
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(Screen.width));


		//select the language you want to add
		mNewSystemLanguage = (SystemLanguage) EditorGUILayout.EnumPopup("Add New Language:", mNewSystemLanguage, EditorStyles.toolbarDropDown, GUILayout.Width(250));

		//add the language you have selected
		if(GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20)))
		{
			Debug.Log("Add:" + mNewSystemLanguage.ToString());
		}

		GUILayout.EndHorizontal();

	}

}
