using UnityEngine;
using System.Collections;

public class Sleep : BaseOption {

	public Tiredness considerationTiredness;
	public Enjoyability considerationEnjoyability;
	public Commit considerationCommit;
	public Transform location;
	public float recoverySpeed = 50;
	
	void Start () {
		AddConsideration(considerationTiredness);
		AddConsideration(considerationEnjoyability);
		AddConsideration(considerationCommit);
	}

	public override IEnumerator StartOption ()
	{
		var nav = GetComponent<ResidentNav>();
		nav.SetDestination(location.position, StartSleeping);
		yield return new WaitForEndOfFrame();
	}
	
	public override void StopOption ()
	{
		StopCoroutine("UpdateSleep");
	}
	
	public void StartSleeping()
	{
		StartCoroutine("UpdateSleep");
	}
	
	private IEnumerator UpdateSleep()
	{
		Personality p = GetComponent<Personality>();
		while(p.tiredness > 0)
		{
			yield return new WaitForEndOfFrame();
			p.tiredness = Mathf.Max(0, p.tiredness - Time.deltaTime * recoverySpeed);
		}
		Debug.Log(p.tiredness);
	}
}
