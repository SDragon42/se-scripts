﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        CarriageMode _mode_SpecialUseOnly;
        CarriageMode GetMode() { return _mode_SpecialUseOnly; }
        void SetMode(CarriageMode value) {
            if (_mode_SpecialUseOnly == value && value != CarriageMode.Manual_Control) return;
            _mode_SpecialUseOnly = value;

            if (!Enum.IsDefined(typeof(CarriageMode), value))
                _mode_SpecialUseOnly = CarriageMode.Manual_Control;

            _status.Mode = GetMode();

            switch (_mode_SpecialUseOnly) {
                case CarriageMode.Manual_Control:
                    ClearAutopilot(true);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    _destination = null;
                    _travelDirection = TravelDirection.None;
                    break;

                case CarriageMode.Transit_Powered:
                    ClearAutopilot(false);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.Unlock();
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _connectors) b.Disconnect();
                    break;

                case CarriageMode.Transit_Coast:
                    ClearAutopilot(false);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;

                case CarriageMode.Transit_Slow2Approach:
                    ClearAutopilot(false);
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;

                case CarriageMode.Transit_Docking:
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    _connectorLockDelayRemaining = _settings.ConnectorLockDelay;
                    break;

                case CarriageMode.Docked:
                    ClearAutopilot(true);
                    foreach (var b in _h2Tanks) b.Stockpile = true;
                    foreach (var b in _allThrusters) b.Enabled = false;
                    foreach (var b in _landingGears) b.Enabled = false;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;
            }
        }

        void ClearAutopilot(bool enableDampeners) {
            _rc.SetAutoPilotEnabled(false);
            _rc.DampenersOverride = enableDampeners;
            _rc.ClearWaypoints();
        }

        void ActivateAutopilot(Vector3D target) {
            _rc.DampenersOverride = true;
            _rc.ClearWaypoints();
            _rc.AddWaypoint(target, "Destination");
            _rc.SetValueBool("DockingMode", true); // Activate Precision mode.
            _rc.SetValue<long>("FlightMode", 2); // Sets Flight mode to "One way". 2 is index of "One way" in combobox. (0 = Patrol, 1 = Circle)
            _rc.SetValue<float>("SpeedLimit", Convert.ToSingle(_settings.DockSpeed));
            _rc.SetAutoPilotEnabled(true);
        }

        void RunModeActions() {
            CheckRampsAtLimits();
            Action travelMethod = null;
            if (_travelDirection == TravelDirection.Ascent && _boardingRampsClear)
                travelMethod = AscentModeOps;
            else if (_travelDirection == TravelDirection.Descent && _boardingRampsClear)
                travelMethod = DecentModeOps;

            switch (GetMode()) {
                case CarriageMode.Awaiting_CarriageReady2Depart:
                    if (_boardingRampsClear && travelMethod != null)
                        SetMode(CarriageMode.Transit_Powered);
                    break;
                //case CarriageMode.Awaiting_DepartureClearance: goto case CarriageMode.Transit_Powered;
                case CarriageMode.Transit_Powered: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Coast: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Slow2Approach: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Docking: LockConnectorsWhenStopped(); break;
                case CarriageMode.Docked: LowerBoardingRamps(); break;
            }
        }

        void LockConnectorsWhenStopped() {
            if (_rc.GetShipSpeed() > 0.1) return;

            _landingGears.ForEach(b => b.Lock());

            var anyLocked = _landingGears.Any(Collect.IsLandingGearLocked);
            if (anyLocked) {
                SetMode(CarriageMode.Docked);
                SendDockedMessage(_destination.GetName());
                _travelDirection = TravelDirection.None;
                _destination = null;
            }
        }

        void RaiseBoardingRamps() {
            _boardingRampsClear = true;
            if (_boardingRamps.Count == 0) return;
            _boardingRamps.ForEach(rotor => _boardingRampsClear &= Rotate2Limit(rotor, false));
            if (_gravityGen != null) _gravityGen.FieldSize = new Vector3(GRAV_RANGE_Rampsup, _gravityGen.FieldSize.Y, _gravityGen.FieldSize.Z);
        }
        void LowerBoardingRamps() {
            if (_boardingRamps.Count == 0) return;
            if (GetMode() != CarriageMode.Docked && GetMode() != CarriageMode.Manual_Control)
                return;
            _boardingRampsClear = false;
            _boardingRamps.ForEach(rotor => Rotate2Limit(rotor, true));
            if (_gravityGen != null) _gravityGen.FieldSize = new Vector3(GRAV_RANGE_RampsDown, _gravityGen.FieldSize.Y, _gravityGen.FieldSize.Z);
        }
        bool Rotate2Limit(IMyMotorStator rotor, bool rotateToMax) {
            if (IsRotated2Limit(rotor, rotateToMax)) return true;
            rotor.SafetyLock = false;
            var velocity = rotateToMax ? ElevatorConst.ROTOR_VELOCITY : ElevatorConst.ROTOR_VELOCITY * -1;
            rotor.SetValueFloat("Velocity", velocity);
            return false; // not in position yet
        }
        bool IsRotated2Limit(IMyMotorStator rotor, bool rotateToMax) {
            if (rotor == null) return true;
            var currAngle = Math.Round(rotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
            var angleLimit = rotateToMax ? rotor.UpperLimit : rotor.LowerLimit;
            var targetAngle = Math.Round(angleLimit, ElevatorConst.RADIAN_ROUND_DIGITS);
            var notAtTarget = rotateToMax ? (currAngle < targetAngle) : (currAngle > targetAngle);
            return !notAtTarget;
        }
        void CheckRampsAtLimits() {

            var atLimit = true;
            var isRaised = true;
            foreach (var r in _boardingRamps) {
                var rotate2Max = r.TargetVelocity > 0;
                atLimit &= IsRotated2Limit(r, rotate2Max);
                isRaised &= !rotate2Max && atLimit;
            }
            _boardingRampsClear = isRaised;
            if (!atLimit) return;
            _boardingRamps.ForEach(rotor => rotor.SafetyLock = true);
        }

        void AscentModeOps() {
            _rc.DampenersOverride = false;
            // attempt to compensate for the changing gravity force on the ship
            var gravityForceChangeCompensation = (_gravityForceOnShip / 2) * -1;

            var totalMaxBreakingThrust = _descentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var rangeToTarget = (_rc.GetPosition() - _destination.GetLocation()).Length();
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, gravityForceChangeCompensation);
            var coastRange = CalcBrakeDistance(0.0, gravityForceChangeCompensation);

            _debug.AppendLine("Break Range: {0:N2}", brakeingRange);
            _debug.AppendLine("Coast Range: {0:N2}", coastRange);

            var inCoastRange = (rangeToTarget <= coastRange + _settings.ApproachDistance);
            var inBrakeRange = (rangeToTarget <= brakeingRange + _settings.ApproachDistance);
            var inDockRange = Math.Abs(rangeToTarget - brakeingRange) < SWITCH_TO_AUTOPILOT_RANGE;

            if (inDockRange) {
                SetMode(CarriageMode.Transit_Docking);
                ActivateAutopilot(_destination.GetLocation());
            } else if (!inCoastRange && !inBrakeRange && GetMode() != CarriageMode.Transit_Powered)
                SetMode(CarriageMode.Transit_Powered);
            else if (_settings.GravityDescelEnabled && inCoastRange && !inBrakeRange && GetMode() != CarriageMode.Transit_Coast)
                SetMode(CarriageMode.Transit_Coast);
            else if (inBrakeRange && GetMode() != CarriageMode.Transit_Slow2Approach)
                SetMode(CarriageMode.Transit_Slow2Approach);

            switch (GetMode()) {
                case CarriageMode.Transit_Powered:
                    MaintainSpeed(_settings.TravelSpeed);
                    break;
                case CarriageMode.Transit_Slow2Approach:
                    MaintainSpeed(_settings.DockSpeed);
                    break;
            }
        }
        void DecentModeOps() {
            _rc.DampenersOverride = false;
            var totalMaxBreakingThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var rangeToTarget = (_rc.GetPosition() - _destination.GetLocation()).Length();
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

            _debug.AppendLine("Break Range: {0:N2}", brakeingRange);
            _debug.AppendLine("Target Break Diff: {0:N2}", rangeToTarget - brakeingRange);

            var inBrakeRange = (rangeToTarget <= brakeingRange + _settings.ApproachDistance);
            var inDockRange = Math.Abs(rangeToTarget - brakeingRange) < SWITCH_TO_AUTOPILOT_RANGE;
            var inCoastZone = (!inBrakeRange && _rc.GetShipSpeed() >= _settings.TravelSpeed - 5.0);

            if (inCoastZone)
                SetMode(CarriageMode.Transit_Coast);
            else if (inDockRange) {
                SetMode(CarriageMode.Transit_Docking);
                ActivateAutopilot(_destination.GetLocation());
            } else if (inBrakeRange && GetMode() != CarriageMode.Transit_Slow2Approach)
                SetMode(CarriageMode.Transit_Slow2Approach);

            switch (GetMode()) {
                case CarriageMode.Transit_Powered:
                    MaintainSpeed(_settings.TravelSpeed * -1);
                    break;
                case CarriageMode.Transit_Slow2Approach:
                    MaintainSpeed(_settings.DockSpeed * -1);
                    break;
                case CarriageMode.Transit_Coast:
                    if (rangeToTarget - brakeingRange < 300
                        && !_activateSpeedLimiter
                        && _inNaturalGravity)
                        ThrusterHelper.SetThrusterOverride(_ascentThrusters[0], 1.001);
                    break;
            }
        }
        void MaintainSpeed(double targetVertSpeed) {
            var ascentMaxEffectiveThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var decentMaxEffectiveThrust = _descentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));

            var hoverOverridePower = _inNaturalGravity
                ? Convert.ToSingle((_gravityForceOnShip / ascentMaxEffectiveThrust) * 100)
                : 0f;

            var ascentOverridePower = 0f;
            var decentOverridePower = 0f;

            var speedDiff = targetVertSpeed - _verticalSpeed;
            _debug.AppendLine("");
            _debug.AppendLine("S.Diff: {0:N1}", speedDiff);

            if (speedDiff > -0.5f && speedDiff < 0.5f) {
                ascentOverridePower = hoverOverridePower;
            } else if (speedDiff > 2) {
                ascentOverridePower = 100f;
            } else if (speedDiff > 0) {
                ascentOverridePower = hoverOverridePower + ((100f - hoverOverridePower) / 2);
            } else if (speedDiff < -2) {
                decentOverridePower = 100f;
            } else if (speedDiff < 0) {
                ascentOverridePower = hoverOverridePower;
                decentOverridePower = 2f;
            }

            _debug.AppendLine("Ascent Override %: {0:N1}", ascentOverridePower);
            _debug.AppendLine("Decent Override %: {0:N1}", decentOverridePower);

            foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, ascentOverridePower);
            foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, decentOverridePower);
        }

    }
}
