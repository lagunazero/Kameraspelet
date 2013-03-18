using UnityEngine;
using System.Collections;

public class Dance : BaseOption {
	
	public Enjoyability considerationEnjoyability;
	public Commit considerationCommit;
	public Tiredness considerationTiredness;
	public Transform location;
	public float tiringSpeed = 10;
	
	// Use this for initialization
	void Start () {
		AddConsideration(considerationEnjoyability);
		AddConsideration(considerationCommit);
		AddConsideration(considerationTiredness);
	}

	public override IEnumerator StartOption ()
	{
		var nav = GetComponent<ResidentNav>();
		nav.SetDestination(location.position, StartDancing);
		yield return new WaitForEndOfFrame();
	}
	
	public override void StopOption ()
	{
		StopCoroutine("UpdateDance");
	}
	
	public void StartDancing()
	{
		StartCoroutine("UpdateDance");
	}
	
	public IEnumerator UpdateDance()
	{
		Personality p = GetComponent<Personality>();
		while(true)
		{
			p.tiredness += tiringSpeed * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
}
