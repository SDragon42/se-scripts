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
        class StateMachine {
            readonly List<string> Keys2Remove = new List<string>();
            readonly Dictionary<string, IEnumerator<bool>> AllTasks = new Dictionary<string, IEnumerator<bool>>();

            public void RunAll() {
                foreach (var key in AllTasks.Keys) {
                    var task = AllTasks[key];
                    if (!task.MoveNext() || !task.Current)
                        Keys2Remove.Add(key);
                }

                while (Keys2Remove.Count > 0) {
                    var key = Keys2Remove[0];
                    Keys2Remove.RemoveAt(0);
                    Remove(key);
                }
            }

            public void Add(IEnumerator<bool> task) => Add(DateTime.Now.ToString(), task);
            public void Add(string key, IEnumerator<bool> task, bool replace = false) {
                var hasKey = AllTasks.ContainsKey(key);
                if (hasKey && !replace) return;
                if (hasKey && replace) Remove(key);
                AllTasks.Add(key, task);
            }
            public void Remove(string key) {
                if (!AllTasks.ContainsKey(key)) return;
                var task = AllTasks[key];
                task.Dispose();
                AllTasks.Remove(key);
            }

            public void RemoveAll() {
                foreach (var p in AllTasks) p.Value.Dispose();
                AllTasks.Clear();
            }

            public bool HasKey(string key) => AllTasks.ContainsKey(key);
        }
    }
}
