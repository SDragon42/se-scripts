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
        class StateMachineSets {
            readonly List<string> Keys2Remove = new List<string>();
            readonly Dictionary<string, StateMachineQueue> AllTasks = new Dictionary<string, StateMachineQueue>();

            public bool HasTask(string key) => AllTasks.ContainsKey(key);
            public bool HasTasks => AllTasks.Count > 0;


            public void RunAllTasks() {
                if (!HasTasks) return;
                foreach (var key in AllTasks.Keys) RunTask2(key);
                RemoveCompleted();
            }
            public bool RunTask(string key) => HasTask(key) ? RunTask2(key) : false;
            bool RunTask2(string key) {
                var result = RunTask(key, AllTasks[key]);
                RemoveCompleted();
                return result;
            }

            public void Add(string key, IEnumerator<bool> task, bool replace = false) {
                if (!HasTask(key)) AllTasks.Add(key, new StateMachineQueue());
                var sm = AllTasks[key];
                if (replace) sm.Clear();
                sm.Add(task);
            }
            public void Remove(string key) {
                if (!HasTask(key)) return;
                var sm = AllTasks[key];
                sm.Clear();
                AllTasks.Remove(key);
            }
            public void Clear() {
                foreach (var p in AllTasks) p.Value.Clear();
                AllTasks.Clear();
            }



            void RemoveCompleted() {
                while (Keys2Remove.Count > 0) {
                    var key = Keys2Remove[0];
                    Keys2Remove.RemoveAt(0);
                    Remove(key);
                }
            }

            bool RunTask(string key, StateMachineQueue task) {
                var result = task.Run();
                if (!result) Keys2Remove.Add(key);
                return result;
            }
        }
    }
}
