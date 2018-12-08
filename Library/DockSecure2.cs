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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        enum DockSecureActions {
            OnOff, On, Off
        }

        class DockSecure2 {
            public Action<string> Debug = (text) => { };

            readonly List<IMyFunctionalBlock> ToggleBlocks = new List<IMyFunctionalBlock>();
            readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
            readonly List<IMyShipConnector> Connectors = new List<IMyShipConnector>();


            bool loadTypeCollects = true;
            delegate bool CollectMethod(IMyFunctionalBlock b);
            readonly List<CollectMethod> TurnOffCollects = new List<CollectMethod>();
            readonly List<CollectMethod> TurnOnCollects = new List<CollectMethod>();

            readonly Dictionary<string, DockSecureActions> BlockTypes = new Dictionary<string, DockSecureActions>();

            MyGridProgram gridProgram;
            bool wasLockedLastRun = false;
            bool isLocked = false;

            public bool IsDocked { get; private set; }

            public string IgnoreTag { get; set; }
            public bool Auto_On { get; set; }
            public bool Auto_Off { get; set; }

            public void Init(MyGridProgram gridProgram, bool findBlocks = true) {
                this.gridProgram = gridProgram;
                if (!findBlocks) return;
                this.gridProgram.GridTerminalSystem.GetBlocksOfType(LandingGears, this.IsNotIgnored);
                this.gridProgram.GridTerminalSystem.GetBlocksOfType(Connectors, this.IsNotIgnored);
            }
            public void Clear() {
                BlockTypes.Clear();
                loadTypeCollects = true;
            }
            public void Set(string key, DockSecureActions action) {
                if (BlockTypes.ContainsKey(key))
                    BlockTypes[key] = action;
                else BlockTypes.Add(key, action);
                loadTypeCollects = true;
            }
            public void Remove(string key) {
                if (BlockTypes.ContainsKey(key)) {
                    BlockTypes.Remove(key);
                    loadTypeCollects = true;
                }
            }

            public void Dock() {
                LandingGears.ForEach(b => b.Lock());
                Connectors.ForEach(b => b.Connect());
                CheckIfLocked();
                if (isLocked) {
                    TurnOffSystems();
                    IsDocked = true;
                }
            }
            public void UnDock() {
                TurnOnSystems();
                LandingGears.ForEach(b => b.Unlock());
                Connectors.ForEach(b => b.Disconnect());
                isLocked = false;
                IsDocked = false;
            }
            public void AutoToggleDock() {
                CheckIfLocked();
                if (wasLockedLastRun == isLocked) return;
                wasLockedLastRun = isLocked;

                if (isLocked) {
                    if (Auto_Off) {
                        TurnOffSystems();
                        IsDocked = true;
                    }
                } else {
                    if (Auto_On)
                        TurnOnSystems();
                    IsDocked = false;
                }
            }
            public void ToggleDock() {
                CheckIfLocked();
                if (isLocked)
                    UnDock();
                else
                    Dock();
            }

            void TurnOffSystems() {
                //Debug("  TurnOffSystems()");
                LoadTypeLists();
                ToggleSystems(gridProgram, ToggleBlocks, TurnOffCollects, false);
            }
            void TurnOnSystems() {
                //Debug("  TurnOnSystems()");
                LoadTypeLists();
                ToggleSystems(gridProgram, ToggleBlocks, TurnOnCollects, true);
            }

            void ToggleSystems(MyGridProgram gridProgram, List<IMyFunctionalBlock> blockList, List<CollectMethod> blockTypeCollects, bool enabled) {
                gridProgram.GridTerminalSystem.GetBlocksOfType(blockList, b => {
                    if (!Program.OnSameGrid(gridProgram.Me, b)) return false;
                    return blockTypeCollects.Any(t => t(b));
                });
                //Debug($"  # blocks: {blockList.Count}");
                blockList.ForEach(b => b.Enabled = enabled);
            }

            void CheckIfLocked() {
                isLocked = Connectors.Where(Collect.IsConnectorConnected).Any();
                if (isLocked) return;
                isLocked = LandingGears.Where(Collect.IsLandingGearLocked).Any();
            }

            void LoadTypeLists() {
                if (!loadTypeCollects) return;
                //Debug("  LoadTypeLists()");
                LoadTypeList(TurnOffCollects, kvp => kvp.Value == DockSecureActions.Off || kvp.Value == DockSecureActions.OnOff);
                LoadTypeList(TurnOnCollects, kvp => kvp.Value == DockSecureActions.On || kvp.Value == DockSecureActions.OnOff);
            }
            void LoadTypeList(List<CollectMethod> typeList, Func<KeyValuePair<string, DockSecureActions>, bool> collect) {
                var q = BlockTypes.Where(collect)
                    .Select(kvp => kvp.Key)
                    .Distinct()
                    .ToArray();
                //Debug($"  LoadTypeList()  {q.Length}");

                typeList.Clear();
                foreach (var t in q) {
                    var method = getCollectMethod(t);
                    //if (method == null)
                    //    Debug($"    {t} : NULL");
                    if (method != null) typeList.Add(method);
                }
                //Debug($"  typeList - {q.Length}");
            }

            static CollectMethod getCollectMethod(string t) {
                switch (t) {
                    case "IMyGyro": return (b) => b is IMyGyro;
                    case "IMyThrust": return b => b is IMyThrust;
                    case "IMyRadioAntenna": return (b) => b is IMyRadioAntenna;
                    case "IMyLaserAntenna": return (b) => b is IMyLaserAntenna;
                    case "IMyBeacon": return (b) => b is IMyBeacon;
                    case "IMyInteriorLight": return (b) => b is IMyInteriorLight;
                    case "IMySensorBlock": return (b) => b is IMySensorBlock;
                    case "IMyOreDetector": return (b) => b is IMyOreDetector;
                    case "IMyReflectorLight": return (b) => b is IMyReflectorLight;
                    case "IMyConveyorSorter": return (b) => b is IMyConveyorSorter;
                    default: return null;
                }
            }


            bool IsNotIgnored(IMyTerminalBlock b) => OnSameGrid(gridProgram.Me, b) && !Collect.IsTagged(b, IgnoreTag);

        }
    }
}
