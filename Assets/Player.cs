using System;
using UnityEngine;

public class Player : MonoBehaviour {

	public float runSpeed = 20f;
	public float moveLimiter = 0.7f;

	private Rigidbody2D _body;
	private float _horizontal, _vertical;

	private void Start() {
		_body = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		_horizontal = Input.GetAxisRaw("Horizontal");
		_vertical = Input.GetAxisRaw("Vertical");
	}

	private void FixedUpdate() {
		if (Mathf.Abs(_horizontal + _vertical) > 1.1f) {
			_horizontal *= moveLimiter;
			_vertical *= moveLimiter;
		}

		_body.velocity = new Vector2(_horizontal * runSpeed, _vertical * runSpeed);
	}
}
