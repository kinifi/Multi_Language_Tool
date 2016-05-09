using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MultiLanguage : EditorWindow {

	//the language database that is selected
	public LanguageDatabase mLanguages;

	public bool isLoaded = false;

	//currently editing Languages
	public SystemLanguage mCurrentLanguage;

	//enum of the avaliable languages
	public SystemLanguage mSystemLanguage;
	public SystemLanguage mNewSystemLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey, m_NewLanguageValue;

	private string[] loadedLanguageList;
	private int currentLoadedLanguageListSelection;

	[MenuItem("Window/Multi-Language %l")]
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
		DetectLanguageFileFromSelection();
	}

	public void OnEnable ()
	{

		Init ();
		DetectLanguageFileFromSelection ();

	}
		
	void OnGUI()
	{

		//draw the main area 
		GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));

		//check if the current language is loaded or not
		if(isLoaded == true)
		{
			//the menu that will always display on top
			LanguageMenuBar();

			if(mLanguages.database != null)
			{
				//display the buttons and text boxes so you can add key and values
				AddLanguageKeyValues();

				//display keys and values
				LanguageKeyValueDisplay();
			}
			else
			{
				GUILayout.Label("Create A New Language");
			}

		}
		else
		{
			//the language is not loaded
			GUILayout.Label("No Language Asset Selected", "CN EntryError");

		}

		GUILayout.EndArea();

	}

	private void LanguageKeyValueDisplay()
	{
		if(mLanguages.database[currentLoadedLanguageListSelection].keys.Count != 0)
		{
			for (int i = 0; i < mLanguages.database[currentLoadedLanguageListSelection].keys.Count; i++) 
			{
				GUILayout.Label("Key: " + mLanguages.database[currentLoadedLanguageListSelection].keys[i]
					+ " | Value: " + mLanguages.database[currentLoadedLanguageListSelection].values[i]
				);
			}
		}
	}

	private void LoadDatabase ()
	{
		if(mLanguages.database == null)
		{
			if(mLanguages.database[0] != null)
			{
				mCurrentLanguage = mLanguages.database[0].languageName;
				//populate the list
				populateLanguagesList();
				Debug.Log("Database is not empty. Assigning mCurrentLanguage");
			}
		}
		else
		{
			//Debug.Log("Database is Empty");
		}
	}

	private void AddLanguageKeyValues ()
	{

		GUILayout.BeginHorizontal("HelpBox");
		m_NewLanguageKey = EditorGUILayout.TextField("Key: ", m_NewLanguageKey);
		m_NewLanguageValue = EditorGUILayout.TextField("Value: ", m_NewLanguageValue);

		//add the language you have selected
		if(GUILayout.Button("Add"))
		{
			//check if a key already exists
			if(mLanguages.database[currentLoadedLanguageListSelection].keys.Contains(m_NewLanguageKey.ToLower()))
			{
				//throw error to user if this key already exists
				EditorUtility.DisplayDialog("Mult-Language Error", "The Key Already Exists", "Okay", "");
			}
			else
			{
				//key doesn't exist
				//add the key and value to the language
				mLanguages.database[currentLoadedLanguageListSelection].keys.Add(m_NewLanguageKey.ToLower());
				mLanguages.database[currentLoadedLanguageListSelection].values.Add(m_NewLanguageValue);
				//Debug.Log("Adding: Key" + m_NewLanguageKey + " | Value: " + m_NewLanguageValue);
			}

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
			Language _newLanguage = new Language();
			_newLanguage.languageName = mNewSystemLanguage;

			mLanguages.database.Add(_newLanguage);

			//Debug.Log("Add:" + mNewSystemLanguage.ToString());
		}

		EditorGUILayout.Space();

		populateLanguagesList();

		//select the language you want to display
		currentLoadedLanguageListSelection = EditorGUILayout.Popup(currentLoadedLanguageListSelection, loadedLanguageList, EditorStyles.toolbarPopup);
		//Debug.Log(currentLoadedLanguageListSelection);

		GUILayout.EndHorizontal();

	}

	public void populateLanguagesList()
	{

		if(mLanguages == null)
		{
			loadedLanguageList = new string[0];
			return;
		}

		if(loadedLanguageList == null || mLanguages.database.Count != loadedLanguageList.Length) 
		{
			
			//create a local list
			List<string> languageList = new List<string>();

			//add all the languages to the local list
			for (int i = 0; i < mLanguages.database.Count; i++) 
			{
				languageList.Add(mLanguages.database[i].languageName.ToString());
			}

			//convert the list to an array and use it for our popupMenu
			loadedLanguageList = languageList.ToArray();
			//Debug.Log(loadedLanguageList);
		}
	}

	public void DetectLanguageFileFromSelection ()
	{
		mLanguages = null;

		if (Selection.activeObject == null && mLanguages == null)
		{
			mLanguages = null;
		}
		
		if (Selection.activeObject is LanguageDatabase && EditorUtility.IsPersistent(Selection.activeObject))
		{
			mLanguages = Selection.activeObject as LanguageDatabase;
			LoadDatabase();
			isLoaded = true;
			//Debug.Log("Language Asset Selected and Loading");
		}

	}

	public void OnFocus ()
	{
		DetectLanguageFileFromSelection();
	}


	public void OnProjectChange ()
	{
		DetectLanguageFileFromSelection ();
	}

	public void OnSelectionChange ()
	{

		DetectLanguageFileFromSelection();
		Repaint();

	}

	public void OnLostFocus () 
	{

	}

}
