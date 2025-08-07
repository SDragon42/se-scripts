using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    public partial class Program : MyGridProgram {
        // Set to the name of your LCD / Text Panel displays and their font size.
        const string LCD_LapInfo_NAME = "LCD - Lap Info";
        const float LCD_LapInfo_FONTSIZE = 3.6f;

        const string LCD_Speed_Name = "LCD - Speed";
        const float LCD_Speed_FONTSIZE = 6.2f;

        const string LCD_LapLog_NAME = "LCD - Lap Log";
        const float LCD_LapLog_FONTSIZE = 2.6f;

        const string LCD_ArtificialHorizon_NAME = "LCD - Artificial Horizon";
        const float LCD_ArtificialHorizon_FONTSIZE = 0.3f;


        // Set the total number of laps to track.
        const int TOTAL_NUM_LAPS = 5;

        // Artificial Horizon settings
        const int updatesPerSecond = 10;

        bool enableVelocityVector = false; //specifies if the velocity vector indicator is drawn with the artificial horizon
        bool enableOrientationIndicator = true; //specifies if the orientation axes are drawn with the artificial horizon
        bool enableSpeedDisplay = true; //specifies if the craft speed is displayed below the artificial horizon
        bool enableHeadingDisplay = false; //specifies if the craft heading is displayed below the artificial horizon
        bool showSpeedHeadingAbove = true; //specified if the speed/heading should be displayed above the artificial horizon

        const double maximumVelocity = 350; //supports up to 999 m/s max speed




        /*
        The GPS coordinates are from the Space Engineers GPS system. The one addition is the number at the end of
        the line (45.0).  This is the radius out from the GPS point that a ship must pass through. In this case, it is a
        sphere of 90 meters across.
        Multiple points can be defined for "check points" that must be crossed. Each point must be seperated with
        a "\n". The first point must be the start/finish point.
        */
        const string CheckpontText = "GPS:Finish Line:42255.29:2960.54:-43144.8:45.0";

        //


        /*#################################################################################
        ##                                                   ** DO NOT EDIT BELOW THIS LINE **
        #################################################################################*/

        const string CMD_RESET = "reset";

        /**************************************************************************************************************************************/
        void Main(string argument) {
            Init(argument.ToLower());

            timeCurrentCycle += Runtime.TimeSinceLastRun.TotalSeconds;
            timeSymbol += Runtime.TimeSinceLastRun.TotalSeconds;
            Echo("SRE Race Program is online... " + RunningSymbol());

            var currPos = _sc.GetPosition();
            var nextCheckpoint = _checkPoints[_nextCheckpointIdx];

            if (_lapStartTime.HasValue)
                _lapTime = DateTime.Now.Subtract(_lapStartTime.Value);

            // Calc
            var range2cp = (nextCheckpoint.Position - currPos).Length();

            var inRange = (range2cp < nextCheckpoint.RangeRadius);
            if (inRange && !_finishedRace) {
                if (!_passedCP && range2cp > _prevDist2Checkpoint) {
                    _passedCP = true;
                    if (_nextCheckpointIdx == 0) {
                        if (_lapNumber > 0)
                            _logLapTime = true;
                        _lapStartTime = DateTime.Now;
                        _lapNumber++;
                        if (_lapNumber > TOTAL_NUM_LAPS)
                            _finishedRace = true;
                    }
                    if (_checkPoints.Count > 1)
                        _prevDist2Checkpoint = -1.0;
                    _nextCheckpointIdx++; if (_nextCheckpointIdx >= _checkPoints.Count) {
                        _nextCheckpointIdx = 0;
                    }
                }
            }


            // Displays
            CalcAvgLapTime();
            if (_logLapTime) {
                _logLapTime = false;
                if (_lapTime.HasValue)
                    _allLapTimes.Add(_lapTime.Value);

                // if (_logPanel != null) {
                //     WriteToDisplay(_logPanel,
                //         " Lap {0:N0} - {2}\n{1}",
                //         _lapNumber - 1,
                //         _logPanel.GetText(),
                //         FormatLapTime(_lapTime));
                // }
            }

            if (!_finishedRace) {
                WriteToDisplay((_sc as IMyTextSurfaceProvider).GetSurface(3), //_lapPanel,
                    "  Lap  {0:N0} / {1:N0}\n  Lap Time\n {2}\n {3}{4}",
                    _lapNumber,
                    TOTAL_NUM_LAPS,
                    FormatLapTime(_lapTime),
                    FormatLapTime(_avgLapTime),
                    _avgLapTime.HasValue ? " Avg" : "");
            } else {
                var fastestLapTime = GetFastestLapTime();
                WriteToDisplay((_sc as IMyTextSurfaceProvider).GetSurface(2),
                    "  FINISHED\n\n {0}{1}\n {2}{3}",
                    FormatLapTime(fastestLapTime),
                    fastestLapTime.HasValue ? " Best" : "",
                    FormatLapTime(_avgLapTime),
                    _avgLapTime.HasValue ? " Avg" : "");
            }

            WriteToDisplay((_sc as IMyTextSurfaceProvider).GetSurface(1),
                "{0:N1}\n   m/s",
                _sc.GetShipSpeed());

            // if (_horizonPanel != null)
                ArtificialHorizonMain();

            // Save for next tick
            _prevDist2Checkpoint = range2cp;
            if (!inRange && _prevInRange)
                _passedCP = false;
            _prevInRange = inRange;
        }


        // void WriteToDisplay(IMyTextPanel panel, string text, params object[] args) {
        //     if (panel == null)
        //         return;
        //     panel.WriteText(string.Format(text, args));
        //     panel.ContentType = ContentType.TEXT_AND_IMAGE;
        // }
        void WriteToDisplay(IMyTextSurface panel, string text, params object[] args) {
            if (panel == null)
                return;
            panel.WriteText(string.Format(text, args));
            panel.ContentType = ContentType.TEXT_AND_IMAGE;
        }


        void CalcAvgLapTime() {
            if (_allLapTimes.Count <= 0)
                return;

            long totalTicks = 0;
            for (var i = 0; i < _allLapTimes.Count; i++)
                totalTicks += _allLapTimes[i].Ticks;

            _avgLapTime = new TimeSpan(totalTicks / (long)_allLapTimes.Count);
        }
        TimeSpan? GetFastestLapTime() {
            TimeSpan? fastest = null;
            for (var i = 0; i < _allLapTimes.Count; i++) {
                if (!fastest.HasValue || _allLapTimes[i].Ticks < fastest.Value.Ticks)
                    fastest = _allLapTimes[i];
            }
            return fastest;
        }

        string FormatLapTime(TimeSpan? time) {
            if (!time.HasValue)
                return "";

            return string.Format(
                "{0}:{1:00}.{2:N0}",
                time.Value.Minutes,
                time.Value.Seconds,
                time.Value.Milliseconds / 10);
        }


        /**************************************************************************************************************************************/
        IMyShipController _sc;
        // IMyTextPanel _lapPanel;
        // IMyTextPanel _speedPanel;
        // IMyTextPanel _logPanel;
        // IMyTextPanel _horizonPanel;

        List<CheckpointPosition> _checkPoints = new List<CheckpointPosition>();
        int _nextCheckpointIdx;
        double _prevDist2Checkpoint;
        int _lapNumber;
        int _numLapsRemaining;
        DateTime? _lapStartTime;
        TimeSpan? _lapTime;
        List<TimeSpan> _allLapTimes = new List<TimeSpan>();
        TimeSpan? _avgLapTime;
        bool _prevInRange;
        bool _passedCP;
        bool _logLapTime;
        bool _inited;
        bool _finishedRace = false;


        void Init(string cmdArg) {
            if (cmdArg == CMD_RESET)
                _inited = false;

            if (_inited)
                return;

            _nextCheckpointIdx = 0;
            _prevDist2Checkpoint = -1.0;

            _lapNumber = 0;
            _finishedRace = false;
            _numLapsRemaining = TOTAL_NUM_LAPS;
            _lapStartTime = null;
            _lapTime = null;
            _prevInRange = false;
            _passedCP = false;
            _logLapTime = false;
            _allLapTimes.Clear();
            _avgLapTime = null;

            // Find the cockpit or remote control block
            var tmp = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(tmp);
            if (tmp.Count <= 0)
                GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(tmp);
            _sc = (tmp.Count > 0) ? tmp[0] as IMyShipController : null;

            // Find the displays
            // _lapPanel = InitPanel(LCD_LapInfo_NAME, LCD_LapInfo_FONTSIZE);
            // _speedPanel = InitPanel(LCD_Speed_Name, LCD_Speed_FONTSIZE);
            // _logPanel = InitPanel(LCD_LapLog_NAME, LCD_LapLog_FONTSIZE);
            // _horizonPanel = InitPanel(LCD_ArtificialHorizon_NAME, LCD_ArtificialHorizon_FONTSIZE);

            // Load Checkpoints from Lap Display Panel
            _checkPoints.Clear();
            // if (_lapPanel != null) {
                var tmpPoints = CheckpontText.Split(new char[] { '\n' });
                for (var i = 0; i < tmpPoints.Length; i++)
                    _checkPoints.Add(new CheckpointPosition(tmpPoints[i].Trim()));
            // }

            // Check if all needed blocks where found
            var valid = true;
            if (_sc == null) { Echo("No Cockpit or Remote Control block found!"); valid = false; }
            // if (_lapPanel == null) { Echo("Not Found: " + LCD_LapInfo_NAME); valid = false; }
            if (_checkPoints.Count <= 0) { Echo("Checkpoints not loaded"); valid = false; }
            if (!valid)
                throw new Exception("Ship is Missing blocks");

            Echo(string.Format("Loaded {0:N0} checkpoints", _checkPoints.Count));

            _inited = true;
        }
        IMyTextPanel InitPanel(string panelName, float fontSize = 1.0f) {
            var panel = GridTerminalSystem.GetBlockWithName(panelName) as IMyTextPanel;

            if (panel != null) {
                panel.WriteText("");
                panel.FontSize = fontSize;
                panel.ContentType = ContentType.TEXT_AND_IMAGE;
            }

            return panel;
        }


        /**************************************************************************************************************************************/
        class CheckpointPosition {
            private static readonly char[] __delimiter = new char[] { ':' };

            public CheckpointPosition(string pointInfo) {
                var parts = pointInfo.Split(__delimiter);

                Name = parts[1].Trim();
                Position = new Vector3D(
                    double.Parse(parts[2]),
                    double.Parse(parts[3]),
                    double.Parse(parts[4]));
                RangeRadius = double.Parse(parts[5]);
            }

            public string Name { get; private set; }
            public Vector3D Position { get; private set; }
            public double RangeRadius { get; private set; }
        }



        /***************************************************************************************************************************************
        *  Whip's Artificial Horizon Script
        *  Modified for use here.
        ***************************************************************************************************************************************/


        //===================================================
        //-------------------------------------------------
        //COLOR CONSTANTS: Do not change!!!
        //Credit to alex-thatradarguy for finding these characters
        //http://steamcommunity.com/sharedfiles/filedetails/?id=627416824
        //-------------------------------------------------
        const string green = "\uE001"; //No touchey
        const string blue = "\uE002"; //No touchey
        const string red = "\uE003"; //No touchey
        const string yellow = "\uE004"; //No touchey
        const string white = "\uE006"; //No touchey
        const string lightGray = "\uE00E"; //No touchey
        const string mediumGray = "\uE00D"; //No touchey
        const string darkGray = "\uE00F"; //No touchey
                                          //====================================================

        //-------------------------------------------------
        //COLOR DEFAULTS: You can change these; see the above section for allowed colors
        //-------------------------------------------------
        const string backgroundColor = darkGray;
        const string horizonLineColor = darkGray;
        const string belowHorizonColor = green;
        const string aboveHorizonColor = blue;
        const string textColor = red;
        const string numberColor = yellow;
        const string velocityIndicatorColor = yellow;
        const string spaceOrientationColor = lightGray;
        const string planetaryOrientationColor = red;

        //---------------------------------------------------
        //DO NOT TOUCH ANYTHING BELOW THIS
        //---------------------------------------------------
        const int midpoint = 26; // this should be an even number, DO NOT TOUCH
        const int gridSize = midpoint * 2 - 1;
        const int planeSymbolWidth = 10; //measured from center to wingtip

        int velocityRow = 0;
        int velocityColumn = 0;

        const double timeCycleMax = 1 / (double)updatesPerSecond;
        const double rad2deg = 180 / Math.PI;
        const double deg2rad = Math.PI / 180;
        double velocityIncrement = maximumVelocity / (double)midpoint;
        double pitchIncrement = 90 / (double)midpoint;
        double timeCurrentCycle = 0;
        double shipSpeed = 0;
        double rollAngle = 0;
        double pitchAngle = 0;
        double bearingAngle = 0;

        bool isUpsideDown = false;
        bool inGravity = false;
        bool isBackwards = false;

        string headingAndVelocityString = "";

        Dictionary<Vector2I, string> characterGrid = new Dictionary<Vector2I, string>();
        Vector3D absoluteNorthVec = new Vector3D(0, 0, 1);

        void ArtificialHorizonMain() {
            if (timeCurrentCycle < timeCycleMax)
                return;

            GetVelocity();
            GetRollPitchAndHeading();
            headingAndVelocityString = GetNumberPlacesString(shipSpeed, bearingAngle);
            DrawGrid();

            timeCurrentCycle = 0;
            characterGrid.Clear();
        }


        void GetVelocity() //This method gets the relative position of the velocity vector on the screen
        {
            var velocityVec = _sc.GetShipVelocities().LinearVelocity;
            var rightVelocity = VectorProjection(velocityVec, _sc.WorldMatrix.Right).Length() * VectorCompareDirection(velocityVec, _sc.WorldMatrix.Right);
            var upVelocity = VectorProjection(velocityVec, _sc.WorldMatrix.Up).Length() * VectorCompareDirection(velocityVec, _sc.WorldMatrix.Up);
            var forwardVelocity = VectorProjection(velocityVec, _sc.WorldMatrix.Forward).Length() * VectorCompareDirection(velocityVec, _sc.WorldMatrix.Forward);
            shipSpeed = velocityVec.Length(); //raw speed of ship

            if (rightVelocity < 0) {
                velocityColumn = midpoint - (int)Math.Round(-rightVelocity / velocityIncrement);
            } else {
                velocityColumn = midpoint + (int)Math.Round(rightVelocity / velocityIncrement);
            }

            if (upVelocity < 0) {
                velocityRow = midpoint + (int)Math.Round(-upVelocity / velocityIncrement);
            } else {
                velocityRow = midpoint - (int)Math.Round(upVelocity / velocityIncrement);
            }

            if (forwardVelocity < 0) {
                isBackwards = true;
            } else {
                isBackwards = false;
            }

            if (enableVelocityVector) {
                if (!isBackwards) //draws prograde velocity
                {
                    /*
                    Looks like:
                        o o o
                    o o o   o o o
                        o o o
                          o
                          o
                    */

                    AddToGrid(new Vector2I(velocityRow + 1, velocityColumn), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 1, velocityColumn + 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 1, velocityColumn - 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 1, velocityColumn), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 1, velocityColumn + 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 1, velocityColumn - 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn + 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn - 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 2, velocityColumn), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 3, velocityColumn), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn + 2), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn + 3), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn - 2), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow, velocityColumn - 3), velocityIndicatorColor);

                } else //draws retrograde velocity
                  {
                    /*
                    Looks like:
                       o       o
                         o   o
                           o
                         o   o
                       o       o
                    */

                    AddToGrid(new Vector2I(velocityRow, velocityColumn), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 1, velocityColumn + 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 1, velocityColumn - 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 1, velocityColumn + 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 1, velocityColumn - 1), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 2, velocityColumn + 2), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow - 2, velocityColumn - 2), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 2, velocityColumn + 2), velocityIndicatorColor);
                    AddToGrid(new Vector2I(velocityRow + 2, velocityColumn - 2), velocityIndicatorColor);
                }
            }
        }

        void DrawOrientationIndicator() {
            if (!inGravity) {
                for (int j = 1; j <= gridSize; j++) {
                    AddToGrid(new Vector2I(midpoint, j), spaceOrientationColor); //draws a horizontal line
                }

                for (int j = 1; j <= gridSize; j++) {
                    AddToGrid(new Vector2I(j, midpoint), spaceOrientationColor); //draws a vertical line
                }
            } else //draws a nose orientation indicator that looks like  --- W ---
              {
                AddToGrid(new Vector2I(midpoint, midpoint), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint + 1, midpoint + 1), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint + 1, midpoint - 1), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint, midpoint - 2), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint, midpoint + 2), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint - 1, midpoint - 3), planetaryOrientationColor);
                AddToGrid(new Vector2I(midpoint - 1, midpoint + 3), planetaryOrientationColor);

                for (int j = midpoint - planeSymbolWidth; j < midpoint - 4; j++) {
                    AddToGrid(new Vector2I(midpoint, j), planetaryOrientationColor);
                }

                for (int j = midpoint + 5; j <= midpoint + planeSymbolWidth; j++) {
                    AddToGrid(new Vector2I(midpoint, j), planetaryOrientationColor);
                }
            }
        }

        void GetRollPitchAndHeading() {
            /// Get Needed Vectors ///
            Vector3D shipForwardVec = _sc.WorldMatrix.Forward;
            Vector3D shipLeftVec = _sc.WorldMatrix.Left;
            Vector3D shipDownVec = _sc.WorldMatrix.Down;
            Vector3D gravityVec = _sc.GetNaturalGravity();
            Vector3D planetRelativeLeftVec = shipForwardVec.Cross(gravityVec);

            if (gravityVec.Length().ToString() == "NaN" || gravityVec.Length() == 0) {
                inGravity = false;
                DrawOrientationIndicator();
                return;
            } else {
                inGravity = true;
                if (enableOrientationIndicator) {
                    DrawOrientationIndicator();
                }
            }

            isUpsideDown = false;

            /// Compute Pitch and Roll ///
            if (!VectorIsSameDirection(shipDownVec, gravityVec)) {
                isUpsideDown = true;
            }

            rollAngle = VectorAngleBetween(shipLeftVec, planetRelativeLeftVec);

            rollAngle *= VectorCompareDirection(VectorProjection(shipLeftVec, gravityVec), gravityVec); //ccw is positive

            if (rollAngle > 90 || rollAngle < -90) {
                rollAngle = 180 - rollAngle; //accounts for upsidedown
            }

            pitchAngle = VectorAngleBetween(shipForwardVec, gravityVec); //angle from nose direction to gravity
            pitchAngle -= 90; //as 90 degrees is level with ground

            GetHorizonLine(); //gets horizon line

            /// Compute Bearing ///
            //get east vector
            Vector3D relativeEastVec = gravityVec.Cross(absoluteNorthVec);

            //get relative north vector
            Vector3D relativeNorthVec = relativeEastVec.Cross(gravityVec);

            //project forward vector onto a plane comprised of the north and east vectors
            Vector3D forwardProjNorthVec = VectorProjection(shipForwardVec, relativeNorthVec);
            Vector3D forwardProjEastVec = VectorProjection(shipForwardVec, relativeEastVec);
            Vector3D forwardProjPlaneVec = forwardProjEastVec + forwardProjNorthVec;

            //find angle from abs north to projected forward vector measured clockwise
            bearingAngle = Math.Acos(forwardProjPlaneVec.Dot(relativeNorthVec) / forwardProjPlaneVec.Length() / relativeNorthVec.Length()) * rad2deg;
            if (VectorIsSameDirection(shipForwardVec, relativeEastVec) == false) {
                bearingAngle = 360 - bearingAngle; //because of how the angle is measured
            }
        }

        void GetHorizonLine() {
            int horizontalOffset = (int)Math.Round(pitchAngle / pitchIncrement * Math.Sin(rollAngle * deg2rad)); //offset of every point in the horizontal direction
            int verticalOffset = (int)Math.Round(pitchAngle / pitchIncrement * Math.Cos(rollAngle * deg2rad)); //offset of every point in the vertical direction

            double constant = 1;
            if (isUpsideDown) {
                verticalOffset *= -1;
                constant = -1;

            }

            int adjustedMidpoint = midpoint - horizontalOffset; //offsets our midpoint horizontally due to pitch and roll

            for (int i = 1; i <= gridSize; i++) //rows
            {
                int thisHeight = 0;

                if (i < midpoint) {
                    thisHeight = adjustedMidpoint - (int)Math.Round((adjustedMidpoint - i) * Math.Tan(constant * rollAngle * deg2rad));
                } else {
                    thisHeight = adjustedMidpoint + (int)Math.Round((i - adjustedMidpoint) * Math.Tan(constant * rollAngle * deg2rad));
                }

                thisHeight += verticalOffset; //offset our computed height by this value

                AddToGrid(new Vector2I(thisHeight, i), horizonLineColor);

                string aboveColor = aboveHorizonColor; string belowColor = belowHorizonColor;
                if (isUpsideDown) {
                    aboveColor = belowHorizonColor; belowColor = aboveHorizonColor;
                }

                for (int j = 1; j <= gridSize; j++) {
                    if (j < thisHeight) {
                        AddToGrid(new Vector2I(j, i), aboveColor);
                    } else if (j > thisHeight) {
                        AddToGrid(new Vector2I(j, i), belowColor);
                    }
                }
            }
        }

        void AddToGrid(Vector2I place, string color) {
            string value;
            bool exists = characterGrid.TryGetValue(place, out value);
            if (exists) {
                return;
            } else {
                characterGrid.Add(place, color);
            }
        }

        void DrawGrid() //draws graphical grid
        {
            var grid = new StringBuilder(); //5 spaces to center

            if (showSpeedHeadingAbove) {
                grid.Append(headingAndVelocityString + "\n");
            }

            // Draw Horizon
            grid.Append("     "); //5 spaces to center
            for (int row = 1; row <= gridSize; row++) {
                for (int column = 1; column <= gridSize; column++) {
                    string character = backgroundColor;

                    Vector2I thisGridPosition = new Vector2I(row, column);

                    string thisCharacter;
                    bool containsPosition = characterGrid.TryGetValue(thisGridPosition, out thisCharacter);
                    if (containsPosition) {
                        character = thisCharacter;
                    }

                    grid.Append(character); //adds our current character
                }

                if (row != gridSize)
                    grid.Append("\n     ");
            }

            if (!showSpeedHeadingAbove) {
                grid.Append("\n\n" + headingAndVelocityString);
            }

            WriteToDisplay((_sc as IMyTextSurfaceProvider).GetSurface(0), grid.ToString());
        }


        string GetNumberPlacesString(double vel, double head) //gets a graphical string from velocity and heading
        {
            //Velocity Splitting
            double velHundreds = 0; double velTens = 0; double velOnes = 0;
            vel = Math.Round(vel);
            if (vel >= 100) {
                velHundreds = Math.Floor(vel / 100);
            } else {
                velHundreds = 0;
            }

            vel = vel - velHundreds * 100;
            if (vel >= 10) {
                velTens = Math.Floor(vel / 10);
            } else {
                velTens = 0;
            }

            velOnes = vel - velTens * 10;

            //Heading Splitting
            double headHundreds = 0; double headTens = 0; double headOnes = 0;
            head = Math.Round(head);
            if (head >= 100) {
                headHundreds = Math.Floor(head / 100);
            } else {
                headHundreds = 0;
            }

            head = head - headHundreds * 100;
            if (head >= 10) {
                headTens = Math.Floor(head / 10);
            } else {
                headTens = 0;
            }

            headOnes = head - headTens * 10;

            return CombineStringsByLine(
                  numVelocity + GetStringFromNumber(velHundreds) + GetStringFromNumber(velTens) + GetStringFromNumber(velOnes)
                + numHeading + GetStringFromNumber(headHundreds) + GetStringFromNumber(headTens) + GetStringFromNumber(headOnes));
        }

        string GetStringFromNumber(double num) //gets graphical representation from a double
        {
            string numString = "";

            switch (num.ToString()) {
                case "0":
                    numString = numZero;
                    break;

                case "1":
                    numString = numOne;
                    break;

                case "2":
                    numString = numTwo;
                    break;

                case "3":
                    numString = numThree;
                    break;

                case "4":
                    numString = numFour;
                    break;

                case "5":
                    numString = numFive;
                    break;

                case "6":
                    numString = numSix;
                    break;

                case "7":
                    numString = numSeven;
                    break;

                case "8":
                    numString = numEight;
                    break;

                case "9":
                    numString = numNine;
                    break;

                default:
                    numString = numZero;
                    break;
            }
            return numString;
        }

        string CombineStringsByLine(string inputString) //messy way of combining our characters line by line
        {
            var stringLines = inputString.Split('\n');
            var outputString = new StringBuilder();
            if (enableSpeedDisplay || enableHeadingDisplay) {
                for (int i = 0; i < 5; i++) {
                    if (enableSpeedDisplay)
                        outputString.Append("  " + stringLines[i] + backgroundColor + stringLines[i + 5] + backgroundColor + stringLines[i + 10] + backgroundColor + stringLines[i + 15]);
                    if (enableSpeedDisplay && enableHeadingDisplay)
                        outputString.Append("     "); //5 spaces
                    if (enableHeadingDisplay)
                        outputString.Append(stringLines[i + 20] + backgroundColor + stringLines[i + 25] + backgroundColor + stringLines[i + 30] + backgroundColor + stringLines[i + 35]);
                    outputString.Append("\n");
                }
            }
            return outputString.ToString();
        }

        int VectorCompareDirection(Vector3D a, Vector3D b) //returns -1 if vectors return negative dot product
        {
            double check = a.Dot(b);
            if (check < 0)
                return -1;
            else
                return 1;
        }

        double VectorAngleBetween(Vector3D a, Vector3D b) //returns degrees
        {
            return Math.Acos(a.Dot(b) / a.Length() / b.Length()) * 180 / Math.PI;
        }

        Vector3D VectorProjection(Vector3D a, Vector3D b) //projects a onto b
        {
            Vector3D projection = a.Dot(b) / b.Length() / b.Length() * b;
            return projection;
        }

        bool VectorIsSameDirection(Vector3D a, Vector3D b) //returns true if vectors produce positive dot product
        {
            double check = a.Dot(b);
            if (check < 0)
                return false;
            else
                return true;
        }

        //Whip's Running Symbol Method v3
        double timeSymbol = 0;
        string strRunningSymbol = "";

        string RunningSymbol() //makes a cool spinning bar symbol :)
        {
            if (timeSymbol < .2d)
                strRunningSymbol = "|";
            else if (timeSymbol < .4d)
                strRunningSymbol = "/";
            else if (timeSymbol < .6d)
                strRunningSymbol = "--";
            else if (timeSymbol < .8d)
                strRunningSymbol = "\\";
            else {
                timeSymbol = 0;
                strRunningSymbol = "|";
            }

            return strRunningSymbol;
        }

        const string numZero = numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numOne = backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n";

        const string numTwo = numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + backgroundColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numThree = numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + backgroundColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numFour = numberColor + backgroundColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n";

        const string numFive = numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + backgroundColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numSix = numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + backgroundColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numSeven = numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n"
            + backgroundColor + numberColor + backgroundColor + "\n";

        const string numEight = numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";                                                                                                                                                                                           //w.H.1.p.L.4.s.H

        const string numNine = numberColor + numberColor + numberColor + "\n"
            + numberColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n"
            + backgroundColor + backgroundColor + numberColor + "\n"
            + numberColor + numberColor + numberColor + "\n";

        const string numHeading = textColor + backgroundColor + textColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + "\n"
            + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + backgroundColor + backgroundColor + backgroundColor + textColor + "\n"
            + textColor + textColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + "\n"
            + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + backgroundColor + textColor + backgroundColor + textColor + "\n"
            + textColor + backgroundColor + textColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + "\n";

        const string numVelocity = backgroundColor + textColor + textColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + "\n"
            + textColor + backgroundColor + backgroundColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + "\n"
            + textColor + textColor + textColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + backgroundColor + "\n"
            + backgroundColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + backgroundColor + backgroundColor + textColor + backgroundColor + textColor + backgroundColor + textColor + "\n"
            + textColor + textColor + backgroundColor + backgroundColor + textColor + backgroundColor + backgroundColor + backgroundColor + textColor + textColor + backgroundColor + backgroundColor + backgroundColor + "\n";

    }
}
