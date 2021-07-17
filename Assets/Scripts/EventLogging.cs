using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System;
using UnityEngine;
using SimpleJson;

public class EventLogging : MonoBehaviour {
    public bool log2Console = false;
    public string logFile = "log.json";
    public bool verboseLogging = true;
    public HashSet<string> consoleEnabledEvents = new HashSet<string> { "KeyEvent", "MovementEvent" };

    private SimpleJsonWriter _jsonOut;
    
    public static EventLogging INSTANCE = null;

    private void Awake()
    {
        INSTANCE = this;
        _jsonOut = SimpleJsonFileWriter.openJsonWriter(/*Application.persistentDataPath +*/ logFile).WriteArrayScope();
    }

    private void OnDestroy()
    {
        _jsonOut.Dispose();
    }

    public void logToConsole(AbstractEvent evt, bool verbose)
    {
        if (verbose || consoleEnabledEvents.Contains(evt.getName())){
            Debug.Log(evt.message());
        }
    }

    public void logToFile(AbstractEvent evt)
    {
        evt.writeAsJson(_jsonOut);
    }

    public void _logEvent(AbstractEvent evt)
    {
        if(log2Console) {
            logToConsole(evt, verboseLogging);
        }
        logToFile(evt);
    }

    public static void logEvent(AbstractEvent evt)
    {
        INSTANCE._logEvent(evt);
    }
}

namespace SimpleJson {
    public interface SimpleJsonWriter : IDisposable {

        void Flush();

        SimpleJsonWriter WriteArrayScope(string key = null);
        SimpleJsonWriter WriteObjectScope(string key = null);

        void WriteKeyNull(string key);
        void WriteKeyValue(string key, float value);
        void WriteKeyValue(string key, int value);
        void WriteKeyValue(string key, string value);
    }

    public class SimpleJsonScope : SimpleJsonWriter {
        private bool _hadPreviousEntry = false;

        protected SimpleJsonFileWriter _writer;
        private SimpleJsonScope _parentScope;

        public SimpleJsonScope(SimpleJsonFileWriter writer, SimpleJsonScope parentScope = null) {
            _writer = writer;
            _parentScope = parentScope;
        }
        public virtual void Dispose() {
            if(_parentScope == null) {
                _writer.Dispose();
            } else {
                Flush();
            }
        }

        public void Flush() {
            _writer.Flush();
        }

        protected void WriteKey(string key = null) {
            if(_hadPreviousEntry) {
                _writer.WriteEntrySeparator();
            }
            if(key != null) {
                _writer.WriteStringLiteral(key);
                _writer.WriteKeyValueSeparator();
            }
            _hadPreviousEntry = true;
        }
        
        public SimpleJsonWriter WriteArrayScope(string key = null) {
            WriteKey(key);
            _writer.WriteOpenArrayLiteral();
            return new SimpleJsonArrayScope(_writer, this);
        }

        public SimpleJsonWriter WriteObjectScope(string key = null) {
            WriteKey(key);
            _writer.WriteOpenObjectLiteral();
            return new SimpleJsonObjectScope(_writer, this);
        }

        public void WriteKeyNull(string key) {
            WriteKey(key);
            _writer.WriteNullLiteral();
        }
        public void WriteKeyValue(string key, float value) {
            WriteKey(key);
            _writer.WriteFloatLiteral(value);
        }
        public void WriteKeyValue(string key, int value) {
            WriteKey(key);
            _writer.WriteIntLiteral(value);
        }
        public void WriteKeyValue(string key, string value) {
            WriteKey(key);
            _writer.WriteStringLiteral(value);
        }

        public class SimpleJsonArrayScope : SimpleJsonScope {
            public SimpleJsonArrayScope(SimpleJsonFileWriter writer, SimpleJsonScope parentScope) : base(writer, parentScope) {}
            public override void Dispose() {
                _writer.WriteCloseArrayLiteral();
                base.Dispose();
            }
        }

        public class SimpleJsonObjectScope : SimpleJsonScope {
            public SimpleJsonObjectScope(SimpleJsonFileWriter writer, SimpleJsonScope parentScope) : base(writer, parentScope) {}
            public override void Dispose() {
                _writer.WriteCloseObjectLiteral();
                base.Dispose();
            }
        }
    }

    public class SimpleJsonFileWriter : IDisposable {
        private TextWriter _out;

        private class SimpleStreamWriter : StreamWriter {
            public SimpleStreamWriter(string path) : base(path) {}
            private readonly IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            public override IFormatProvider FormatProvider {
                get {
                    return this.formatProvider;
                }
            }
        }

        public class SimpleAutoclosingJsonScope : SimpleJsonScope {
            public SimpleAutoclosingJsonScope(SimpleJsonFileWriter writer) : base(writer, null) {}
            public new SimpleJsonWriter WriteArrayScope() {
                _writer.WriteOpenArrayLiteral();
                return new SimpleJsonArrayScope(_writer, null);
            }
            public new SimpleJsonWriter WriteObjectScope() {
                _writer.WriteOpenObjectLiteral();
                return new SimpleJsonObjectScope(_writer, null);
            }
        }

        private SimpleJsonFileWriter(string filePath) {
            _logFile = filePath;
            Debug.Log("Starting log into file \"" + _logFile + "\"");
            _out = TextWriter.Synchronized(new SimpleStreamWriter(filePath));
        }

