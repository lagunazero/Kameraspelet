using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResidentNav : MonoBehaviour {
	
	public enum Action
	{
		Idling,
		Moving,
	}
	
	public NavMeshAgent nav;
	public Action action = Action.Idling;
	
	public int checkFrequency = 5;
	private float checkCounter = 0;
	private Vector3 checkLastPosition;
	public float checkTooCloseDistance = 2;
	
	public float? closeEnoughArrived;
	public float closeEnoughRange = 3;
	public float closeEnoughTime = 5;
	
	public void SetDestination(Vector3 dest, System.Action arrivalFunc)
	{
		nav.SetDestination(dest);
		
		//Reset some stuff
		closeEnoughArrived = null;
		checkCounter = 0;
		checkLastPosition = transform.position;
		
		//Since we don't know for sure if it's running or not, stop it to be sure.
		action = Action.Moving;
		StopCoroutine("CheckDistance");
		StartCoroutine("CheckDistance", arrivalFunc);
	}
	
	IEnumerator CheckDistance(System.Action arrivalFunc)
	{
		while(action == Action.Moving)
		{
			//Some safety code if the agent can't get close enough.
			//If she hangs around decently close for some time, just accept that.
			if(nav.remainingDistance < closeEnoughRange)
			{
				if(closeEnoughArrived.HasValue)
				{
					if(Time.timeSinceLevelLoad - closeEnoughArrived.Value >= closeEnoughTime
						|| nav.remainingDistance == 0)
					{
						action = Action.Idling;
						if(arrivalFunc != null)
							arrivalFunc();
					}
				}
				else
				{
					closeEnoughArrived = Time.timeSinceLevelLoad;
				}
			}
			//Some safety code if the agent can't move.
			//If she hangs around in the same spot for a while, find someplace else to be.
			else
			{
				checkCounter += Time.deltaTime;
				if(checkCounter >= checkFrequency)
				{
					checkCounter = 0;
					if(Vector3.Magnitude(transform.position - checkLastPosition) < checkTooCloseDistance)
					{
						action = Action.Idling;
						if(arrivalFunc != null)
							arrivalFunc();
					}
					else
					{
						checkLastPosition = transform.position;
					}
				}
			}
			
			yield return new WaitForSeconds(1);
		}
	}
}
