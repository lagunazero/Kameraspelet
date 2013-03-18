using UnityEngine;
using System.Collections;

public class BaseConsideration : MonoBehaviour {
	
	public Personality personality;
	
	public void Start()
	{
		if(personality == null)
		{
			Debug.LogWarning("Consideration " + name + " does not have a Personality component linked to it.");
		}
	}
	
	public virtual float Rank()
	{
		return 1;
	}
	
	public virtual void Selected(){}
	public virtual void Deselected(){}
}
