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
        class StateMachine {
            readonly List<string> Keys2Remove = new List<string>();
            readonly Dictionary<string, IEnumerator<bool>> AllTasks = new Dictionary<string, IEnumerator<bool>>();

            public void RunAll() {
                if (!HasTasks) return;
                foreach (var task in AllTasks) RunTask(task.Key, task.Value);
                RemoveCompleted();
            }
            public bool Run(string key) {
                if (!HasTask(key)) return false;
                var result = RunTask(key, AllTasks[key]);
                RemoveCompleted();
                return result;
            }

            public void Add(string key, IEnumerator<bool> task, bool replace = false) {
                var hasKey = HasTask(key);
                if (hasKey && !replace) return;
                if (hasKey && replace) Remove(key);
                AllTasks.Add(key, task);
            }
            public void Remove(string key) {
                if (!HasTask(key)) return;
                var task = AllTasks[key];
                task.Dispose();
                AllTasks.Remove(key);
            }

            public void Clear() {
                foreach (var p in AllTasks) p.Value.Dispose();
                AllTasks.Clear();
            }

            public bool HasTask(string key) => AllTasks.ContainsKey(key);
            public bool HasTasks => AllTasks.Count > 0;



            void RemoveCompleted() {
                while (Keys2Remove.Count > 0) {
                    var key = Keys2Remove[0];
                    Keys2Remove.RemoveAt(0);
                    Remove(key);
                }
            }



            bool RunTask(string key, IEnumerator<bool> task) {
                if (!task.MoveNext()) {
                    Keys2Remove.Add(key);
                    return false;
                }
                return task.Current;
            }
        }
    }
}
