using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;


public class SceneAssist : MonoBehaviour {

	public static void LevelSwitch(string name)
	{
		SceneManager.LoadScene(name);
	}

	public void SwitchLevel(string name)
	{
		LevelSwitch(name);
	}
}
