using Sandbox.Game.EntityComponents;
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
            _rangeToDestination = (_destination != null) ? Vector3D.Distance(pos, _destination.Location) : 0.0;
            _rangeToGround = Vector3D.Distance(pos, _settings.GetBottomPoint());
            _rangeToSpace = Vector3D.Distance(pos, _settings.GetTopPoint());

            _verticalSpeed = ((_rangeToGround - _rangeToGroundLast) >= 0)
                ? _rc.GetShipSpeed()
                : _rc.GetShipSpeed() * -1;
            var totalMaxBreakingThrust = _ascentThrusters.Sum(b => b.MaxEffectiveThrust);
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

            _h2TankFilledPercent = GasTankHelper.GetTanksFillPercentage(_h2Tanks);

            var speed = Math.Round(_verticalSpeed, 1);
            var speedDir = "--";
            if (speed > 0) speedDir = @"/\";
            if (speed < 0) speedDir = @"\/";

            _debug.AppendLine($"Speed: {speedDir}  {Math.Abs(_verticalSpeed):N1}");
            _debug.AppendLine($"Lift T/W r: {totalMaxBreakingThrust / _gravityForceOnShip:N2}");
            _debug.AppendLine($"Brake Dist: {brakeingRange:N2}");
            _debug.AppendLine("");
            _debug.AppendLine($"Range to Destination: {_rangeToDestination:N2} m");
            _debug.AppendLine($"Range to Ground: {_rangeToGround:N2} m");

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
