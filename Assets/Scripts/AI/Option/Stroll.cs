using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stroll : BaseOption {
	
	public Enjoyability considerationEnjoyability;
	
	public List<NavDestination> destinations;
	public NavDestination currentDest;
	public int getNewDestAttempts = 5;
	
	private bool isOngoing = true;
	
	public void Start()
	{
		AddConsideration(considerationEnjoyability);
	}

	public override IEnumerator StartOption ()
	{
		isOngoing = true;
		RandomizeDestination();
		yield return new WaitForEndOfFrame();
	}
	
	public override void StopOption ()
	{
		isOngoing = false;
	}
	
//	public void Arrived()
//	{
//		yield return new WaitForSeconds(currentDestination.GetHangAroundTime());
//	}

	void RandomizeDestination()
	{
		if(!isOngoing){ return; }
		
		var nav = GetComponent<ResidentNav>();
		//Get the new destination, including some special cases for
		//when we can't randomize.
		if(destinations.Count == 0)
		{
			Debug.LogWarning("No destinations set for " + transform.name);
			return;
		}
		else if(destinations.Count == 1)
		{
			nav.SetDestination(destinations[0].GetDestination(), RandomizeDestination);
		}
		else
		{
			int count = getNewDestAttempts;
			NavDestination newDest;
			do
			{
				newDest = destinations[Random.Range(0, destinations.Count)];
				count--;
			} while(currentDest == newDest && count > 0);
			currentDest = newDest;
			nav.SetDestination(newDest.GetDestination(), RandomizeDestination);
		}
	}
}
