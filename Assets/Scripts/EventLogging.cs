using System.Collections;
using System.Collections.Generic;
using System.IO.TextWriter;
using System.IO.StreamWriter;
using UnityEngine;

public class EventLogging : MonoBehaviour {
    public bool log2Console = false;
    public string logFile = "log.json";
    public bool verboseLogging = true;
    public HashSet<string> consoleEnabledEvents = new HashSet<string> { "KeyEvent", "MovementEvent" };

    private StreamWriter _logOut;

    public static EventLogging INSTANCE = null;

    private void Start()
    {
        INSTANCE = this;
        _logOut = TextWriter.Synchronized(new StreamWriter(logFile));
        _logOut.writeLine("{");
        _logOut.writeLine("level:\"TODO\",");
        _logOut.writeLine("messages:[");
    }

    private void OnDestroy()
    {
        _logOut.writeLine("]");
        _logOut.writeLine("}");
        _logOut.close();
    }

    public void logToConsole(AbstractEvent evt, bool verbose)
    {
        if (verbose || consoleEnabledEvents.Contains(evt.getName())){
            Debug.Log(evt.message());
        }
    }

    public void logToFile(AbstractEvent evt)
    {
        evt.writeAsJson(_logOut);
        _logOut.writeLine(",");
    }

    public void logEvent(AbstractEvent evt)
    {
        if(log2Console) {
            logToConsole(evt, verboseLogging);
        }
    }

    public static void logEvent(AbstractEvent evt)
    {
        INSTANCE.logEvent(evt);
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

    protected abstract void _writeJson(TextWriter out);

    public virtual void writeAsJson(TextWriter out)
    {
        out.writeLine("{");
        out.writeLine("name:{0},", name);
        out.writeLine("createdAt:{0},", creationTime);
        _json(out);
        out.write("}");
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

    protected override void _writeJson(TextWriter out)
    {
        out.writeLine("action:{0},", action);
        out.writeLine("keyCode:{0},", keyCode);
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

    protected override void _writeJson(TextWriter out)
    {
        out.writeLine("action:{0},", action);
        out.writeLine("startPos:{0},", startPos);
        out.writeLine("endPos:{0},", endPos);
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