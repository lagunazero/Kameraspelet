using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAt : MonoBehaviour
{
    public LayerMask hitLayerMask;
    public float interactHitRadius = 0.4f;
    public float interactDistance = 1f;
    public float lookAtHitRadius = 0.65f;
    public float lookAtDistance = 2.5f;
    public float updateFrequency = 0.1f;

    public bool debugLog;
    public bool drawInteractSphere = false;
    public bool drawLookAtSphere = false;

    private List<GameObject> lastHits = new List<GameObject>();
    private HashSet<GameObject> hits = new HashSet<GameObject>();

    private WaitForSeconds wait;
    private bool isTryingToInteract = false;

    void Start()
    {
        wait = new WaitForSeconds(updateFrequency);
        StartCoroutine(LookAround());
    }

    void OnDrawGizmosSelected()
    {
        //Draw where we're looking.
        if (drawInteractSphere)
            Gizmos.DrawWireSphere(transform.position + transform.forward * interactDistance, interactHitRadius);
        if (drawLookAtSphere)
            Gizmos.DrawWireSphere(transform.position + transform.forward * lookAtDistance, lookAtHitRadius);
    }

    public void Update()
    {
        //On Update we check if we'd like to make an interaction attempt.
        //Since we don't enter the coroutine on every frame we need to check
        //here instead to see if the Interact button has been pressed, but
        //we still want to keep it "pressed" until it's been passed in the
        //coroutine to avoid missing a key press.
        if (!isTryingToInteract && Input.GetButtonDown("Interact"))
            isTryingToInteract = true;
    }

    private IEnumerator LookAround()
    {
        while (true)
        {
            //Are we trying to interact?
            if (isTryingToInteract)
            {
                isTryingToInteract = false;
                foreach (RaycastHit hit in Physics.SphereCastAll(transform.position,
                    interactHitRadius, transform.forward, interactDistance, hitLayerMask))
				{                    
					hit.transform.gameObject.SendMessage("Interact",SendMessageOptions.DontRequireReceiver);
	            }
			}

            //Find the objects we're looking at.
            foreach (RaycastHit h in Physics.SphereCastAll(transform.position,
                lookAtHitRadius, transform.forward, lookAtDistance, hitLayerMask))
                hits.Add(h.transform.gameObject);

            //Iterate through the hit objects and tell them we're looking at them.
            foreach (GameObject hit in hits)
            {
                //Have we been looking at them before?
                if (lastHits.Remove(hit))
                {
                    hit.SendMessage("LookAtStay", transform.position - hit.transform.position, SendMessageOptions.DontRequireReceiver);

                    if (debugLog) Debug.Log("LookAtStay: " + hit.name);
                }
                //Did we just start looking at them?
                else
                {
                    hit.SendMessage("LookAtEnter", SendMessageOptions.DontRequireReceiver);
                    if (debugLog) Debug.Log("LookAtEnter: " + hit.name);
                }
            }

            //Tell the objects we're not looking at anymore that we've stopped looking at them.
            foreach (GameObject hit in lastHits)
            {
                if (!hits.Contains(hit))
                {
                    hit.SendMessage("LookAtExit", SendMessageOptions.DontRequireReceiver);
                    if (debugLog) Debug.Log("LookAtExit: " + hit.name);
                }
            }

            //Clear out and prepare for the next turn.
            lastHits.Clear();
            lastHits.AddRange(hits);
            hits.Clear();
            yield return wait;
        }
    }
}