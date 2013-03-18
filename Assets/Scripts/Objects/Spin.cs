using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

    public Vector3 spinSpeed;
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(spinSpeed);
	}
}
