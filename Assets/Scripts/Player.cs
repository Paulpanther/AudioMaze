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
    public bool useRelativeGoalOrientation = true;

    private Level level;

    public enum ControlSystem {
        Relative, // rotate and forward
        Absolute // directional movement
    }

    public ControlSystem controlSystem = ControlSystem.Relative;
    public bool walkingAgainstWall = false;
    public VisualizationToggle visualization;

    private Rigidbody2D _body;
    private WalkingSound _walkingSound;
    private RotationClickerSound _rotationClicker;
    private float _horizontal, _vertical;

    private float? _lastDistance;

    private DateTime _nextAllowedRotationTime = DateTime.MinValue;

    private float _startGoalDistance;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _walkingSound = GetComponent<WalkingSound>();
        _rotationClicker = GetComponentInChildren<RotationClickerSound>();
    }

    private void LevelChange()
    {
        transform.rotation = Quaternion.Euler(Vector3.up);
        _startGoalDistance = maze.GetAccurateDistanceFrom(goal, transform.position);
        MovementEvent.previousPosition = new Vector2(transform.position.x, transform.position.y);
        EventLogging.logEvent(new ControlSystemChangedEvent(controlSystem));
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
            controlSystem = (controlSystem == ControlSystem.Relative)
                ? ControlSystem.Absolute
                : ControlSystem.Relative;
        
        	EventLogging.logEvent(new ControlSystemChangedEvent(controlSystem));
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
            
                if(DateTime.Now >= _nextAllowedRotationTime) {
                    // use _nextAllowedRotationTime to reduce log spam
                    _nextAllowedRotationTime = DateTime.Now.AddMilliseconds(100);
                    int degrees = (int)Mathf.Round(transform.rotation.eulerAngles.z);
                    if(RotationEvent.previousOrientation != degrees) {
                        EventLogging.logEvent(new RotationEvent(degrees));
                    }
                }
            }
            walkingAgainstWall = (_horizontal != 0 || _vertical != 0) && _body.velocity.magnitude < 0.01;
        }
        else
        {
            if (Mathf.Abs(_horizontal) > 0 && DateTime.Now >= _nextAllowedRotationTime)
            {
                transform.Rotate(0, 0, -90 * _horizontal);
                _nextAllowedRotationTime = DateTime.Now.AddMilliseconds(200);
                int degrees = (int)Mathf.Round(transform.rotation.eulerAngles.z);
                EventLogging.logEvent(new RotationEvent(degrees));
                _rotationClicker.RotationChanged(degrees);
            }

            _body.AddRelativeForce(new Vector2(0, _vertical * movementAcceleration));

            walkingAgainstWall = _vertical != 0 && _body.velocity.magnitude < 0.01;

        }
        _previousSpeed = speed;
        speed = _body.velocity.magnitude;
        var tilePos = (Vector2Int) this.map.WorldToCell(transform.position);

        if (speed > 0)
        {
            if (_previousSpeed == 0)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Started, tilePos, position, position));
                MovementEvent.previousPosition = position;
            }
            else if ((MovementEvent.previousPosition - position).sqrMagnitude > 0.01)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Progressing, tilePos, MovementEvent.previousPosition, position));
                MovementEvent.previousPosition = position;
            }
        }
        else
        {
            if (_previousSpeed > 0)
            {
                EventLogging.logEvent(new MovementEvent(AbstractEvent.Action.Stopped, tilePos, position, position));
                MovementEvent.previousPosition = position;
            }
        }
        var distance = maze.GetAccurateDistanceFrom(goal, transform.position);
        var delta = (distance - _lastDistance) ?? 0;
        distanceChange = -delta;
        distancePercentage = distance / _startGoalDistance;

        _lastDistance = distance;
        _walkingSound.SetWalking(speed / maxSpeed);
    }

    public float RelativeGoalOrientation {
        get {
            var dir = maze.GetBestDirectionFor(goal, transform.position);
            var angle = Vector3.Angle(transform.up, dir);
            // Debug.Log(angle);
            return angle / 180;
        }
    }

    public float AbsoluteGoalOrientation {
        get {
            var angle = Vector3.Angle(transform.up, (goal.position - transform.position).normalized);
            return angle / 180;
        }
    }

    public void SetRelativeGoalOrientation() {
        useRelativeGoalOrientation = true;
        EventLogging.logEvent(new GoalOrientationChangedEvent(useRelativeGoalOrientation));
    }
    public void SetAbsoluteGoalOrientation() {
        useRelativeGoalOrientation = false;
        EventLogging.logEvent(new GoalOrientationChangedEvent(useRelativeGoalOrientation));
    }

    public float CurrentGoalOrientation {
        get {
            if (useRelativeGoalOrientation) {
                return RelativeGoalOrientation;
            }
            else {
                return AbsoluteGoalOrientation;
            }
        }
    }

    public void RegisterLevel(Level level)
    {
        this.level = level;
        level.player = this;
        transform.position = Vector3.zero;
        LevelChange();
    }
}
