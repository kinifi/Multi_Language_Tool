using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

	public string levelToLoad;


	public void changeSceneNow()
	{
		if(levelToLoad != null || levelToLoad != "" || levelToLoad != " ")
		{
			Application.LoadLevel(levelToLoad);
		}
	}

}
