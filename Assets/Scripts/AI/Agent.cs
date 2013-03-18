using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The generic half of the AI. This could be applied to other games without modification.
/// The game specific half is Personality.
/// </summary>
public class Agent : MonoBehaviour {
	
	public List<BaseOption> options = new List<BaseOption>();
	public BaseOption currentOption;
	public float optionInstability = 0.1f;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(UpdateOption());
	}
	
	IEnumerator UpdateOption()
	{
		while(true)
		{
			for(int i = 0; i < options.Count; i++)
			{
				if((currentOption == null && options[i] != null) ||
					(currentOption != options[i] &&  currentOption.Rank() < options[i].Rank() * Random.Range(1 - optionInstability, 1 + optionInstability)))
				{
					Debug.Log(transform.name + ": " + (currentOption != null ? currentOption.GetType().ToString() : "null") + " -> " + options[i].GetType());
					//Stop the old Option.
					if(currentOption != null)
					{
						for(int k = 0; k < currentOption.considerations.Count; k++)
							currentOption.considerations[k].Deselected();
						currentOption.StopOption();
					}
					
					currentOption = options[i];
					//Start the new Option.
					for(int k = 0; k < currentOption.considerations.Count; k++)
						currentOption.considerations[k].Selected();
					StartCoroutine(currentOption.StartOption());
					break;
				}
			}
			yield return new WaitForSeconds(1);
		}
	}
}
