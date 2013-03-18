using UnityEngine;
using System.Collections;

public class Aim : MonoBehaviour
{
	public FPSWalker walker;
	
    public float skewFactor = 0.2f;
    public Vector2 maxRotationOffset = new Vector2(20, 30);
    public Vector2 maxRotationOffsetHolding = new Vector2(20, 5);
    public Vector2 rotationOffsetSensitivity = new Vector2(2, 2);
    public int grabbedObjectMaxOffsetDown = 180;
    public int grabbedObjectMaxOffsetUp = 270;

    public bool hasControl = true;
    public bool canToggleLight = true;
    [HideInInspector]
    public bool wantsToBeEnabled;

    [HideInInspector]
    public Vector3 lastRotationOffset;

    public bool isSkewingAim = false;

    public bool resetViewWhenWalking = true;

    public void Awake()
    {
    }

    void Start()
    {
        lastRotationOffset = Vector3.zero;
        wantsToBeEnabled = light.enabled;
		
		if(walker == null) Debug.LogWarning("No FPSWalker component linked to Aim.");
    }

    float stillTimer;

    void Update()
    {
        if (!hasControl) return;

        if (resetViewWhenWalking)
        {
            if (walker.IsMoving && InputHandler.DeltaMousePosition.magnitude < 1)
            {
                stillTimer += Time.deltaTime;
            }
            else
                stillTimer = 0;
            if (stillTimer > 0.5f)
            {
                InputHandler.SetMousePosition(Vector3.Lerp(InputHandler.MousePosition, new Vector3(Screen.width / 2, Screen.height / 2, InputHandler.MousePosition.z), Time.deltaTime));
            }
        }

        //Toggle light on/off.
        if (Input.GetButtonDown("Flashlight"))
        {
            wantsToBeEnabled = !wantsToBeEnabled;
            if (canToggleLight)
                light.enabled = wantsToBeEnabled;
        }

        Vector3 focusPoint = InputHandler.MousePosition;
        Ray ray = Camera.main.ScreenPointToRay(focusPoint);

        //Sum the ray's origin (same as transform's pos) with the direction
        //to get a world position which we can then use with the handy LookAt.
        Vector3 lookAt = ray.origin + ray.direction;
        transform.LookAt(lookAt);
    }
    public void LookTowards(Vector3 spot, float speed)
    {
        //det här borde man ju kunna använda till allt möjligt.
        //Tanken är att siktet rör sig mot en punkt, att huvudet följer siktet vid behov och att kroppen följer huvudet 1:1 (fast bara i Y-led).

        //Fullösning:
        Vector3 localSpot = new Vector3(spot.x, transform.parent.parent.position.y, spot.z);

        transform.parent.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.LookRotation(localSpot), speed * Time.deltaTime);
    }
}