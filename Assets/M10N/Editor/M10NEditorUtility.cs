using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M10NEditorUtility {

	public static string GetSystemLanguageCulture(SystemLanguage language)
	{
		switch(language) {
		case SystemLanguage.Afrikaans:		return "af";
		case SystemLanguage.Arabic:			return "ar";
		case SystemLanguage.Basque:			return "eu";
		case SystemLanguage.Belarusian:		return "be";
		case SystemLanguage.Bulgarian:		return "bg";
		case SystemLanguage.Catalan:		return "ca";
		case SystemLanguage.Chinese:		return "zh";
		case SystemLanguage.SerboCroatian:	return "hr";
		case SystemLanguage.Czech:			return "cs";
		case SystemLanguage.Danish:			return "da";
		case SystemLanguage.Dutch:			return "nl";
		case SystemLanguage.English:		return "en";
		case SystemLanguage.Estonian:		return "et";
		case SystemLanguage.Faroese:		return "fo";
		case SystemLanguage.Finnish:		return "fi";
		case SystemLanguage.French:			return "fr";
		case SystemLanguage.German:			return "de";
		case SystemLanguage.Greek:			return "el";
		case SystemLanguage.Hebrew:			return "he";
		case SystemLanguage.Hungarian:		return "hu";
		case SystemLanguage.Icelandic:		return "is";
		case SystemLanguage.Indonesian:		return "id";
		case SystemLanguage.Italian:		return "it";
		case SystemLanguage.Japanese:		return "ja";
		case SystemLanguage.Korean:			return "ko";
		case SystemLanguage.Latvian:		return "lv";
		case SystemLanguage.Lithuanian:		return "lt";
		case SystemLanguage.Norwegian:		return "no";
		case SystemLanguage.Polish:			return "pl";
		case SystemLanguage.Portuguese:		return "pt";
		case SystemLanguage.Romanian:		return "ro";
		case SystemLanguage.Russian:		return "ru";
		case SystemLanguage.Slovak:			return "sk";
		case SystemLanguage.Slovenian:		return "sl";
		case SystemLanguage.Spanish:		return "es";
		case SystemLanguage.Swedish:		return "sv";
		case SystemLanguage.Thai:			return "th";
		case SystemLanguage.Turkish:		return "tr";
		case SystemLanguage.Ukrainian:		return "uk";
		case SystemLanguage.Vietnamese:		return "vi";
		default: return "";
		}
	}

	public static SystemLanguage ISOToSystemLanguage (string langTag)
	{
		if(langTag == "af") return SystemLanguage.Afrikaans;
		if(langTag == "ar") return SystemLanguage.Arabic;
		if(langTag == "eu") return SystemLanguage.Basque;
		if(langTag == "be") return SystemLanguage.Belarusian;
		if(langTag == "bg") return SystemLanguage.Bulgarian;
		if(langTag == "ca") return SystemLanguage.Catalan;
		if(langTag == "zh-hans") return SystemLanguage.ChineseSimplified;
		if(langTag == "zh-hant") return SystemLanguage.ChineseTraditional;
		if(langTag == "cs") return SystemLanguage.Czech;
		if(langTag == "da") return SystemLanguage.Danish;
		if(langTag == "nl") return SystemLanguage.Dutch;
		if(langTag == "en") return SystemLanguage.English;
		if(langTag == "et") return SystemLanguage.Estonian;
		if(langTag == "fo") return SystemLanguage.Faroese;
		if(langTag == "fi") return SystemLanguage.Finnish;
		if(langTag == "fr") return SystemLanguage.French;
		if(langTag == "de") return SystemLanguage.German;
		if(langTag == "el") return SystemLanguage.Greek;
		if(langTag == "he") return SystemLanguage.Hebrew;
		if(langTag == "hu") return SystemLanguage.Hungarian;
		if(langTag == "is") return SystemLanguage.Icelandic;
		if(langTag == "id") return SystemLanguage.Indonesian;
		if(langTag == "it") return SystemLanguage.Italian;
		if(langTag == "ja") return SystemLanguage.Japanese;
		if(langTag == "ko") return SystemLanguage.Korean;
		if(langTag == "lv") return SystemLanguage.Latvian;
		if(langTag == "lt") return SystemLanguage.Lithuanian;
		if(langTag == "no") return SystemLanguage.Norwegian;
		if(langTag == "pl") return SystemLanguage.Polish;
		if(langTag == "pt") return SystemLanguage.Portuguese;
		if(langTag == "ro") return SystemLanguage.Romanian;
		if(langTag == "ru") return SystemLanguage.Russian;
		if(langTag == "sr") return SystemLanguage.SerboCroatian;
		if(langTag == "sk") return SystemLanguage.Slovak;
		if(langTag == "sl") return SystemLanguage.Slovenian;
		if(langTag == "es") return SystemLanguage.Spanish;
		if(langTag == "sv") return SystemLanguage.Swedish;
		if(langTag == "th") return SystemLanguage.Thai;
		if(langTag == "tr") return SystemLanguage.Turkish;
		if(langTag == "uk") return SystemLanguage.Ukrainian;
		if(langTag == "vi") return SystemLanguage.Vietnamese;
		return SystemLanguage.Unknown;
	}
}
