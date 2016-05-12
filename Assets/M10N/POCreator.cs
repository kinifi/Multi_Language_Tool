using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class POCreator {

	private static List<string> mPOEntries = new List<string>();


	/// <summary>
	/// Creates the PO Language file
	/// </summary>
	/// <param name="language">Name of the file</param>
	public static void CreateEntryFile (SystemLanguage language, string path)
	{

		if(mPOEntries.Count <= 0)
		{
			Debug.LogError("PO File Creation Error: No Entries to write");
		}
		else
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(PoInitEntry(language)).AppendLine().AppendLine();

			for (int i = 0; i < mPOEntries.Count; i++)
			{
				builder.Append(mPOEntries[i]);
			}

			File.WriteAllText(path, builder.ToString());
			AssetDatabase.Refresh();
			mPOEntries = new List<string>();
			Debug.Log(language + ".po file created");
		}

	}

	private static string PoInitEntry (SystemLanguage language)
	{
		
		string newLang = language.ToString().Substring(0, 2).ToLower();
		Debug.Log("Language: " + newLang);

		string PoInitEntry = 

		"msgid " + '"' + '"' + "\n" +
		"msgstr "  + '"' + '"' + "\n" +
		'"' + "Project-Id-Version: " + '"' + "\n" + 
		'"' + "Report-Msgid-Bugs-To: " + '"' + "\n" + 
		'"' + "POT-Creation-Date: 05/12/2016" + "\\n" + '"' +  "\n" +
		'"' + "PO-Revision-Date: \\n" + '"' + "\n" +
		'"' + "Last-Translator: \\n" + '"' + "\n" +
		'"' + "Language-Team: \\n" + '"' + "\n" +
		'"' + "Language: " + newLang + "\\n" + '"' + "\n" +
		'"' + "MIME-Version: 1.0\\n" + '"' + "\n" +
		'"' + "Content-Type: text/plain; charset=UTF-8\\n" + '"' + "\n" +
		'"' + "Content-Transfer-Encoding: 8bit\\n" + '"' + "\n" +
		'"' + "X-Generator: Unity3D" + Application.unityVersion + "\\n" + '"' + "\n" +
		'"' + "POT-Creation-Date: " + '"';

		return PoInitEntry;
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
		//check if msgid has quotes
		msgid.Replace("\"","\\\"");
		msgstr.Replace("\"","\\\"");

		//create entry
		string newEntry = 
		"# " + comment + "\n" +
		"#: " + key + "\n" +
		"msgid " + '"' + msgid + '"' + "\n" +
		"msgstr " + '"' + msgstr + '"' + "\n" + "\n";
		
		mPOEntries.Add(newEntry);
	}

}
