using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    // Change rotation and movement max speed through Rigidbody linear drag and angular drag
    public float rotationAcceleration = 1f;
    public float movementAcceleration = 30f;
    public float maxSpeed = 1.5f;

    public Transform cam;

    public MazeSolver maze => level.mazeSolver;
    public Tilemap map => level.map;
    public Win win => level.win;
    public Transform goal => level.win.transform;

    public float distanceChange;
    public float distancePercentage = 1;

    private float _previousSpeed;
    public float speed;

    private Level level;

    public enum ControlSystem {
        Relative, // rotate and forward
        Absolute // directional movement
    }

    public ControlSystem controlSystem = ControlSystem.Relative;
    public bool walkingAgainstWall = false;

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
        MovementEvent.previousPosition = new Vector2(transform.position.x, transform.position.y);

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

        var hEvt = KeyEvent.getKeyEventForAxis(_horizontal, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.A, KeyCode.D);
        if (hEvt != null) EventLogging.logEvent(hEvt);
        var vEvt = KeyEvent.getKeyEventForAxis(_vertical, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.W, KeyCode.S);
        if (vEvt != null) EventLogging.logEvent(vEvt);

        if (Input.GetKeyDown("m"))
        {
            controlSystem = (controlSystem == ControlSystem.Relative) ? ControlSystem.Absolute : ControlSystem.Relative;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        cam.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    private void FixedUpdate()
    {
        var position = new Vector2(transform.position.x, transform.position.y);

        if (controlSystem == ControlSystem.Absolute)
        {
            if (_horizontal != 0 || _vertical != 0)
            {
                _body.AddForce((new Vector2(_horizontal, 0) + new Vector2(0, _vertical)).normalized * movementAcceleration);
            }
            if (_body.velocity.magnitude > 0.5)
            {
                var velocityNormalized = _body.velocity.normalized;
                _body.SetRotation(Mathf.Atan2(velocityNormalized.y, velocityNormalized.x) * Mathf.Rad2Deg - 90);
            }
            walkingAgainstWall = (_horizontal != 0 || _vertical != 0) && _body.velocity.magnitude < 0.01;
        }
        else
        {
            //_body.AddTorque(-_horizontal * rotationAcceleration);
            if (Mathf.Abs(_horizontal) > 0 && DateTime.Now >= _nextAllowedRotationTime)
            {
                transform.Rotate(0, 0, -30 * _horizontal);
                _nextAllowedRotationTime = DateTime.Now.AddMilliseconds(200);
                _rotationClicker.RotationChanged((int)Mathf.Round(transform.rotation.eulerAngles.z)); ;
            }

            _body.AddRelativeForce(new Vector2(0, _vertical * movementAcceleration));

            walkingAgainstWall = _vertical != 0 && _body.velocity.magnitude < 0.01;

        }
        _previousSpeed = speed;
        speed = _body.velocity.magnitude;

        if (speed > 0)
        {
            if (_previousSpeed == 0)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Started, position, position));
            }
            else if ((MovementEvent.previousPosition - position).sqrMagnitude > 0.01)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Progessing, MovementEvent.previousPosition, position));
                MovementEvent.previousPosition = position;
            }
        }
        else
        {
            if (_previousSpeed > 0)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Stopped, position, position));
            }
        }

        _walkingSound.SetWalking(speed / maxSpeed);
    }

    public void RegisterLevel(Level level)
    {
        this.level = level;
        level.player = this;
        transform.position = Vector3.zero;
    }
}
