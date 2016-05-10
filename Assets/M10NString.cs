using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class M10NString {

	[System.Serializable]
	public class M10NPuralString {
		public string value;
		public long rangeMin;
		public long rangeMax;

		public M10NPuralString(string v, long rMin, long rMax) {
			value = v;
			rangeMin = rMin;
			rangeMax = rMax;
		}
	}

	[SerializeField]
	private string m_value;

	[SerializeField]
	private List<M10NPuralString> m_pluralValues;

	public M10NString() {
		m_value = string.Empty;
	}

	public M10NString(string value) {
		m_value = value;
	}

	public string text {
		get {
			return m_value;
		}
		set {
			m_value = value;
		}
	}

	public List<M10NPuralString> plurals {
		get {
			return m_pluralValues;
		}
	}

	public void AddPluralString(string v, long rMin, long rMax) {
		if(m_pluralValues == null) {
			m_pluralValues = new List<M10NPuralString>();
		}
		m_pluralValues.Add(new M10NPuralString(v, rMin, rMax));
	}

	public string GetPluralString(long n) {
		M10NPuralString s = m_pluralValues.Find( i => ( n >= i.rangeMin && n < i.rangeMax ) );
		if(s != null) {
			return s.value;
		} 
		return null;
	}

	public string Format(object[] args) {
		return string.Format(m_value, args);
	}

	public string PluralFormat(long n) {
		string s = GetPluralString(n);
		if(s != null) {
			return string.Format(s, n);
		}
		return null;
	}
}
