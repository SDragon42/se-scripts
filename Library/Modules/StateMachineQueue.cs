// <mdk sortorder="1000" />
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
        class StateMachineQueue {
            readonly Queue<IEnumerator<bool>> TaskQueue = new Queue<IEnumerator<bool>>();

            IEnumerator<bool> CurrentTask;

            public bool HasTasks => TaskQueue.Count > 0;

            public void Add(IEnumerator<bool> task) => TaskQueue.Enqueue(task);

            public void Clear() {
                CurrentTask?.Dispose();
                CurrentTask = null;
                while (TaskQueue.Count > 0) {
                    var task = TaskQueue.Dequeue();
                    task.Dispose();
                }
            }

            public bool Run() {
                if (CurrentTask == null) {
                    if (TaskQueue.Count == 0) return false;
                    CurrentTask = TaskQueue.Dequeue();
                }
                if (CurrentTask.MoveNext()) return CurrentTask.Current;
                CurrentTask.Dispose();
                CurrentTask = null;
                return Run();
            }
        }
    }
}
