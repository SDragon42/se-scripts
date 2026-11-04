# Drill Control

## Features

- Control a Bore hole of 1 or more pistons and 1 or more drills
  - Control starts at a root piston (tagged), and will only find/use pistons and drills that are attached to it.
- Set a maximum bore distance, starting from the head of the root piston. (maybe the base?)
  - Once maximum distance is reached, retract all pistons.
- Monitors the inventory of tagged containers (if any), and also all of the drills.
  - Pause operations once a configured threshold is reached. 
    1. ***All*** tagged cargo go above the threshold.
    2. ***Any*** attached drill goes above the threshold.
  - Resume operation once the respective inventories from above drop below a configured threshold.
- If the maximum possible extension is reached before the configured target distance:
  1. Detach the tagged sub-grid (rotor, hinge, or piston).
  2. Retract the last N pistons (configured), or all pistons if none is configured.
  
