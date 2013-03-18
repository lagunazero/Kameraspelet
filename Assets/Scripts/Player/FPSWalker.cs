using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FPSWalker : MonoBehaviour
{
	public Aim aim;
	public PlayerPhysics playerPhysics;
	public GameObject playerHead;
	
    #region Move fields
    public enum MoveState
    {
        idle,
        walk,
        crouchMove,
        crouchIdle,
        crouchStart,
        crouchEnd,
        run,
        runStart,
        runEnd,
    }
    private MoveState moveState = MoveState.idle;
    private float moveStateTimer;

    public float runStartTime = 0.5f;
    public float runEndTime = 0.5f;
    public float crouchStartTime = 0.5f;
    public float crouchEndTime = 0.5f;

    public float idleTurnSpeed = 50;
    public float idleBobDuration = 1;
    public Vector3 idleBobAmount = new Vector3(0.002f, 0.002f, 0.002f);

    public float walkMoveSpeed = 40;
    public float walkBackwardSpeed = 25;
    public float walkTurnSpeed = 60;
    public float walkStrafeSpeed = 25;
    public float walkBobDuration = 0.3f;
    public Vector3 walkBobAmount = new Vector3(0f, 0.03f, 0.01f);

    public float runMoveSpeed = 130;
    public float runTurnSpeed = 10;
    public float runStrafeSpeed = 0;
    public float runBobDuration = 0.1f;
    public Vector3 runBobAmount = new Vector3(0.08f, 0.05f, 0.01f);
    public Vector3 runStartBobAmount = new Vector3(0.05f, 0.05f, 0.01f);
    public float runFieldOfViewChange = 10;

    public float crouchMoveSpeed = 20;
    public float crouchBackwardSpeed = 20;
    public float crouchTurnSpeed = 120;
    public float crouchStrafeSpeed = 20;
    public float crouchBobDuration = 1;
    public Vector3 crouchBobAmount = new Vector3(0f, 0.1f, 0f);
    #endregion Move fields

    #region Look fields
    public enum MouseLook
    {
        Still, //Apply no rotation of the body when looking around
        DiscreteRotation, //Rotate the body like a normal mouse look
        ContinousRotation //Rotate the body like using a wiimote
    }
    public MouseLook mouseLook = MouseLook.ContinousRotation;
    /// <summary>The exponent to apply to the panning sensitivity.</summary>
    //public float discRotSensitivity = 1;
    /// <summary>How fast to rotate the player. Gets multiplied with TurnSpeed.</summary>
    public float discRotTurnFactor = 2;
    /// <summary>How far from the middle the mouse must be to start rotating.</summary>
    public float discRotDeadZone = 150;
    /// <summary>The exponent to apply to the panning sensitivity.</summary>
    public float contRotSensitivity = 1;
    /// <summary>How fast to rotate the player. Gets multiplied with TurnSpeed.</summary>
    public float contRotTurnFactor = 1;
    /// <summary>How far from the middle the mouse must be to start rotating.</summary>
    public float contRotDeadZone = 150;

    /// <summary>By how much to multiply the turn speed with when the mouse is by the edge.</summary>
    public float turnSpeedLookFactor = 2;
    /// <summary>By how much the ratio between middle and edge is raised to;
    /// the higher the value, the more rapid the effect of the turnSpeedLookFactor is by the edge
    /// and the less effect near the middle.</summary>
    public float turnSpeedLookExponent = 2;
    #endregion Look fields

    #region Misc fields
    /// <summary>By how much to lower the camera when crouching.</summary>
    public float cameraOffsetCrouch = -0.5f;

    public SoundPrefab footstepSoundWalk;
    public SoundPrefab footstepSoundRun;
    public SoundPrefab footstepSoundCrouch;
    public SoundPrefab footstepSoundIdleTurn;
    public SoundPrefab runStartSound;

    private float bobTimer;
    private Vector3 bobCurrent = Vector3.zero;
    private Vector3 moveVel = Vector3.zero;
    [HideInInspector]
	public Vector3 rAngles = Vector3.zero;
    private Vector3 originalScale = Vector3.zero;
    private float originalFieldOfView;

    public bool isCircularStrafe = false;
    public float strafeCircRotateSpeed = 75;
	
	public Vector3 MidScreen { get { return new Vector3(Screen.width / 2, Screen.height / 2, 0); } }
    #endregion Misc fields

    #region States and properties
    private bool hasControl = true;
    public bool HasControl { get { return hasControl; } }

    private bool isBobbingLeft;
    public bool LockControl;// { get { return Helper.COMPONENTS_DragRigidbody.isRotating; } }

    /// <summary>Returns true if the player is moving forward; straight or diagonally.</summary>
    public bool IsMovingForward { private set; get; }
    /// <summary>Returns true if the player is moving backward; straight or diagonally.</summary>
    public bool IsMovingBackward { private set; get; }
    public bool IsMoving { private set; get; }

    /// <summary>Returns true if the player is currently in any of the idle states.</summary>
    public bool IsIdle { get { return moveState == MoveState.idle || moveState == MoveState.crouchIdle; } }
    /// <summary>Returns true if the player is currently in any of the running states.</summary>
    public bool IsRunning { get { return moveState == MoveState.run || moveState == MoveState.runStart || moveState == MoveState.runEnd; } }
    /// <summary>Returns true if the player is currently in any of the crouching states.</summary>
    public bool IsCrouching { get { return moveState == MoveState.crouchIdle || moveState == MoveState.crouchMove || moveState == MoveState.crouchStart || moveState == MoveState.crouchEnd; } }
    /// <summary>Returns true if the player is currently in any of the walking states.</summary>
    public bool IsWalking { get { return moveState == MoveState.walk; } }

    private float idleTurnCounter;
    public float idleTurnDegreeSensitivity = 60;

    public bool WantsToGoForward { private set; get; }
    public bool WantsToGoBackward { private set; get; }

    /// <summary>Gets the position of the feet.</summary>
    public Vector3 PositionFeet
    { get { return new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z); } }
    #endregion States and properties

    #region Speeds
    private float TurnSpeed
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return idleTurnSpeed;
                case MoveState.walk: return walkTurnSpeed;
                case MoveState.crouchIdle: return crouchTurnSpeed;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * idleTurnSpeed + (moveStateTimer / crouchStartTime) * crouchTurnSpeed;
                case MoveState.crouchMove: return crouchTurnSpeed;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * idleTurnSpeed + (1 - moveStateTimer / crouchEndTime) * crouchTurnSpeed;
                case MoveState.runStart: return (1 - moveStateTimer / runStartTime) * walkTurnSpeed + (moveStateTimer / runStartTime) * runTurnSpeed;
                case MoveState.run: return runTurnSpeed;
                case MoveState.runEnd: return (moveStateTimer / runEndTime) * walkTurnSpeed + (1 - moveStateTimer / runEndTime) * runTurnSpeed;
                default: return 0;
            }
        }
    }

    private float ForwardSpeed
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return walkMoveSpeed;
                case MoveState.walk: return walkMoveSpeed;
                case MoveState.crouchIdle: return crouchMoveSpeed;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * walkMoveSpeed + (moveStateTimer / crouchStartTime) * crouchMoveSpeed;
                case MoveState.crouchMove: return crouchMoveSpeed;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * walkMoveSpeed + (1 - moveStateTimer / crouchEndTime) * crouchMoveSpeed;
                case MoveState.runStart: return (WantsToGoForward ? (1 - moveStateTimer / runStartTime) * walkMoveSpeed : 0) + (moveStateTimer / runStartTime) * runMoveSpeed;
                case MoveState.run: return runMoveSpeed;
                case MoveState.runEnd: return (WantsToGoForward ? (moveStateTimer / runEndTime) * walkMoveSpeed : 0) + (1 - moveStateTimer / runEndTime) * runMoveSpeed;
                default: return 0;
            }
        }
    }

    private float BackwardSpeed
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return walkBackwardSpeed;
                case MoveState.walk: return walkBackwardSpeed;
                case MoveState.crouchIdle: return crouchBackwardSpeed;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * walkBackwardSpeed + (moveStateTimer / crouchStartTime) * crouchBackwardSpeed;
                case MoveState.crouchMove: return crouchBackwardSpeed;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * walkBackwardSpeed + (1 - moveStateTimer / crouchEndTime) * crouchBackwardSpeed;
                case MoveState.runStart: return (1 - moveStateTimer / runStartTime) * walkBackwardSpeed + (moveStateTimer / runStartTime) * runMoveSpeed;
                case MoveState.run: return runMoveSpeed;
                case MoveState.runEnd: return (moveStateTimer / runEndTime) * walkBackwardSpeed + (1 - moveStateTimer / runEndTime) * runMoveSpeed;
                default: return 0;
            }
        }
    }

    private float StrafeSpeed
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return walkStrafeSpeed;
                case MoveState.walk: return walkStrafeSpeed;
                case MoveState.crouchIdle: return crouchStrafeSpeed;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * walkStrafeSpeed + (moveStateTimer / crouchStartTime) * crouchStrafeSpeed;
                case MoveState.crouchMove: return crouchStrafeSpeed;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * walkStrafeSpeed + (1 - moveStateTimer / crouchEndTime) * crouchStrafeSpeed;
                case MoveState.runStart: return (1 - moveStateTimer / runStartTime) * walkStrafeSpeed + (moveStateTimer / runStartTime) * runStrafeSpeed;
                case MoveState.run: return runStrafeSpeed;
                case MoveState.runEnd: return (moveStateTimer / runEndTime) * walkStrafeSpeed + (1 - moveStateTimer / runEndTime) * runStrafeSpeed;
                default: return 0;
            }
        }
    }

    /// <summary>By how much to translate the head, in metres.</summary>
    private Vector3 BobAmount
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return idleBobAmount;
                case MoveState.walk: return walkBobAmount;
                case MoveState.crouchIdle: return idleBobAmount;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * (WantsToGoForward ? walkBobAmount : idleBobAmount) + (moveStateTimer / crouchStartTime) * (IsMoving ? crouchBobAmount : idleBobAmount);
                case MoveState.crouchMove: return crouchBobAmount;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * (WantsToGoForward ? walkBobAmount : idleBobAmount) + (1 - moveStateTimer / crouchEndTime) * (IsMoving ? crouchBobAmount : idleBobAmount);
                case MoveState.runStart: return runBobAmount; // (1 - moveStateTimer / runStartTime) * walkBobAmount + (moveStateTimer / runStartTime) * runStartBobAmount;
                case MoveState.run: return runBobAmount;
                case MoveState.runEnd: return (moveStateTimer / runEndTime) * (WantsToGoForward ? walkBobAmount : idleBobAmount) + (1 - moveStateTimer / runEndTime) * runBobAmount;
                default: return Vector3.zero;
            }
        }
    }
    private Vector3 currentBobAmount;

    /// <summary>
    /// How long a quarter of a bob cycle takes, in seconds,
    /// e.g. from middle to an edge or from an edge to the middle.
    /// Full cycle is middle -> edge -> middle -> other edge -> middle.
    /// </summary>
    private float BobDuration
    {
        get
        {
            switch (moveState)
            {
                case MoveState.idle: return idleBobDuration;
                case MoveState.walk: return walkBobDuration;
                case MoveState.crouchIdle: return idleBobDuration;
                case MoveState.crouchStart: return (1 - moveStateTimer / crouchStartTime) * (WantsToGoForward ? walkBobDuration : idleBobDuration) + (moveStateTimer / crouchStartTime) * (IsMoving ? crouchBobDuration : idleBobDuration);
                case MoveState.crouchMove: return crouchBobDuration;
                case MoveState.crouchEnd: return (moveStateTimer / crouchEndTime) * (WantsToGoForward ? walkBobDuration : idleBobDuration) + (1 - moveStateTimer / crouchEndTime) * (IsMoving ? crouchBobDuration : idleBobDuration);
                case MoveState.runStart: return runBobDuration;// (1 - moveStateTimer / runStartTime) * walkBobDuration + (moveStateTimer / runStartTime) * runBobDuration;
                case MoveState.run: return runBobDuration;
                case MoveState.runEnd: return (moveStateTimer / runEndTime) * (WantsToGoForward ? walkBobDuration : idleBobDuration) + (1 - moveStateTimer / runEndTime) * runBobDuration;
                default: return 0;
            }
        }
    }
    private float currentBobDuration;
    #endregion Speeds

    #region Awake and Start
    void Awake()
    {
    }

    void Start()
    {
        originalScale = transform.localScale;
        originalFieldOfView = Camera.main.fieldOfView;
        currentBobAmount = BobAmount;
        currentBobDuration = BobDuration;
		if(aim == null) Debug.LogWarning("No Aim component linked to FPSWalker.");
		if(playerPhysics == null) Debug.LogWarning("No PlayerPhysics component linked to FPSWalker.");
		if(playerPhysics == null) Debug.LogWarning("No PlayerHead component linked to FPSWalker.");
    }
    #endregion Awake and Start

    #region Update
    void FixedUpdate()
    {
        if (hasControl)
        {
            playerPhysics.rigidbody.rotation = Quaternion.Euler(rAngles);

            Vector3 dir = transform.TransformDirection(moveVel);
            transform.parent.rigidbody.velocity = new Vector3(
                dir.x, transform.parent.rigidbody.velocity.y, dir.z);



            moveVel = Vector3.zero;
            rAngles = playerPhysics.rigidbody.rotation.eulerAngles;
        }
//        Debug.Log(transform.parent.rigidbody.velocity);

        //Stumble stuff
        /*
        if (standingOn.rigidbody)
            if (standingOn.rigidbody.velocity.magnitude>5); //inget stöd för colliders som barn till rigidbodies
            Stumble();
        bool stumbling=false;
        float stumbleTimer=0;
        if (stumbling)
        {
            stumbleTimer-=Time.deltaTime;
            if (stumbleTimer<=0)
            {
                RegainBalance();
            }
        }
        */
    }

    void Update()
    {
        if (HasControl)
        {
            if (!LockControl)
            {
                HandleMoveStateInput();
                HandleMoveInput();
            }
            IdleTurning();
            HeadBob();
            UpdateMoveState();
			
			            //Set some properties that might be useful.
            //TODO: Maybe remove or change.
            IsMoving = moveVel.magnitude != 0;
            IsMovingForward = moveVel.z > 0;
            IsMovingBackward = moveVel.z < 0;
            if (IsMoving) idleTurnCounter = 0;
        }

        //Apply all of the head translations and rotations.
        //Gathered here for now, to easily move into FixedUpdate if desired.
        //It also makes it easier, in one sense, with all at one place.
        if (playerPhysics.lockHead)
        {
            playerHead.transform.Translate(bobCurrent);
            if (IsCrouching)
                if (moveState == MoveState.crouchStart)
                    playerHead.transform.Translate(0, cameraOffsetCrouch * (moveStateTimer / crouchStartTime), 0);
                else if (moveState == MoveState.crouchEnd)
                    playerHead.transform.Translate(0, cameraOffsetCrouch * (1 - moveStateTimer / crouchEndTime), 0);
                else
                    playerHead.transform.Translate(0, cameraOffsetCrouch, 0);
        }
    }

    private void HandleMoveStateInput()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            switch (moveState)
            {
                //In the cases of crouchStart/End we invert the moveStateTimer to
                //effectively invert the crouching movement, allowing us to change
                //crouch "direction" whenever we want to.
                case MoveState.crouchStart: moveState = MoveState.crouchEnd;
                    moveStateTimer = crouchEndTime - crouchEndTime * (moveStateTimer / crouchStartTime); break;
                case MoveState.crouchEnd: moveState = MoveState.crouchStart;
                    moveStateTimer = crouchStartTime - crouchStartTime * (moveStateTimer / crouchEndTime); break;
                case MoveState.crouchMove: moveState = MoveState.crouchEnd; break;
                case MoveState.crouchIdle: moveState = MoveState.crouchEnd; break;
                case MoveState.idle: moveState = MoveState.crouchStart; break;
                case MoveState.walk: moveState = MoveState.crouchStart; break;
                default: break;
            }
        }
        else if (Input.GetButton("Run"))
        {
            if (!IsRunning)
                if (IsCrouching)
                    moveState = MoveState.crouchEnd;
                else if (IsWalking || IsIdle)
                {
                    if (moveState == MoveState.runEnd)
                        moveStateTimer = runStartTime - runStartTime * (moveStateTimer / runEndTime);
                    //Camera.main.fieldOfView += runFieldOfViewChange;
//                    SFXMaster.CreateFromPrefab(runStartSound, transform.parent.position, SoundPrefab.Materials.None);
                    moveState = MoveState.runStart;
                }
        }
        //Only run while the Run button is pressed.
        else if (IsRunning)// Input.GetButtonUp("Run"))
        {
            if (moveState == MoveState.runStart)
                moveStateTimer = runEndTime - runEndTime * (moveStateTimer / runStartTime);
            moveState = MoveState.runEnd;
        }
    }

    private void HandleMoveInput()
    {
        float horizontalInput = Mathf.Pow(Input.GetAxis("Horizontal"),2) * Mathf.Sign(Input.GetAxis("Horizontal"));
        float verticalInput = Mathf.Pow(Input.GetAxis("Vertical"),2) * Mathf.Sign(Input.GetAxis("Vertical"));
        float strafeInput = Input.GetAxis("Strafe");

        float dist = InputHandler.MousePosition.x - MidScreen.x;
        float ratio = dist / MidScreen.x;
        //Did we press to the right and anything we're holding isn't to the max right of us?
/*            if (horizontalInput > 0 && !Helper.COMPONENTS_DragRigidbody.isAtMaxRight)
            //Are we one the right side of the screen? If so, apply skewing.
            if (InputHandler.MousePosition.x > Helper.MidScreen.x)
                rAngles.y += horizontalInput * TurnSpeed * ((1 + Mathf.Pow(Mathf.Abs(ratio), turnSpeedLookExponent)) * turnSpeedLookFactor) * Time.deltaTime;
            else
                rAngles.y += horizontalInput * TurnSpeed * Time.deltaTime;
        //Did we press to the left and anything we're holding isn't to the max left of us?
        else if (horizontalInput < 0 && !Helper.COMPONENTS_DragRigidbody.isAtMaxLeft)
            //Are we one the left side of the screen? If so, apply skewing.
            if (InputHandler.MousePosition.x < Helper.MidScreen.x)
                rAngles.y += horizontalInput * TurnSpeed * ((1 - Mathf.Sign(ratio) * Mathf.Pow(Mathf.Abs(ratio), turnSpeedLookExponent)) * turnSpeedLookFactor) * Time.deltaTime;
            else
*/                    rAngles.y += horizontalInput * TurnSpeed * Time.deltaTime;

        //Is this a hybrid controller?
        switch (mouseLook)
        {
            case MouseLook.Still: break;
            case MouseLook.DiscreteRotation:
                if (Mathf.Abs(dist) > discRotDeadZone && ratio != 0)
                {
                    RotateTowardsAim(dist, ratio);
                    ControllerRefocus();
                }
                break;
            case MouseLook.ContinousRotation:
                if (Mathf.Abs(dist) > contRotDeadZone && ratio != 0)
                    RotateTowardsAim(dist, ratio);
                break;
        }

        //Apply strafing
        if (isCircularStrafe && strafeInput != 0)
        {
            Vector3 oldPos = transform.position;
            Vector3 oldRot = transform.rotation.eulerAngles;
            transform.RotateAround(transform.position + transform.forward, transform.up, strafeInput * strafeCircRotateSpeed * Time.deltaTime);
            Debug.Log(transform.position - oldPos);
            moveVel.x += transform.position.x - oldPos.x;
            moveVel.z += transform.position.z - oldPos.z;
            rAngles.y -= transform.rotation.eulerAngles.y - oldRot.y;
            transform.RotateAround(transform.position + transform.forward, transform.up, -strafeInput * strafeCircRotateSpeed * Time.deltaTime);

            //moveVel.x = strafeInput * StrafeSpeed * Time.deltaTime;
            //moveVel.z += Mathf.Abs(strafeInput) * StrafeSpeed * Time.deltaTime;
            //rAngles.y -= strafeInput * strafeCircRotateSpeed * Time.deltaTime;
        }
        else
            moveVel.x += strafeInput * StrafeSpeed * Time.deltaTime;
		
        WantsToGoForward = verticalInput > 0;
        WantsToGoBackward = verticalInput < 0;
        //Move forward/backward
        if ((WantsToGoForward || IsRunning))
            moveVel.z += (IsRunning ? 1 : verticalInput) * ForwardSpeed * Time.deltaTime;
        else if (WantsToGoBackward)
            moveVel.z += verticalInput * BackwardSpeed * Time.deltaTime;
    }

    private void RotateTowardsAim(float dist, float ratio)
    {
        float r = TurnSpeed * Time.deltaTime *
            ((mouseLook == MouseLook.DiscreteRotation)
            ? Mathf.Abs(ratio) * discRotTurnFactor
            : Mathf.Pow(Mathf.Abs(ratio), contRotSensitivity) * contRotTurnFactor);
        if (ratio < 0)
        {
            rAngles.y -= r;
        }
        else if (ratio > 0)
        {
            rAngles.y += r;
        }
    }

    private void ControllerRefocus()
    {
        InputHandler.SetMousePosition(Camera.main.WorldToScreenPoint(aim.transform.position + aim.transform.forward));
    }

    /// <summary>For turning on the spot, to make sound effects.</summary>
    private void IdleTurning()
    {
        if (IsIdle)
            idleTurnCounter += rAngles.y - playerPhysics.rigidbody.rotation.eulerAngles.y;
        else
            idleTurnCounter = 0;
    }

    private void HeadBob()
    {
        float ratio;
        //Negate the previous translation.
        if (!playerPhysics.lockHead)
            playerHead.transform.Translate(-bobCurrent);

        if (isBobbingLeft)
        {
            //Only find a new bob amount when passing the middle point, to avoid jerking the head.
            if ((bobTimer / currentBobDuration) > 0 && ((bobTimer - Time.deltaTime) / currentBobDuration) <= 0)
            {
                currentBobAmount = BobAmount;
                currentBobDuration = BobDuration;
            }
            bobTimer -= Time.deltaTime;
            ratio = bobTimer / currentBobDuration;
            if (ratio <= -1)
            {
                isBobbingLeft = false;
                FootSound();
            }
        }
        else
        {
            //Only find a new bob amount when passing the middle point, to avoid jerking the head.
            if ((bobTimer / currentBobDuration) < 0 && ((bobTimer + Time.deltaTime) / currentBobDuration) >= 0)
            {
                currentBobAmount = BobAmount;
                currentBobDuration = BobDuration;
            }
            bobTimer += Time.deltaTime;
            ratio = bobTimer / currentBobDuration;
            if (ratio >= 1)
            {
                isBobbingLeft = true;
                FootSound();
            }
        }
        bobCurrent.x = currentBobAmount.x * ratio;

        //Since the cycle for y and z are only half as long as the one for x,
        //we invert them half-way through.
        if (ratio > 0)
        {
            bobCurrent.y = currentBobAmount.y * ratio;
            bobCurrent.z = currentBobAmount.z * ratio;
        }
        else
        {
            bobCurrent.y = currentBobAmount.y * -ratio;
            bobCurrent.z = currentBobAmount.z * -ratio;
        }

        //Apply the translation
        if (!playerPhysics.lockHead)
            playerHead.transform.Translate(bobCurrent);

        //transform.eulerAngles = new Vector3(
        //    transform.eulerAngles.x,
        //    transform.eulerAngles.y,
        //    bobCurrentAmount);
    }

    private void FootSound()
    {
        if (playerPhysics.standingOn != null && playerPhysics.standingOn.sharedMaterial != null)
        {
            if (IsWalking)
			{
				SFXMaster.CreateFromPrefab(footstepSoundWalk, PositionFeet,
                	SFXMaster.MaterialPhysicsToSound(playerPhysics.standingOn.sharedMaterial.name));
			}
			else if (IsRunning)
                SFXMaster.CreateFromPrefab(footstepSoundRun, PositionFeet,
                    SFXMaster.MaterialPhysicsToSound(playerPhysics.standingOn.sharedMaterial.name));
            else if (IsCrouching)
                SFXMaster.CreateFromPrefab(footstepSoundCrouch, PositionFeet,
                    SFXMaster.MaterialPhysicsToSound(playerPhysics.standingOn.sharedMaterial.name));
            else if (IsIdle) //when turning on the spot
                if (Mathf.Abs(idleTurnCounter) >= idleTurnDegreeSensitivity)
                {
                    idleTurnCounter = 0;
                    SFXMaster.CreateFromPrefab(footstepSoundIdleTurn, PositionFeet,
                        SFXMaster.MaterialPhysicsToSound(playerPhysics.standingOn.sharedMaterial.name));
                }
        }
    }

    private void UpdateMoveState()
    {
        switch (moveState)
        {
            case MoveState.idle:
                if (moveVel.magnitude != 0)
                    moveState = MoveState.walk;
                break;
            case MoveState.walk:
                if (moveVel.magnitude == 0)
                    moveState = MoveState.idle;
                break;
            case MoveState.crouchStart:
                moveStateTimer += Time.deltaTime;
                if (moveStateTimer >= crouchStartTime)
                {
                    //playerHead.transform.Translate(new Vector3(0, cameraOffsetCrouch * ((moveStateTimer - crouchStartTime) / crouchStartTime), 0));
                    transform.localScale = originalScale + new Vector3(0, cameraOffsetCrouch, 0);
                    moveState = moveVel.magnitude == 0 ? MoveState.crouchIdle : MoveState.crouchMove;
                    moveStateTimer = 0;
                    //playerPhysics.transform.Translate(0, 0.5f, 0);
                }
                else
                {
                    transform.localScale = originalScale + new Vector3(0, cameraOffsetCrouch * (moveStateTimer / crouchStartTime), 0);
                    //playerHead.transform.Translate(new Vector3(0, cameraOffsetCrouch * (Time.deltaTime / crouchStartTime), 0));
                }
                break;
            case MoveState.crouchEnd:
                moveStateTimer += Time.deltaTime;
                if (moveStateTimer >= crouchEndTime)
                {
                    //playerHead.transform.Translate(new Vector3(0, cameraOffsetCrouch * ((moveStateTimer - crouchEndTime) / crouchEndTime), 0));
                    transform.localScale = originalScale;
                    moveState = moveVel.magnitude == 0 ? MoveState.idle : MoveState.walk;
                    moveStateTimer = 0;
                }
                else
                {
                    transform.localScale = originalScale + new Vector3(0, cameraOffsetCrouch * (1 - (moveStateTimer / crouchEndTime)), 0);
                    //playerHead.transform.Translate(-new Vector3(0, cameraOffsetCrouch * (Time.deltaTime / crouchEndTime), 0));
                }
                break;
            case MoveState.runStart:
                moveStateTimer += Time.deltaTime;
                if (moveStateTimer >= runStartTime)
                {
                    Camera.main.fieldOfView += runFieldOfViewChange * ((moveStateTimer - runStartTime) / runStartTime);
                    moveState = moveVel.magnitude == 0 ? MoveState.idle : MoveState.run;
                    moveStateTimer = 0;
                }
                else
                    Camera.main.fieldOfView += runFieldOfViewChange * (Time.deltaTime / runStartTime);
                break;
            case MoveState.runEnd:
                moveStateTimer += Time.deltaTime;
                if (moveStateTimer >= runEndTime)
                {
                    Camera.main.fieldOfView = originalFieldOfView;
                    //Camera.main.fieldOfView -= runFieldOfViewChange * ((moveStateTimer - runEndTime) / runEndTime);
                    moveState = moveVel.magnitude == 0 ? MoveState.idle : MoveState.walk;
                    moveStateTimer = 0;
                }
                else
                    Camera.main.fieldOfView -= runFieldOfViewChange * (Time.deltaTime / runEndTime);
                break;
            case MoveState.run:
                if (moveVel.magnitude == 0)
                    moveState = MoveState.idle;
                break;
            case MoveState.crouchIdle:
                if (moveVel.magnitude != 0)
                    moveState = MoveState.crouchMove;
                break;
            case MoveState.crouchMove:
                if (moveVel.magnitude == 0)
                    moveState = MoveState.crouchIdle;
                break;
            default: break;
        }
    }
    #endregion Update

    #region Various actions and behaviours
    void Stumble()
    {
        //    isStumbling=true;
        //    canControl=false;
        //    moveVel=new Vector3(0,0,stumbleVel);
        //    stumbleTimer=sTime;
    }

    void RegainBalance()
    {
        //    isStumbling=false;
        //    canControl=true;
    }

    public void SetAngle(Vector3 angle)
    {
        rAngles = angle;
    }

    public void SetControl(bool on)
    {
        if (on != hasControl)
            if (on)
                hasControl = true;
            else
            {
                hasControl = false;
                moveVel = Vector3.zero;
            }
    }
    #endregion Various actions and behaviours
}