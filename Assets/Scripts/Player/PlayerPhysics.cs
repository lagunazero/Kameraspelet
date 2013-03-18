using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour
{
	public Aim aim;
	public TextPrompt textPrompt;
	public GameObject playerHead;
	public GameObject playerBody;
	public FPSWalker walker;
	
    [HideInInspector]
    public Collider standingOn = null;

    /// <summary>
    /// Resets the head's position & rotation on every Update call,
    /// to help avoid problems. If this is true, any translations
    /// must be done continously and with the full translation instead
    /// of just the delta.
    /// </summary>
    public bool lockHead = true;

    /// <summary>
    /// Keeps the body's up vector the same at all times, to
    /// help avoid problems.
    /// </summary>
    public bool lockBodyRotation = true;

    /// <summary>Plays the game from the beginning.</summary>
    public bool playFromStart = false;
    /// <summary>The animation to play when the
    /// game starts from the beginning.</summary>
    public AnimationClip gameStartClip;
    public Vector3 gameStartPosition;

    [HideInInspector]
    public Vector3 headOriginalPosition;
    [HideInInspector]
    public Quaternion headOriginalRotation;
    private Vector3 bodyOriginalRotation;

    /// <summary>
    /// Trigger to call from animations.
    /// </summary>/
//    public BaseCounter trigger;

    void Awake()
    {
    }

    void Start()
    {
		if(aim == null) Debug.LogWarning("No Aim component linked to PlayerPhysics.");
		if(playerHead == null) Debug.LogWarning("No PlayerHead component linked to PlayerPhysics.");
		if(playerBody == null) Debug.LogWarning("No PlayerBody component linked to PlayerPhysics.");
		if(textPrompt == null) Debug.LogWarning("No TextPrompt component linked to PlayerPhysics.");
		if(walker == null) Debug.LogWarning("No FPSWalker component linked to PlayerPhysics.");
		
        headOriginalPosition = playerHead.transform.localPosition;
        headOriginalRotation = playerHead.transform.localRotation;
        bodyOriginalRotation = transform.rotation.eulerAngles;
        if (playerHead.collider.enabled && playerBody.collider.enabled)
            Physics.IgnoreCollision(playerHead.collider, playerBody.collider);

        if (playFromStart)
  			StartCoroutine(SetupStart(5));
    }

    public void Update()
    {
        if (lockHead)
        {
            playerHead.transform.localPosition = headOriginalPosition;
            playerHead.transform.localRotation = headOriginalRotation;
        }
        if (lockBodyRotation)
        {
            var e = transform.rotation.eulerAngles;
            e.x = bodyOriginalRotation.x;
            e.z = bodyOriginalRotation.z;
            transform.rotation = Quaternion.Euler(e);
        }
    }

    #region Collisions
    public void OnCollisionStay(Collision c)
    {
        foreach (ContactPoint contact in c.contacts)
        {
            if ((contact.point - transform.position).y < 0)
            {
                standingOn = c.collider;
                walker.SetControl(true);
                return;
            }
        }
    }

    public void OnCollisionExit(Collision c)
    {
        standingOn = null;
        walker.SetControl(false);
    }
    #endregion Collisions

    #region Animations
    public void ResumeAllControl()
    {
        lockBodyRotation = true;
		rigidbody.useGravity = true;
		walker.rAngles = transform.rotation.eulerAngles;
        playerBody.collider.enabled = true;
        playerHead.collider.enabled = true;
        ResumeLookControl();
        walker.SetControl(true);
    }
	
	public void RemoveAllControl()
    {
        lockBodyRotation = false;
        rigidbody.useGravity = false;
        RemoveLookControl();
        walker.SetControl(false);
    }

    public void ResumeLookControl()
    {
        InputHandler.mouseSensitivity=GetComponent<PlayerInput>().mouseSensitivity;
		aim.hasControl = true;
    }

    public void RemoveLookControl()
    {
        InputHandler.mouseSensitivity=0;
		aim.hasControl = false;
    }

    public IEnumerator WaitingToStartAnimation(AnimationClip clip)
    {
        while (!Input.anyKey)
        {
            yield return new WaitForFixedUpdate();
        }
        animation.clip = clip;
        animation.Play();
    }
    #endregion Animations

    public IEnumerator SetupStart(float wait)
    {
//		transform.position=new Vector3(0,1000,0);
//		playerHead.GetComponent<Vignetting>().intensity=2000;
		
		if(wait > 0)yield return new WaitForSeconds(wait);

		transform.position = gameStartPosition;
		aim.hasControl=false;
        rigidbody.useGravity = false;
		rigidbody.velocity=Vector3.zero;
        Physics.IgnoreCollision(playerHead.collider, playerBody.collider);
        aim.canToggleLight = false;
        aim.light.enabled = false;
        playerBody.collider.enabled = false;
        playerHead.collider.enabled = false;
//        ((Blackout)FindObjectOfType(typeof(Blackout))).KillImmediate();
//        Helper.COMPONENTS_Weatherman.SetWeather(WeatherMan.WeatherTypes.rainy);
		animation.clip = gameStartClip;
		animation.Play();
    }

    public void SetTextPrompt(int index)
    {
        textPrompt.SetText(index);
    }

    public void SetTextFlicker(int on)
    {
        textPrompt.flicker = on == 1;
    }

    public void DisableTextPrompt()
    {
        textPrompt.Disable();
    }

//    public void ChangeEvent(BaseCounter t)
//    {
//        trigger = t;
//    }

//    public void TriggerEvent()
//    {
//        trigger.SendMessage("Event", new object());
//    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "manager", false);
    }

}
