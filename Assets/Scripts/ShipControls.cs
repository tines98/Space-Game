using UnityEngine;

public class ShipControls : MonoBehaviour
{
	public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
	private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
	private float forwardAccel = 2.5f, strafeAccel = 2f, hoverAccel = 2;

	public float lookRateSpeedX=45f, lookRateSpeedY = 90f;
	public float activeLookRateSpeedX, activeLookRateSpeedY;
	Rigidbody rb;

	private float rollInput;
	public float rollSpeed = 90f, rollAccel = 3.5f;


	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void FixedUpdate()
	{
		rollInput = Mathf.Lerp(rollInput, Input.GetAxis("Roll"), rollAccel * Time.deltaTime);
		//float xRot = -Input.GetAxis("Mouse Y") * activeLookRateSpeedY;
		float xRot = -Input.GetAxis("Mouse Y") * activeLookRateSpeedY;
		float yRot = Input.GetAxis("Mouse X") * lookRateSpeedX;

		//transform.Rotate(-mouseDistance.y * activeLookRateSpeedY * Time.deltaTime, mouseDistance.x * lookRateSpeedX * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);
		Vector3 rot = new Vector3(xRot, yRot, rollInput * rollSpeed);
		rb.rotation *= Quaternion.Euler(rot);
		//transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, 0, rollInput * rollSpeed * Time.deltaTime, Space.Self);
		if (Input.GetAxis("Roll") == 0
			 && rb.rotation.eulerAngles.z != 0) {
			rb.rotation = Quaternion.Lerp(rb.rotation,Quaternion.Euler(rb.rotation.eulerAngles.x,rb.rotation.eulerAngles.y,0),Time.deltaTime);
		}
		DoMovement();
	}

	void DoMovement() {
		
		activeForwardSpeed = Mathf.Lerp(
			activeForwardSpeed,
			(Input.GetAxis("Vertical")+1) * forwardSpeed,
			forwardAccel * Time.deltaTime
		);
		activeLookRateSpeedY = lookRateSpeedY+(-Input.GetAxis("Vertical")*0.5f);

		/*
		activeForwardSpeed = Mathf.Lerp(
			activeForwardSpeed,
			Input.GetAxis("Vertical") * forwardSpeed,
			forwardAccel * Time.deltaTime
		);
		*/
		activeStrafeSpeed = Mathf.Lerp(
			activeStrafeSpeed,
			Input.GetAxis("Horizontal") * strafeSpeed,
			strafeAccel * Time.deltaTime
		);
		rb.rotation = Quaternion.Lerp(rb.rotation,Quaternion.Euler(rb.rotation.eulerAngles.x,rb.rotation.eulerAngles.y,rb.rotation.eulerAngles.z +(30f * -Input.GetAxis("Horizontal"))),Time.deltaTime*2f);
		activeHoverSpeed = Mathf.Lerp(
			activeHoverSpeed,
			Input.GetAxis("Hover") * hoverSpeed,
			hoverAccel * Time.deltaTime
		);
		Vector3 vel = (transform.forward * activeForwardSpeed) + (((transform.right * activeStrafeSpeed) + (transform.up * activeHoverSpeed)));
		rb.velocity = vel;
		//transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
		//transform.position += ((transform.right * activeStrafeSpeed) + (transform.up * activeHoverSpeed)) * Time.deltaTime;
	}
}
