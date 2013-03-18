using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseOption : MonoBehaviour {
	
	public List<BaseConsideration> considerations = new List<BaseConsideration>();
	
	public float rank;
	
	protected void AddConsideration(BaseConsideration c)
	{
		if(c != null && !considerations.Contains(c))
			considerations.Add(c);
	}
	
	public virtual IEnumerator StartOption()
	{
		yield return new WaitForEndOfFrame();
	}
	
	public virtual void StopOption()
	{
	}
	
	public float Rank()
	{
		rank = 1;
		for(int i = 0; i < considerations.Count; i++)
		{
			rank *= considerations[i].Rank();
		}
		return rank;
	}
}
