/*
R e a d m e
-----------

Flight Cycle

Setup
- All PBs are run.  Pod PD will run the others (if needed)
- Identifies the rocket setup and builds the launch sequence code

Launch
- All PB are in standby except the Pod stage
- Pod PB controls the entire launch

Side-Boosters - 100%



Rocket types

Pod only (SSTO)
Pod + 1 booster
Pod + 2nd stg + 1 booster
Pod + 2nd stg + 3 booster


Features / Operation
Pod only (SSTO)
H2 POD only
Used to set the pod empty (or with a very light load) to space.

Pod + 1 booster
Booster lifts to altitude where the pod can then take over, or 35Km(*) altitude
Booster lands at LZ 1

Pod + 2nd stg + 1 booster
Booster lifts to altitude where the 2nd stg can then take over.
Booster lands at LZ 1
2nd stg will lift the rest of the way to space, then release, stop, and return to planet.
2nd stg lands at LZ 2

Pod + 2nd stg + 3 booster
boosters run as:
    core booster - power set to 25%(**)
    side boosters - run at full power
Side boosters will separate as 15-20Km(***)
Side Booster lands at LZ 1a & LZ 1b
Core Booster lifts to altitude where the 2nd stg can then take over.
Core Booster lands at LZ 1
2nd stg will lift the rest of the way to space, then release, stop, and return to planet.
2nd stg lands at LZ 2





  (*) Altitude ceiling is a guess and will need to be determined.
 (**) Core booster power settings need to be determined. Idea is the side boosters will do most of the work.
(***) Side booster staging altitude really will depend on payload mass and performance of the Core booster.


 */
