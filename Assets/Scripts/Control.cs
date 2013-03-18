using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control : MonoBehaviour
{
	public Camera cam;
    public Material turnedOffMaterial;
    public List<GameObject> screens;

    // Use this for initialization
    void Start()
    {
		if(cam == null) Debug.LogWarning("No Camera component linked to Control.");
        //Screen.showCursor = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (screens.Contains(hitInfo.transform.gameObject))
                {
                    hitInfo.transform.gameObject.SendMessage("Change", turnedOffMaterial);
                }
            }
        }
    }
}
