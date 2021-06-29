import com.fasterxml.jackson.annotation.JsonSubTypes
import com.fasterxml.jackson.annotation.JsonTypeInfo

@JsonTypeInfo(
    use = JsonTypeInfo.Id.NAME,
    include = JsonTypeInfo.As.PROPERTY,
    property = "name"
)
@JsonSubTypes(
    JsonSubTypes.Type(value = LevelEvent::class, name = "LevelEvent"),
    JsonSubTypes.Type(value = MovementEvent::class, name = "MovementEvent"),
    JsonSubTypes.Type(value = KeyEvent::class, name = "KeyEvent")
)
abstract class Event(
    open val createdAt: Double,
) {
    enum class Action {
        Started, Progessing, Stopped
    }
}

data class LevelEvent(
    override val createdAt: Double,
    val levelName: String,
) : Event(createdAt)

data class MovementEvent(
    override val createdAt: Double,
    val action: Action,
    val startPos: Pos,
    val endPos: Pos,
) : Event(createdAt)

data class KeyEvent(
    override val createdAt: Double,
    val action: KeyAction,
    val keyCode: String,
) : Event(createdAt) {
    enum class KeyAction {
        KeyDown,
        KeyUp
    }
}

data class Pos(val x: Double, val y: Double)
