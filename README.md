# AudioMaze

*Experience topology through audio*

An audio game for the [Data Sonification & Opportunities of Sound Seminar, winter and summer semester 20/21 @ Hasso Plattner Institute Potsdam](https://hpi.de/studium/im-studium/lehrveranstaltungen/it-systems-engineering-ma/lehrveranstaltung/wise-20-21-3154-data-sonification--opportunities-of-sound.html).

## About

AudioMaze is an audio-only maze game. Players must find the exit of a grid-based labyrinth by following only auditory clues, having no visuals at all. The project aims at showing that A) making a solvable pleasant game of this format is possible and B) to compare and evaluate the different approaches to audio navigation.

## How to play

Before starting you have to enter a scenario. This selects which levels you will play with which settings and is built for the user study. If you just want to play you can enter `1aaa`.

The first level is a tutorial which you play with visuals. Afterwards you will play 4 levels with increasing difficulty.
After each level you can select which visual best describes the labyrinth you just played (the results are stored in a log file). After the last level you play the tutorial and another 4 levels again with different settings.

The settings differ in the way they react to rotation from and towards the goal. The first one only slightly adds a filter if you rotate away from the general direction of the goal, the second one applies a filter if you rotate away from the best path towards the goal.

### Installation

Download the [latest release](https://github.com/Paulpanther/AudioMaze/releases/latest) for your operating system (currently windows only - you can build your own version using Unity and FMOD Studio).
Extract the zip file and execute the executable inside.

### Controls

- `w` / `↑` : move forward
- `s` / `↓` : move backward
- `a` / `←` : turn left
- `d` / `→` : turn right
- `esc` : close the game

### Keys for Debugging and Surveys

- `p` : shows the percentage of progress towards the exit
- `v` : shows the player rotation
- `c` : shows the whole maze
- `m` : toggles movement from rotation-based to absolute-direction-based

## Authors

Created by @LeonBein, @Paulpanther, @ShirleyNekoDev, @efraimdahl

## License
BSD 3 - see LICENSE file
