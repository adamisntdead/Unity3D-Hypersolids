using UnityEngine;
using System.Collections;

public class Autowalk : MonoBehaviour 
{
	private const int RIGHT_ANGLE = 90; 

	// This variable determinates if the player will move or not 
	private bool isWalking = false;

	CardboardHead head = null;

	// This is the variable for if VR is enabled
	[Tooltip("Use this if using VR mode on the camera, Set manually")]
	public bool UsingVR = false;

	//This is the variable for the player speed
	[Tooltip("With this speed the player will move.")]
	public float speed;

	[Tooltip("Activate this checkbox if the player shall move when the Cardboard trigger is pulled.")]
	public bool walkWhenTriggered;

	[Tooltip("Activate this checkbox if the player shall move when he looks below the threshold.")]
	public bool walkWhenLookDown;

	[Tooltip("This has to be an angle from 0° to 90°")]
	public double thresholdAngle;

	[Tooltip("Activate this Checkbox if you want to freeze the y-coordiante for the player. " +
		"For example in the case of you have no collider attached to your CardboardMain-GameObject" +
		"and you want to stay in a fixed level.")]
	public bool freezeYPosition; 

	[Tooltip("This is the fixed y-coordinate.")]
	public float yOffset;



	void Start () 
	{
		head = Camera.main.GetComponent<StereoController>().Head;
	}

	void Update ()
	{
		if (UsingVR == true) {
			// Walk when the Cardboard Trigger is used 
			if (walkWhenTriggered && !walkWhenLookDown && !isWalking && Cardboard.SDK.Triggered) {
				isWalking = true;
			} else if (walkWhenTriggered && !walkWhenLookDown && isWalking && Cardboard.SDK.Triggered) {
				isWalking = false;
			}

			// Walk when player looks below the threshold angle 
			if (walkWhenLookDown && !walkWhenTriggered && !isWalking &&
			   head.transform.eulerAngles.x >= thresholdAngle &&
			   head.transform.eulerAngles.x <= RIGHT_ANGLE) {
				isWalking = true;
			} else if (walkWhenLookDown && !walkWhenTriggered && isWalking &&
			        (head.transform.eulerAngles.x <= thresholdAngle ||
			        head.transform.eulerAngles.x >= RIGHT_ANGLE)) {
				isWalking = false;
			}

			// Walk when the Cardboard trigger is used and the player looks down below the threshold angle
			if (walkWhenLookDown && walkWhenTriggered && !isWalking &&
			   head.transform.eulerAngles.x >= thresholdAngle &&
			   Cardboard.SDK.Triggered &&
			   head.transform.eulerAngles.x <= RIGHT_ANGLE) {
				isWalking = true;
			} else if (walkWhenLookDown && walkWhenTriggered && isWalking &&
			        head.transform.eulerAngles.x >= thresholdAngle &&
			        (Cardboard.SDK.Triggered ||
			        head.transform.eulerAngles.x >= RIGHT_ANGLE)) {
				isWalking = false;
			}

			if (isWalking) {
				Vector3 direction = new Vector3 (head.transform.forward.x, 0, head.transform.forward.z).normalized * speed * Time.deltaTime;
				Quaternion rotation = Quaternion.Euler (new Vector3 (0, -transform.rotation.eulerAngles.y, 0));
				transform.Translate (rotation * direction);
			}

			if (freezeYPosition) {
				transform.position = new Vector3 (transform.position.x, yOffset, transform.position.z);
			}
		}
	}
}