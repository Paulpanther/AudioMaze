using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class EventLogging
{
    public static HashSet<string> enabledEvents = new HashSet<string> { "KeyEvent", "MovementEvent" };

    public static void logEvent(AbstractEvent evt)
    {
        if (enabledEvents.Contains(evt.getName())){
            Debug.Log(evt.message());
        }
    }
}

public abstract class AbstractEvent
{
    public enum Action
    {
        Started, Progessing, Stopped
    }

    protected float creationTime = Time.time;
    protected string name;

    public AbstractEvent(string name)
    {
        this.name = name;
    }

    public void setCreationTime(float time)
    {
        this.creationTime = time;
    }

    public string getName()
    {
        return name;
    }

    protected abstract string _message();

    public virtual string message()
    {
        return creationTimeAsString() + " [" + name + "]: " + _message();
    }

    public string creationTimeAsString()
    {
        int fullSeconds = (int)creationTime;
        int ms = (int)((creationTime - fullSeconds) * 1000);
        int seconds = (int)(fullSeconds % 60);
        int minutes = (int)(fullSeconds / 60);
        return minutes.ToString().PadLeft(3, '0') + ":"
        + seconds.ToString().PadLeft(2, '0') + "."
        + ms.ToString().PadLeft(3, '0');
    }
}

class KeyEvent : AbstractEvent
{
    public enum KeyAction
    {
        KeyDown,
        KeyUp
    }

    public static KeyEvent getKeyEventForAxis(float value, params KeyCode[] codes)
    {
        if (value != 0)
        {
            foreach (KeyCode code in codes)
            {
                if (Input.GetKeyDown(code))
                {
                    return new KeyEvent(KeyAction.KeyDown, code);
                }
            }
        }
        else
        {
            foreach (KeyCode code in codes)
            {
                if (Input.GetKeyUp(code))
                {
                    return new KeyEvent(KeyAction.KeyUp, code);
                }
            }
        }
        return null;
    }

    protected KeyAction action;
    protected KeyCode keyCode;

    public KeyEvent(KeyAction action, KeyCode keyCode) : base("KeyEvent")
    {
        this.action = action;
        this.keyCode = keyCode;
    }

    protected override string _message()
    {
        return "key " + (action == KeyAction.KeyDown ? "pressed" : "released") + ": " + keyCode;
    }
}

class MovementEvent : AbstractEvent
{
    public static Vector2 previousPosition;

    protected Action action;
    protected Vector2 startPos, endPos;

    public MovementEvent(Action action, Vector2 startPos, Vector2 endPos) : base("MovementEvent")
    {
        this.action = action;
        this.startPos = startPos;
        this.endPos = endPos;
    }

    protected override string _message()
    {
        if (action == Action.Progessing)
        {
            return "movement " + action + " from " + startPos + " to " + endPos;
        }
        else
        {
            return "movement " + action + " at " + startPos;
        }
    }
}