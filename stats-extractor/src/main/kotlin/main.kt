import com.fasterxml.jackson.core.json.JsonReadFeature
import com.fasterxml.jackson.databind.DeserializationFeature
import com.fasterxml.jackson.module.kotlin.jsonMapper
import com.fasterxml.jackson.module.kotlin.kotlinModule
import com.fasterxml.jackson.module.kotlin.readValue
import java.io.File
import kotlin.reflect.KClass

val mapper = jsonMapper {
    addModule(kotlinModule())
    enable(JsonReadFeature.ALLOW_TRAILING_COMMA)
    enable(JsonReadFeature.ALLOW_UNQUOTED_FIELD_NAMES)
    enable(DeserializationFeature.FAIL_ON_INVALID_SUBTYPE)
    disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES)
}

var firstEventTime: Double = 0.0
var lastEventTime: Double = 0.0

fun main(args: Array<String>) {
    val file = File(args[0])
    println("Reading file $file")
    val allEvents = mapper.readValue<List<Event>>(file)
    firstEventTime = allEvents.first().createdAt
    lastEventTime = allEvents.last().createdAt

    println("Event log contains events from $firstEventTime to $lastEventTime")

    val eventsByType = allEvents.groupBy { it::class }
    eventsByType.keys.forEach { eventType ->
        println("${eventType.simpleName}s:")
        eventsByType[eventType]!!.forEach { evt ->
            println(evt)
        }
    }

    statsForMovement(eventsByType.getTyped<MovementEvent>())
}

@Suppress("UNCHECKED_CAST")
inline fun <reified S : Event> Map<KClass<out Event>, List<Event>>.getTyped(): List<S> =
    this[S::class]!! as List<S>
