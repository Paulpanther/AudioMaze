fun statsForMovement(movementEvents: List<MovementEvent>) {
    class MovementInterval(val startEvent: MovementEvent) {
        lateinit var endEvent: MovementEvent

        val time: Double
            get() = endEvent.createdAt - startEvent.createdAt
    }

    val movementIntervals = movementEvents
        .filter { it.action != Event.Action.Progessing }
        .fold(mutableListOf<MovementInterval>()) { acc, evt ->
            when (evt.action) {
                Event.Action.Started -> {
                    acc.add(MovementInterval(evt))
                }
                else -> {
                    acc.last().endEvent = evt
                }
            }
            acc
        }

    println("Time in motion:")
    val timeInMotion = movementIntervals.sumOf { it.time }
    println("$timeInMotion seconds")

    println("Time standing still:")
    val timeStandingStill = (lastEventTime - firstEventTime) - timeInMotion
    println("$timeStandingStill seconds")
}
