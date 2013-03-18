using UnityEngine;
using System.Collections;

public class Commit : BaseConsideration {
	
	public bool hasCommitted;
	public float commitStrength = 10;
	
	public override float Rank ()
	{
		return hasCommitted ? commitStrength : base.Rank();
	}
	
	public override void Selected ()
	{
		hasCommitted = true;
	}
	
	public override void Deselected ()
	{
		hasCommitted = false;
	}
}
