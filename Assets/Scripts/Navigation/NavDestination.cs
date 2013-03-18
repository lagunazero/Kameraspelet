using UnityEngine;
using System.Collections;

public class NavDestination : MonoBehaviour {

	public float hangAroundTimeMin = 3;
	public float hangAroundTimeMax = 20;
	
	public float proximity = 2;
	
	public float GetHangAroundTime()
	{
		return Random.Range(hangAroundTimeMin, hangAroundTimeMax);
	}
	
	public Vector3 GetDestination()
	{
		return new Vector3(
			transform.position.x + Random.Range(-proximity, proximity),
			transform.position.y,
			transform.position.z + Random.Range(-proximity, proximity));
	}
}
