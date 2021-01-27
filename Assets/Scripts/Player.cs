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

    public bool useWasd = false;

    private Rigidbody2D _body;
    private WalkingSound _walkingSound;
    private float _horizontal, _vertical;
    
    private float? _lastDistance;

    private async void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _walkingSound = GetComponent<WalkingSound>();
        
        while (true)
        {
            var distance = maze.GetAccurateDistanceFrom(goal, transform.position);
            var delta = (distance - _lastDistance) ?? 0;
            distanceChange = -delta;

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
            _body.AddTorque(-_horizontal * rotationAcceleration);
            _body.AddRelativeForce(new Vector2(0, _vertical * movementAcceleration));
        }

        _walkingSound.SetWalking(_body.velocity.magnitude / maxSpeed);
        //RayCastSonar();
    }

    private void RayCastSonar()
    {
        float currentAngle = -transform.rotation.eulerAngles.z;

        int maxDistance = 10;
        int numRays = 24;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y); // +  _body.velocity;
        // TODO velocity might need to be removed, was added to make debug visualization smoother, must be tested with actual sounds

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