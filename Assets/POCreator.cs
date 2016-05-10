using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class POCreator {

	private static List<string> mPOEntries = new List<string>();

	//setup text #1 string
	private static string PoInitEntry = 

		"msgid " + '"' + '"' + "\n" +
		"msgstr " +  '"' + '"' + "\n" +
		'"' + "Project-Id-Version: " + '"' + "\n" +
		'"' + "Report-Msgid-Bugs-To: " + '"' + "\n" +
		'"' + "POT-Creation-Date: " + DateTime.UtcNow + '"' + "\n" +
		'"' + "PO-Revision-Date:" + '"' + "\n" +
		'"' + "Last-Translator: " + '"' + "\n" +
		'"' + "Language-Team: " + '"' + "\n" +
		'"' + "Language: ja" + '"' + "\n" +
		'"' + "MIME-Version: 1.0" + '"' + "\n" +
		'"' + "Content-Type: text/plain; charset=UTF-8" + '"' + "\n" +
		'"' + "Content-Transfer-Encoding: 8bit" + '"' + "\n" +
		'"' + "X-Generator: Unity3D " + Application.unityVersion + '"' + "" + '"' + "\n";

	/// <summary>
	/// Creates the PO Language file
	/// </summary>
	/// <param name="language">Name of the file</param>
	public static void CreateEntryFile (SystemLanguage language)
	{

		if(mPOEntries.Count <= 0)
		{
			Debug.LogError("PO File Creation Error: No Entries to write");
		}
		else
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(PoInitEntry).AppendLine().AppendLine();

			for (int i = 0; i < mPOEntries.Count; i++)
			{
				builder.Append(mPOEntries[i]);
			}

			File.WriteAllText(Application.dataPath + "/" + language.ToString().ToLower() + ".po", builder.ToString());
			AssetDatabase.Refresh();
			mPOEntries = new List<string>();
			Debug.Log(language + ".po file created");
		}

	}

	//create the po file

	/// <summary>
	/// POs the entry.
	/// </summary>
	/// <returns>The PO entry.</returns>
	/// <param name="key">Key</param>
	/// <param name="msgid">Value to be translated</param>
	/// <param name="msgstr">Translated Value</param>
	/// <param name="comment">Production Comment</param>
	public static void POEntry(string key, string msgid, string msgstr, string comment = "")
	{
		string newEntry = 
		"# " + comment + "\n" +
		"#: " + key + "\n" +
		"msgid " + '"' + msgid + '"' + "\n" +
		"msgstr " + '"' + msgstr + '"' + "\n" + "\n";
		
		mPOEntries.Add(newEntry);
	}

}
