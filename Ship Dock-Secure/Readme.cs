/***********************************************************
Ship Docking Secure
By SDragon42
ver 1.3.4    2018-02-11
------------------------------------------------------------

Turns blocks On/Off when docking a ship. It is setup to be fully automatic by default, but can be changed to 
partial or manual if desired.
The configuration for the script is set in the "Custom Data" window.


The following commands can be used when running in partial/manual mode:

dock
    Will "dock" the ship if any of the connectors and landing gears are in locking range.
    If the ship can dock, then it will turn off the blocks that a set in the custom data configuration.

undock
    Will "Undock" the ship by turning on the blocks that a set in the custom data configuration.
    It will then unlock all the connectors and landing gears.

dock-toggle
    This dock the ship if it isn't docked, but can be.
    Or it will un-dock the ship if it is docked.


***********************************************************/
