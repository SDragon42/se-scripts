// <mdk sortorder="30" />
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

        IEnumerator<bool> Action_StartAllDrills() {
            Debug("Action_StartAllDrills invoked.");
            StartAllDrills();
            yield return true;
        }

        IEnumerator<bool> Action_DrillingLoop() {
            Debug("Action_DrillingLoop invoked.");
            while (true) {
                // Debug("Starting drilling loop iteration.");
                if (_cargoInventoryPercentage >= _config.HoldInventoryPercentage || _drillInventoryPercentage >= _config.HoldInventoryPercentage) {
                    CommandHold();
                    yield break; // Exit the drilling loop if we are holding due to inventory limits
                }
                // StartAllDrills();
                var isExtending = ExtendPistonChain();
                // Debug($"isExtending: {isExtending}");
                if (!isExtending) yield break; // Stop the enumerator if the piston chain cannot extend further
                yield return true;
            }
        }

        IEnumerator<bool> Action_SetStatus(MyDrillState status) {
            Debug($"Action_SetStatus invoked with status: {status}");
            State = status;
            yield return true;
        }
        IEnumerator<bool> Action_StopMovement() {
            Debug("Action_StopMovement invoked.");
            StopAllPistons();
            StopAllDrills();
            yield return true;
        }

        // IEnumerator<bool> Action_RetractPistons(int numberOfPistons = int.MaxValue) {
        //     Debug("Action_RetractPistons invoked.");
        //     var maxNumber = Math.Min(numberOfPistons, _pistons.Count);
        //     for (var i = 0; i < maxNumber; i++) {
        //         var b = _pistons[i];
        //         b.Velocity = _config.RetractionVelocity;
        //         b.Retract();
        //     }
        //     yield return true;
        // }


        // IEnumerator<bool> Action_WaitForRetractionCompleted(int numberOfPistons = int.MaxValue) {
        //     Debug("Action_WaitForRetractionCompleted invoked.");
        //     var allRetracted = false;
        //     var pistonsToCheck = _pistons.Take(numberOfPistons).ToArray();
        //     do {
        //         allRetracted = pistonsToCheck.All(IsRetracted);
        //         if (!allRetracted)
        //             yield return true;
        //     } while (!allRetracted);
        // }

        IEnumerator<bool> Action_RetractPistonLoop(int numberOfPistons = int.MaxValue) {
            Debug("Action_RetractPistonLoop invoked.");
            while (true) {
                
                // StartAllDrills();
                var isRetracting = RetractPistonChain(numberOfPistons);
                // Debug($"isRetracting: {isRetracting}");
                if (!isRetracting) yield break; // Stop the enumerator if the piston chain cannot retract further
                yield return true;
            }
        }

        IEnumerator<bool> Action_WaitForHoldCondition() {
            Debug("Action_WaitForHoldCondition invoked.");
            while (_cargoInventoryPercentage >= _config.ResumeInventoryPercentage || _drillInventoryPercentage >= _config.ResumeInventoryPercentage) {
                yield return true;
            }
        }

        IEnumerator<bool> Action_RunCommand(Action command) {
            command?.Invoke();
            yield return true;
        }

    }
}
