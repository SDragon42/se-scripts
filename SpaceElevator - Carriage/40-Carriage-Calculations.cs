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

            _h2TankFilledPercent = GasTankHelper.GetTanksFillPercentage(_h2Tanks);

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

        void SetStatuses() {
            _status.Position = _rc.GetPosition();
            _status.VerticalSpeed = _verticalSpeed;
            _status.FuelLevel = _h2TankFilledPercent;
            _status.CargoMass = _cargoMass;
            _status.Range2Bottom = _rangeToGround;
            _status.Range2Top = _rangeToSpace;
        }


    }
}
