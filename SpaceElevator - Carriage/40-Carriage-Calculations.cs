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

        void LoadCalculations() {
            _gravVec = _rc.GetNaturalGravity();
            //gravity in m/s^2
            _gravMS2 = Math.Sqrt(
                Math.Pow(_gravVec.X, 2) +
                Math.Pow(_gravVec.Y, 2) +
                Math.Pow(_gravVec.Z, 2));

            _inNaturalGravity = (_gravMS2 > 0.0);

            // carriage total mass including cargo mass
            var totalMass = _rc.CalculateShipMass().TotalMass;
            // mass of the carriage without cargo
            var baseMass = _rc.CalculateShipMass().BaseMass;
            _cargoMass = totalMass - baseMass;
            // the mass the game uses for physics calculation
            _actualMass = baseMass + (_cargoMass / _settings.InventoryMultiplier);
            // the gravity "thrust" applied to the carriage
            _gravityForceOnShip = _actualMass * _gravMS2;

            _cargoMass = _cargo.Sum(c => CargoHelper.GetInventoryTotals(c, CargoHelper.GetInventoryCurrentMass)) / 1000000.0;

            var pos = _rc.GetPosition();
            if (_destination != null)
                _rangeToDestination = Vector3D.Distance(pos, _destination.GetLocation());
            _rangeToGround = Vector3D.Distance(pos, _settings.GetBottomPoint());
            _rangeToSpace = Vector3D.Distance(pos, _settings.GetTopPoint());

            _verticalSpeed = ((_rangeToGround - _rangeToGroundLast) >= 0)
                ? _rc.GetShipSpeed()
                : _rc.GetShipSpeed() * -1;
            var totalMaxBreakingThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

            _h2TankFilledPercent = GasTankHelper.GetTanksFillPercentage(_h2Tanks);

            //var speed = Math.Round(_verticalSpeed, 1);
            //var speedDir = "--";
            //if (speed > 0) speedDir = "/\\";
            //if (speed < 0) speedDir = "\\/";

            //_debug.AppendLine("Speed: {1}  {0:N1}", Math.Abs(_verticalSpeed), speedDir);
            //_debug.AppendLine("Lift T/W r: {0:N2}", totalMaxBreakingThrust / _gravityForceOnShip);
            //_debug.AppendLine("Brake Dist: {0:N2}", brakeingRange);
            //_debug.AppendLine("");
            //if (_destination != null)
            //    _debug.AppendLine("Range to destination: {0:N2} m", _rangeToDestination);
            //_debug.AppendLine("Range to Ground: {0:N2} m", _rangeToGround);
            //_debug.AppendLine("MODE: {0}", GetMode());

            if (_doCalcStatus) {
                _doCalcStatus = false;
                SetStatuses();
            }
        }
        void SaveLastValues() {
            _rangeToGroundLast = _rangeToGround;
        }

        double CalcBrakeDistance(double maxthrust, double gravForceOnShip) {
            var brakeForce = maxthrust - gravForceOnShip;
            if (brakeForce < 0.0) brakeForce = 0.0;
            var deceleration = brakeForce / _actualMass;
            return Math.Pow(_rc.GetShipSpeed(), 2) / (2 * deceleration);
        }

    }
}
