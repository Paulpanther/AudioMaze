using System;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Change rotation and movement max speed through Rigidbody linear drag and angular drag
	public float rotationAcceleration = 1f;
	public float movementAcceleration = 30f;
	public float moveLimiter = 0.7f;

	public Transform cam;

	private Rigidbody2D _body;
	private float _horizontal, _vertical;

	private void Start()
	{
		_body = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		_horizontal = Input.GetAxisRaw("Horizontal");
		_vertical = Input.GetAxisRaw("Vertical");

		cam.position.Set(transform.position.x, transform.position.y, -1);
	}

	private void FixedUpdate()
	{
		_body.AddTorque(-_horizontal * rotationAcceleration);
		_body.AddRelativeForce(new Vector2(0, _vertical * movementAcceleration));

		RayCastSonar();
	}

	private void RayCastSonar()
	{
		float currentAngle = transform.rotation.eulerAngles.z;

		int maxDistance = 10;
		int numRays = 24;

		Vector2
			pos = new Vector2(transform.position.x, transform.position.y) +
			      _body.velocity; //TODO velocity might need to be removed, was added to make debug visualization smoother, must be tested with actual sounds

		for (int i = 0; i < numRays; i++)
		{
			float angle = currentAngle + 360f / (float) numRays * ((float) i + 0.5f);
			float radians = angle * Mathf.Deg2Rad;
			Vector2 direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));

			RaycastHit2D rayHit = Physics2D.Raycast(pos, direction, maxDistance, 1 << LayerMask.NameToLayer("Walls"));
			if (rayHit.collider)
			{
				Debug.DrawRay(pos, rayHit.point - pos, Color.red);
			}
		}
	}

}