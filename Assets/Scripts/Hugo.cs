using UnityEngine;
using System.Collections;

public class Hugo : MonoBehaviour {

	public float timeFast = 10;
	public float timeSlow = 0.1f;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Debug_Fast"))
			Time.timeScale = timeFast;
		else if(Input.GetButtonDown("Debug_Slow"))
			Time.timeScale = timeSlow;
		else if(Input.GetButtonUp("Debug_Fast") || Input.GetButtonUp("Debug_Slow"))
			Time.timeScale = 1;
	}
}
