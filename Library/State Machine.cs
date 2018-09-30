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
        class StateMachine<T> {
            readonly List<string> Keys2Remove = new List<string>();
            readonly List<string> Keys = new List<string>();
            readonly Dictionary<string, IEnumerator<T>> AllTasks = new Dictionary<string, IEnumerator<T>>();

            public void RunAll() {
                if (!HasTasks) return;
                var i = 0;
                while (i < Keys.Count) {
                    RunTask(Keys[i++]);
                }
                RemoveCompleted();
            }
            public T Run(string key) {
                if (!HasTask(key)) return default(T);
                var result = RunTask(key);
                RemoveCompleted();
                return result;
            }
            
            public void Add(string key, IEnumerator<T> task, bool replace = false) {
                var hasKey = HasTask(key);
                if (hasKey && !replace) return;
                if (hasKey && replace) Remove(key);
                AllTasks.Add(key, task);
                if (!hasKey) Keys.Add(key);
            }
            public void Remove(string key) {
                if (Keys.Contains(key)) Keys.Remove(key);
                if (!HasTask(key)) return;
                var task = AllTasks[key];
                task.Dispose();
                AllTasks.Remove(key);
            }

            public void Clear() {
                foreach (var p in AllTasks) p.Value.Dispose();
                AllTasks.Clear();
                Keys.Clear();
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



            T RunTask(string key) {
                var task = AllTasks[key];
                if (!task.MoveNext()) {
                    Keys2Remove.Add(key);
                    return default(T);
                }
                return task.Current;
            }
        }
    }
}
