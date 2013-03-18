using UnityEngine;
using System.Collections;

public class Tiredness : BaseConsideration {

	public AnimationCurve tiredness;

	public override float Rank()
	{
		return tiredness.Evaluate(personality.tiredness);
	}
	
	public override void Selected()
	{
	}
}
