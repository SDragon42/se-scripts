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
        class StateMachine<T> {
            readonly List<string> Keys2Remove = new List<string>();
            readonly Dictionary<string, IEnumerator<T>> AllTasks = new Dictionary<string, IEnumerator<T>>();
            readonly Func<T, bool> ContinueCheck;

            public StateMachine(Func<T, bool> continueMethod = null) {
                ContinueCheck = (continueMethod != null)
                    ? continueMethod
                    : (r) => false;
            }

            public void RunAll() {
                if (!HasTasks) return;
                foreach (var key in AllTasks.Keys) RunTask(key);
                RemoveKeys();
            }
            public T Run(string key) {
                if (!HasTask(key)) return default(T);
                var result = RunTask(key);
                RemoveKeys();
                return result;
            }
            
            public void Add(IEnumerator<T> task) => Add(DateTime.Now.ToString(), task);
            public void Add(string key, IEnumerator<T> task, bool replace = false) {
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



            private T RunTask(string key) {
                var task = AllTasks[key];
                if (!task.MoveNext()) {
                    Keys2Remove.Add(key);
                    return default(T);
                }
                if (!ContinueCheck(task.Current))
                    Keys2Remove.Add(key);
                return task.Current;
            }
            private void RemoveKeys() {
                while (Keys2Remove.Count > 0) {
                    var key = Keys2Remove[0];
                    Keys2Remove.RemoveAt(0);
                    Remove(key);
                }
            }
        }
    }
}
