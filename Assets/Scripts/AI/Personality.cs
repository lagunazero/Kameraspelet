using UnityEngine;
using System.Collections;

/// <summary>
/// The game specific half of the AI.
/// The generic half is Agent.
/// </summary>
public class Personality : MonoBehaviour {
	
	public float tiredness = 0;
	
	void Start () {
	
	}
	
	void Update () {
		tiredness += Time.deltaTime;
	}
}
