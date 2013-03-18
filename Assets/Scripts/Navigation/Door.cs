using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public enum OpenState
	{
		Open,
		Closed,
		Opening,
		Closing
	}
	
	public enum OpenDirection
	{
		InLeft,
		InRight,
		OutLeft,
		OutRight,
		SlideLeft,
		SlideRight
	}
	
	public bool isLocked = false;
	public OpenState openState = OpenState.Closed;
	public OpenDirection openDirection;
	
	public float openSpeed = 70;
	public float openEnoughAngle = 2;
	public float openAmountAngle = 90;

	public Vector3 jointPosition;
	public float closeY, openY;
//	public Quaternion closeQuat, openQuat;
	private int nearbyResidents = 0;
	
	void Start()
	{
		//Figure out the desired close and open y angles.
		closeY = transform.eulerAngles.y;
//		closeQuat = transform.rotation;
		if(openDirection == OpenDirection.InRight ||
			openDirection == OpenDirection.OutLeft)
		{
			openY = closeY + openAmountAngle;
//			openQuat = Quaternion.Euler(transform.rotation.eulerAngles.x, closeY + openAmountAngle, transform.rotation.eulerAngles.z);
		}
		else
		{
			openSpeed *= -1;
			openY = closeY - openAmountAngle;
//			openQuat = Quaternion.Euler(transform.rotation.eulerAngles.x, closeY - openAmountAngle, transform.rotation.eulerAngles.z);
		}
		if(openY > 360) openY -= 360;
		else if(openY < 0) openY += 360;
		
		//Figure out where the joints are. A bit problematic since the trigonometry
		//behaves a bit differently depending on angle.
		if(openDirection == OpenDirection.InLeft)
		{
			if(closeY <= 90 || (closeY > 180 && closeY <= 270))
				jointPosition = renderer.bounds.min;
			else
				jointPosition = new Vector3(renderer.bounds.min.x, transform.position.y, renderer.bounds.max.z);
		}
		else if(openDirection == OpenDirection.InRight)
		{
			if(closeY <= 90 || (closeY > 180 && closeY <= 270))
				jointPosition = renderer.bounds.max;
			else
				jointPosition = new Vector3(renderer.bounds.max.x, transform.position.y, renderer.bounds.min.z);
		}
		else if(openDirection == OpenDirection.OutLeft)
		{
			if(closeY <= 90 || (closeY > 180 && closeY <= 270))
				jointPosition = renderer.bounds.min;
			else
				jointPosition = new Vector3(renderer.bounds.min.x, transform.position.y, renderer.bounds.max.z);
		}
		else if(openDirection == OpenDirection.OutRight)
		{
			if(closeY <= 90 || (closeY > 180 && closeY <= 270))
				jointPosition = renderer.bounds.max;
			else
				jointPosition = new Vector3(renderer.bounds.max.x, transform.position.y, renderer.bounds.min.z);
		}
		else if(openDirection == OpenDirection.SlideLeft)
		{
			jointPosition = renderer.bounds.min - renderer.bounds.size;
		}
		
		//Figure out where the joints are.
//		jointPosition = transform.position;
//		float size;
//		if(transform.localScale.x > transform.localScale.z)
//			size = transform.localScale.x * 0.5f;
//		else
//			size = transform.localScale.z * 0.5f;
//		if(openDirection == OpenDirection.InLeft)
//		{
//			jointPosition.x -= Mathf.Cos(transform.eulerAngles.y) * size;
//			jointPosition.z -= Mathf.Sin(transform.eulerAngles.y) * size;
//		}
//		else if(openDirection == OpenDirection.InRight)
//		{
//			jointPosition.x += Mathf.Cos(transform.eulerAngles.y) * size;
//			jointPosition.z += Mathf.Sin(transform.eulerAngles.y) * size;
//		}
//		else if(openDirection == OpenDirection.OutLeft)
//		{
//			jointPosition.x -= Mathf.Cos(transform.eulerAngles.y) * size;
//			jointPosition.z -= Mathf.Sin(transform.eulerAngles.y) * size;
//		}
//		else if(openDirection == OpenDirection.OutRight)
//		{
//			jointPosition.x += Mathf.Cos(transform.eulerAngles.y) * size;
//			jointPosition.z += Mathf.Sin(transform.eulerAngles.y) * size;
//		}
		
		//If any doors are meant to be open, open them now!
		if(openState == OpenState.Open)
			StartCoroutine(Open());
	}
	
	void OnTriggerEnter(Collider other)
	{
//		Debug.Log("On trigger enter: " + other.name);
		KeyChain keyChain = other.transform.GetComponent<KeyChain>();
		if(keyChain != null)
		{
			nearbyResidents++;
			if(openState == OpenState.Closed)
			{
				if(!isLocked || keyChain.keys.Contains(this))
				{
					StartCoroutine(Open());
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
//		Debug.Log("On trigger exit: " + other.name);
		ResidentNav res = other.transform.GetComponent<ResidentNav>();
		if(res != null)
		{
			nearbyResidents--;
			if(nearbyResidents == 0 && (openState == OpenState.Open || openState == OpenState.Opening))
			{
				StartCoroutine(Close());
			}
		}
	}

	IEnumerator Open()
	{
		openState = OpenState.Opening;
		if(openDirection == OpenDirection.SlideLeft ||
			openDirection == OpenDirection.SlideRight)
		{
//			while(Mathf.Abs(transform.position - jointPosition) > openEnoughAngle)
//			{
//				transform.TransformDirection(
//				yield return new WaitForEndOfFrame();
//			}
		}
		else
		{
//			float counter = 0;
//			bool increasesY = transform.eulerAngles.y < openY;
			while(openState == OpenState.Opening && Mathf.Abs(transform.eulerAngles.y - openY) > openEnoughAngle)
//			while(increasesY ? (transform.eulerAngles.y < openY) : (transform.eulerAngles.y > openY))
//			while(openState == OpenState.Opening && Quaternion.Angle(closeQuat, openQuat) > openEnoughAngle)
			{
//				transform.rotation = Quaternion.Lerp(closeQuat, openQuat, counter);
//				counter += openSpeed * Time.deltaTime;
				transform.RotateAround(jointPosition, Vector3.up, openSpeed * Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
			if(openState == OpenState.Opening)
				transform.RotateAround(jointPosition, Vector3.up, openY - transform.eulerAngles.y);
		}
		openState = OpenState.Open;
	}

	IEnumerator Close()
	{
		openState = OpenState.Closing;
		if(openDirection == OpenDirection.SlideLeft ||
			openDirection == OpenDirection.SlideRight)
		{
		}
		else
		{
//			float counter = 0;
//			bool increasesY = transform.eulerAngles.y < closeY;
			while(openState == OpenState.Closing && Mathf.Abs(transform.eulerAngles.y - closeY) > openEnoughAngle)
//			while(increasesY ? (transform.eulerAngles.y < closeY) : (transform.eulerAngles.y > closeY))
//			while(openState == OpenState.Opening && Quaternion.Angle(closeQuat, openQuat) > openEnoughAngle)
			{
//				transform.rotation = Quaternion.Lerp(openQuat, closeQuat, counter);
//				counter += openSpeed * Time.deltaTime;
				transform.RotateAround(jointPosition, Vector3.up, -openSpeed * Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
			if(openState == OpenState.Closing)
				transform.RotateAround(jointPosition, Vector3.up, closeY - transform.eulerAngles.y);
		}
		openState = OpenState.Closed;
	}
}
