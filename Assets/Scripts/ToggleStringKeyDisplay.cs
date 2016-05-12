using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleStringKeyDisplay : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if( Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T) ) {
			M10NText.displayKeyLabel = !M10NText.displayKeyLabel;
		}
	}
}
