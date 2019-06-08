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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {

        //const string OPS_GravAlign = "GravAlign";
        //const string OPS_Launch = "FlightOps";



        //IEnumerator<bool> Sequence_Launch() {
        //    yield return true;

        //    // Turn on Thrusters
        //    AscentThrusters.ForEach(t => t.Enabled = true);

        //    var delay = Delay(2000);
        //    while (delay.MoveNext()) yield return true;

        //    AscentThrusters.ForEach(t => t.ThrustOverridePercentage = 1f);

        //    delay = Delay(500);
        //    while (delay.MoveNext()) yield return true;

        //    LandingGears.ForEach(lg => lg.Unlock());
        //    LaunchClamps.ForEach(lc => lc.Detach());

        //    //double altitude = 0.0;
        //    //while (true) {
        //    //    if (!Remote.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude)) break;
        //    //    if (altitude >= 100.0)
        //    //        yield return true;
        //    //}

        //    var fuelLevel = 0.0;
        //    while (true) {
        //        fuelLevel = H2Tanks.Average(t => t.FilledRatio);
        //        if (fuelLevel <= 0.1) break;
        //        //SetBurnLevel
        //        yield return true;
        //    }

        //    // boosters to full
        //    // disconnect launch clamps
        //}

        //double? CurrentAltitude;
        ////double? VerticalVelocity;

        //class StageBlocks {
        //    public List<IMyThrust> Thrusters;
        //    public List<IMyGasTank> H2Tanks;
        //    public List<IMyMotorStator> StageClamps;
        //    public List<IMyLandingGear> LandingGears;
        //    public double? StageAltitude = null;
        //    public double? StageFuelReserve = null;

        //    //public Action StageAction = null;
        //    public bool ReadyToStage(double? currentAltitude) {
        //        if (currentAltitude >= StageAltitude) return true;
        //        if (StageAltitude.HasValue) return true;
        //        var currentFuel = GasTankHelper.GetTanksFillPercentage(H2Tanks);
        //        if (currentFuel <= StageFuelReserve) return true;
        //        return false;
        //    }
        //}

        //IEnumerator<bool> Sequence_StageAndThrust(StageBlocks args) {
        //    // Turn on Thrusters
        //    args.Thrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1f; });
        //    args.LandingGears?.ForEach(lg => lg.Unlock());
        //    args.StageClamps?.ForEach(lc => lc.Detach());

        //    while (true) {
        //        yield return true;
        //        if (!args.ReadyToStage(CurrentAltitude)) continue;

        //    }


        //    double altitude = 0.0;
        //    while (true) {
        //        if (!Remote.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude)) break;
        //        if (altitude >= 100.0)
        //            yield return true;
        //    }

        //    var fuelLevel = 0.0;
        //    while (true) {
        //        fuelLevel = H2Tanks.Average(t => t.FilledRatio);
        //        if (fuelLevel <= 0.1) break;
        //        //SetBurnLevel
        //        yield return true;
        //    }

        //    // boosters to full
        //    // disconnect launch clamps
        //}

        //void LoadCalculations() {
        //    var _gravVec = Remote.GetNaturalGravity();
        //    //gravity in m/s^2
        //    var _gravMS2 = Math.Sqrt(
        //        Math.Pow(_gravVec.X, 2) +
        //        Math.Pow(_gravVec.Y, 2) +
        //        Math.Pow(_gravVec.Z, 2));

        //    var _inNaturalGravity = (_gravMS2 > 0.0);

        //    // carriage total mass including cargo mass
        //    var totalMass = Remote.CalculateShipMass().TotalMass;
        //    // mass of the carriage without cargo
        //    var baseMass = Remote.CalculateShipMass().BaseMass;
        //    var _cargoMass = totalMass - baseMass;
        //    // the mass the game uses for physics calculation
        //    var _actualMass = baseMass + (_cargoMass / 3); //_settings.InventoryMultiplier);
        //    // the gravity "thrust" applied to the carriage
        //    var _gravityForceOnShip = _actualMass * _gravMS2;

        //    //_cargoMass = _cargo.Sum(c => CargoHelper.GetInventoryTotals(c, CargoHelper.GetInventoryCurrentMass)) / 1000000.0;

        //    var pos = Remote.GetPosition();
        //    //_rangeToDestination = (_destination != null) ? Vector3D.Distance(pos, _destination.Location) : 0.0;
        //    //_rangeToGround = Vector3D.Distance(pos, _settings.GetBottomPoint());
        //    //_rangeToSpace = Vector3D.Distance(pos, _settings.GetTopPoint());

        //    //_verticalSpeed = ((_rangeToGround - _rangeToGroundLast) >= 0)
        //    //    ? Remote.GetShipSpeed()
        //    //    : Remote.GetShipSpeed() * -1;
        //    var totalMaxBreakingThrust = _ascentThrusters.Sum(b => b.MaxEffectiveThrust);
        //    var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

        //    _h2TankFilledPercent = GasTankHelper.GetTanksFillPercentage(_h2Tanks);

        //    if (_doCalcStatus) {
        //        _doCalcStatus = false;
        //        SetStatuses();
        //    }
        //}
        //void MaintainSpeed(IMyShipController sc, double targetVelocity, params List<IMyThrust>[] allThrusterGroups) {
        //    foreach (var thrustGroup in allThrusterGroups)
        //        MaintainSpeed(sc, targetVelocity, thrustGroup);
        //}
        //void MaintainSpeed(IMyShipController sc, double targetVertSpeed, List<IMyThrust> thrusters) {
        //    var ascentMaxEffectiveThrust = thrusters.Sum(b => b.MaxEffectiveThrust);
        //    var aaa = sc.GetNaturalGravity();


        //    var hoverOverridePower = _inNaturalGravity
        //        ? Convert.ToSingle(_gravityForceOnShip / ascentMaxEffectiveThrust)
        //        : 0f;

        //    var ascentOverridePower = 0f;
        //    var decentOverridePower = 0f;
        //    var speedDiff = targetVertSpeed - _verticalSpeed;

        //    if (speedDiff > -0.5f && speedDiff < 0.5f) {
        //        ascentOverridePower = hoverOverridePower;
        //    } else if (speedDiff > 2) {
        //        ascentOverridePower = 1f;
        //    } else if (speedDiff > 0) {
        //        ascentOverridePower = hoverOverridePower + ((1f - hoverOverridePower) / 2);
        //    } else if (speedDiff < -2) {
        //        decentOverridePower = 1f;
        //    } else if (speedDiff < 0) {
        //        ascentOverridePower = hoverOverridePower;
        //        decentOverridePower = 0.02f;
        //    }

        //    foreach (var b in _ascentThrusters) b.ThrustOverridePercentage = ascentOverridePower;
        //    foreach (var b in _descentThrusters) b.ThrustOverridePercentage = decentOverridePower;
        //}



        //IEnumerator<bool> Sequence_LaunchGravAlign() {
        //    while (true) {
        //        VecAlign.AlignWithGravity(Remote, Direction.Backward, Gyros, true);
        //        yield return true;
        //    }
        //}

        //IEnumerator<bool> Sequence_LandGravAlign() {
        //    while (true) {
        //        VecAlign.AlignWithGravity(Remote, Direction.Down, Gyros);
        //        yield return true;
        //    }
        //}
    }
}
