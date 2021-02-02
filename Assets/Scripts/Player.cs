using System;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Change rotation and movement max speed through Rigidbody linear drag and angular drag
    public float rotationAcceleration = 1f;
    public float movementAcceleration = 30f;
    public float maxSpeed = 1.5f;

    public Transform cam;

    public MazeSolver maze;
    public Transform goal;

    public float distanceChange;
    public float distancePercentage = 1;
    public float speed;

    public bool useWasd = false;

    private Rigidbody2D _body;
    private WalkingSound _walkingSound;
    private RotationClicker _rotationClicker;
    private float _horizontal, _vertical;
    
    private float? _lastDistance;

    private DateTime _nextAllowedRotationTime = DateTime.MinValue;

    private float _startGoalDistance;

    private async void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _walkingSound = GetComponent<WalkingSound>();
        _rotationClicker = GetComponentInChildren<RotationClicker>();
        _startGoalDistance = maze.GetAccurateDistanceFrom(goal, transform.position);
        
        while (true)
        {
            var distance = maze.GetAccurateDistanceFrom(goal, transform.position);
            var delta = (distance - _lastDistance) ?? 0;
            distanceChange = -delta;
            distancePercentage = distance / _startGoalDistance;

            _lastDistance = distance;
            await Task.Delay(100);
        }
    }

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        cam.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    private void FixedUpdate()
    {
        if (useWasd)
        {
            if (_horizontal != 0 || _vertical != 0)
            {
                _body.AddForce((new Vector2(_horizontal, 0) + new Vector2(0, _vertical)).normalized * movementAcceleration);
            }
            if (_body.velocity.magnitude > Mathf.Epsilon)
            {
                var velocityNormalized = _body.velocity.normalized;
                _body.SetRotation(Mathf.Atan2(velocityNormalized.y, velocityNormalized.x) * Mathf.Rad2Deg - 90);
            }
        }
        else
        {
            //_body.AddTorque(-_horizontal * rotationAcceleration);
            if (Mathf.Abs(_horizontal) > 0 && DateTime.Now >= _nextAllowedRotationTime) {
                transform.Rotate(0, 0, - 30 * _horizontal);
                _nextAllowedRotationTime = DateTime.Now.AddMilliseconds(200);
                _rotationClicker.RotationChanged((int) Mathf.Round(transform.rotation.eulerAngles.z));;
            }

            _body.AddRelativeForce(new Vector2(0, _vertical * movementAcceleration));
        }
        speed = _body.velocity.magnitude;
        _walkingSound.SetWalking(speed / maxSpeed);
    }
}