using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class M10NPOCreator {

	//the po file headers that are required
	private static string CreatePoHeaderEntry (SystemLanguage language)
	{
		
		string newLang = language.ToString().Substring(0, 2).ToLower();

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
	private static string CreatePOEntry(string key, string msgid, string msgstr, string comment = "")
	{
		//check if msgid has quotes
		msgid.Replace("\"","\\\"");
		msgstr.Replace("\"","\\\"");

		//create entry
		string newEntry = 
		"# " + comment + "\n" +
		"#: " + key + "\n" +
		"msgctxt " + '"' + key + '"' + "\n" +
		"msgid " + '"' + msgid + '"' + "\n" +
		"msgstr " + '"' + msgstr + '"' + "\n" + "\n";
		
		return newEntry;
	}

	private class POEntry {
		public string msgid;
		public string msgctxt;
		public string msgstr;
		public List<string> comments;

		public POEntry() {
			comments = new List<string>();
		}

		public bool ready {
			get {
				return msgid != null && msgctxt != null && msgstr != null &&
					msgid.Length > 0;
			}
		}
	}

	private static SystemLanguage ReadPOHeader(StreamReader r) {

		SystemLanguage l = SystemLanguage.Unknown;

		// read header
		while(!r.EndOfStream) {
			string line = r.ReadLine();
			if(line == null ) {
				break;
			}
			line = line.Trim();
			if(line.Length == 0) {
				break;
			}

			if(line.Contains("Language:"))
			{
				line = line.Replace("Language:", "").Trim().Trim('\"').Replace("\\n", "").Trim();
				l = M10NEditorUtility.ISOToSystemLanguage(line);
			}
		}

		return l;
	}

	private static POEntry ReadNextEntry(StreamReader r) {

		POEntry e = null;

		while( !r.EndOfStream ) {
			string line = r.ReadLine();
			if(line == null ) {
				break;
			}
			line = line.Trim();
			if(line.Length == 0) {
				break;
			}
				
			if(line.Contains("msgid"))
			{
				if(e == null) e = new POEntry();
				if( e.msgid != null ) {
					Debug.LogWarning("[Bad entry]Skipping " + e.msgctxt + " " + e.msgid);
					break;
				}
				line = line.Replace("msgid", "").Trim().Trim('\"');
				e.msgid = line;
			}
			else if(line.Contains("msgctxt"))
			{
				if(e == null) e = new POEntry();
				if( e.msgctxt != null ) {
					Debug.LogWarning("[Bad entry]Skipping " + e.msgctxt + " " + e.msgid);
					break;
				}
				line = line.Replace("msgctxt", "").Trim().Trim('\"');
				e.msgctxt = line;
			}
			else if(line.Contains("msgstr"))
			{
				if(e == null) e = new POEntry();
				if( e.msgstr != null ) {
					Debug.LogWarning("[Bad entry]Skipping " + e.msgctxt + " " + e.msgid);
					break;
				}
				line = line.Replace("msgstr", "").Trim().Trim('\"');
				e.msgstr = line;
			}
			else if(line.StartsWith("\""))
			{
				if(e!=null && e.msgstr != null) {
					line = line.Trim().Trim('\"');
					e.msgstr = e.msgstr + line;
				}
			}
			else if(line.Contains("#:"))
			{
				if(e == null) e = new POEntry();
				line = line.Replace("#:", "").Trim();
				e.comments.Add(line);
			}
		}

		if(e!=null && e.ready) {
			return e;
		}

		return null;
	}

	public static void ImportFile(M10NStringDatabase db, string newPath, 
		SystemLanguage refLanguage = SystemLanguage.Unknown, bool updateRefLanguage = false)
	{
		List<POEntry> entries = new List<POEntry>();

		SystemLanguage parsedLanguage = SystemLanguage.Unknown;

		//start parsing each line
		using(StreamReader file = new StreamReader(newPath))
		{
			// read header
			parsedLanguage = ReadPOHeader(file);
			if( parsedLanguage == SystemLanguage.Unknown ) {
				Debug.LogError("Unknown language po file: " + newPath);
				return ;
			}

			while(!file.EndOfStream) {
				POEntry e = ReadNextEntry(file);
				if(e != null) {
					entries.Add(e);
				}
			}
		}

		///done parsing now add them to the language.asset file
		foreach(POEntry e in entries)
		{
			if( e.msgid == null || e.msgctxt == null || e.msgstr == null ) {
				Debug.Log("Skipping:" + e.msgctxt + " " + e.msgid);
				continue;
			}

			//add the key and value to the language file
			db.SetTextEntry(parsedLanguage, e.msgctxt, e.msgstr);

			if( refLanguage != SystemLanguage.Unknown ) {
				string refValue = db[refLanguage].values[ db.IndexOfKey(e.msgctxt) ].text;
				if( refValue != e.msgid ) {
					if( updateRefLanguage ) {
						db.SetTextEntry(refLanguage, e.msgctxt, e.msgid);
					} else {
						Debug.LogWarning("Reference Language is different for key["+e.msgctxt+"]:\\n Is: " + refValue +
							"\\nPO: " + e.msgid);
					}
				}
			}
		}

	}

	public static void ExportFile(M10NStringDatabase db, SystemLanguage targetLanguage, SystemLanguage referenceLanguage, string newPath)
	{

		StringBuilder builder = new StringBuilder();
		builder.Append(CreatePoHeaderEntry(targetLanguage)).AppendLine().AppendLine();
			
		//get the mLanguages Object
		//get the keys
		for(int i = 0; i < db.Count; ++i) 
		{
			string key    = db[i];
			string msgid  = db[referenceLanguage].values[i].text;
			string msgstr = db[targetLanguage].values[i].text;

			//for each key, create an entry
			builder.Append( M10NPOCreator.CreatePOEntry(key, msgid, msgstr) );
		}

		File.WriteAllText(newPath, builder.ToString());
	}

}
