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
        
        IEnumerator<bool> Action_SetStatus(MyDrillState status) {
            Debug($"Action_SetStatus invoked with status: {status}");
            _currentState = status;
            yield return true;
        }
        IEnumerator<bool> Action_StopMovement() {
            Debug("Action_StopMovement invoked.");
            StopAllPistons();
            StopAllDrills();
            yield return true;
        }

        IEnumerator<bool> Action_RetractPistons() {
            Debug("Action_RetractPistons invoked.");
            foreach (var b in _pistons) {
                b.Velocity = _config.RetractionVelocity;
                b.Retract();
            }
            yield return true;
        }

        IEnumerator<bool> Action_WaitForRetractionCompleted() {
            Debug("Action_WaitForRetractionCompleted invoked.");
            var allRetracted = false;
            do {
                allRetracted = _pistons.All(IsRetracted);
                if (!allRetracted)
                    yield return true;
            } while (!allRetracted);
        }

        IEnumerator<bool> Action_DrillingLoop() {
            Debug("Action_DrillingLoop invoked.");
            while (true) {
                // Debug("Starting drilling loop iteration.");
                StartAllDrills();
                var isExtending = ExtendPistonChain();
                // Debug($"isExtending: {isExtending}");
                if (!isExtending) yield break; // Stop the enumerator if the piston chain cannot extend further
                yield return true;
            }
        }

        IEnumerator<bool> Action_RunCommand(Action command) {
            // Debug($"Action_RunCommand invoked with command: {command}");
            // // Main(command, UpdateType.None);
            // Me.TryRun(command);
            command?.Invoke();
            yield return true;
        }

    }
}
