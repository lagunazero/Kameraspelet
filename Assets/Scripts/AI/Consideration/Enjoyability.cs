using UnityEngine;
using System.Collections;

public class Enjoyability : BaseConsideration {
	
	public float degree = 1;
//	public float lastPerformed;
	
	public override float Rank ()
	{
		return degree;
	}
}