        private readonly string _logFile;

        public static SimpleAutoclosingJsonScope openJsonWriter(string filePath) {
            SimpleJsonFileWriter _this = new SimpleJsonFileWriter(filePath);
            return new SimpleAutoclosingJsonScope(_this);
        }

        public void Dispose() {
            _out.Close();
            Debug.Log("Stopping log into file \"" + _logFile + "\"");
        }

        public void Flush() {
            _out.Flush();
        }

        public void WriteEntrySeparator() {
            _out.WriteLine(",");
        }
        public void WriteKeyValueSeparator() {
            _out.Write(": ");
        }

        public void WriteNullLiteral() {
            _out.Write("null");
        }
        public void WriteFloatLiteral(float value) {
            _out.Write(value);
        }
        public void WriteIntLiteral(int value) {
            _out.Write(value);
        }
        public void WriteStringLiteral(string value) {
            _out.Write("\"{0}\"", value);
        }

        public void WriteOpenArrayLiteral() {
            _out.WriteLine("[");
        }
        public void WriteCloseArrayLiteral() {
            _out.Write("\n]");
        }
        public void WriteOpenObjectLiteral() {
            _out.WriteLine("{");
        }
        public void WriteCloseObjectLiteral() {
            _out.Write("\n}");
        }
    }
}

public abstract class AbstractEvent
{
    public enum Action
    {
        Started, Progressing, Stopped
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

    protected abstract void _writeJson(SimpleJsonWriter evtScope);

    public virtual void writeAsJson(SimpleJsonWriter output)
    {
        using(SimpleJsonWriter evtScope = output.WriteObjectScope()) {
            evtScope.WriteKeyValue("name", name);
            evtScope.WriteKeyValue("createdAt", creationTime);
            _writeJson(evtScope);
        }
        output.Flush();
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

class LevelEvent : AbstractEvent
{
    protected string levelName;

    public LevelEvent(string levelName) : base("LevelEvent")
    {
        this.levelName = levelName;
    }

    protected override void _writeJson(SimpleJsonWriter evtScope)
    {
        evtScope.WriteKeyValue("levelName", levelName);
    }

    protected override string _message()
    {
        return "level changed to \"" + levelName + "\"";
    }
}

abstract class SoundEvent : AbstractEvent
{
    protected string soundName;

    protected SoundEvent(string soundName) : base("SoundEvent")
    {
        this.soundName = soundName;
    }

    protected override void _writeJson(SimpleJsonWriter evtScope)
    {
        evtScope.WriteKeyValue("soundName", soundName);
    }

    class Oneshot : SoundEvent
    {
        public Oneshot(string soundName) : base(soundName) {}
        
        protected override void _writeJson(SimpleJsonWriter evtScope)
        {
            evtScope.WriteKeyValue("type", "ONESHOT");
            base._writeJson(evtScope);
        }

        protected override string _message()
        {
            return "audio oneshot \"" + soundName + "\"";
        }
    }

    class Loop : SoundEvent
    {
        protected Action action;

        public Loop(string soundName, Action action) : base(soundName)
        {
            this.action = action;
        }
        
        protected override void _writeJson(SimpleJsonWriter evtScope)
        {
            evtScope.WriteKeyValue("type", "LOOP");
            evtScope.WriteKeyValue("action", action.ToString());
            base._writeJson(evtScope);
        }

        protected override string _message()
        {
            if(action == Action.Started) {
                return "audio loop \"" + soundName + "\" started";
            } else {
                return "audio loop \"" + soundName + "\" stopped";
            }
        }
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

    protected override void _writeJson(SimpleJsonWriter evtScope)
    {
        evtScope.WriteKeyValue("action", action.ToString());
        evtScope.WriteKeyValue("keyCode", keyCode.ToString());
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
    protected Vector2Int tilePos;

    public MovementEvent(Action action, Vector2Int tilePos, Vector2 startPos, Vector2 endPos) : base("MovementEvent")
    {
        this.action = action;
        this.tilePos = tilePos;
        this.startPos = startPos;
        this.endPos = endPos;
    }

    protected override void _writeJson(SimpleJsonWriter evtScope)
    {
        evtScope.WriteKeyValue("action", action.ToString());
        using(SimpleJsonWriter tilePosScope = evtScope.WriteObjectScope("tilePos")) {
            tilePosScope.WriteKeyValue("x", tilePos.x);
            tilePosScope.WriteKeyValue("y", tilePos.y);
        }
        using(SimpleJsonWriter startPosScope = evtScope.WriteObjectScope("startPos")) {
            startPosScope.WriteKeyValue("x", startPos.x);
            startPosScope.WriteKeyValue("y", startPos.y);
        }
        using(SimpleJsonWriter endPosScope = evtScope.WriteObjectScope("endPos")) {
            endPosScope.WriteKeyValue("x", endPos.x);
            endPosScope.WriteKeyValue("y", endPos.y);
        }
    }

    protected override string _message()
    {
        if (action == Action.Progressing)
        {
            return "movement " + action + " from " + startPos + " to " + endPos + " tp " + tilePos;
        }
        else
        {
            return "movement " + action + " at " + startPos + " tp " + tilePos;
        }
    }
}
