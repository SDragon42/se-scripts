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
    partial class Program : MyGridProgram {
        /* Modify the following values as needed: */

        /* To move the focus point, middle value is forward(+)/backward(-) */
        Vector3D AnchorOffset = new Vector3D(0.0, -5.0, 0.0);
        /* Friction when going straight and of inside wheels when sliding */
        const float FrictionInside = 50.0f;
        /* Friction of outside wheels when sliding */
        const float FrictionOutside = 8.0f;
        /* Friction in case of braking when going straight and of inside wheels when sliding */
        const float FrictionBrakInside = 70.0f;
        /* Friction in case of braking of rear outside wheel(s) when sliding */
        const float FrictionBrakOutside = 15.0f;
        /* Above this sideways speed [m/s] the vehicle is considered sliding */
        const float SlideSpeedLimit = 5.0f;

        /* Don't modify lines below (or at own risk) */
        const bool debug = true;
        const double secondsElapsedInv = 60.0;

        WheelController wheelController;
        LinearSpeed speedInfo;

        static MyGridProgram GP;

        public void Main(string argument) {
            List<IMyTerminalBlock> wheels = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(wheels,
                x => (x.CubeGrid == Me.CubeGrid) && (x.IsWorking));
            if (wheelController == null || argument == "reset" || wheelController.WheelAdded(wheels.Count)) {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                GP = this;
                GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(blocks, x => (x.CubeGrid == Me.CubeGrid));

                // Lets initialize an example wheel controller
                wheelController = new WheelController(
                        blocks.Count == 0 ? Me : blocks[0],
                        wheels,
                        2f,
                        4f,
                        AnchorOffset
                    );
            }
            if (speedInfo == null) speedInfo = new LinearSpeed(wheelController.Anchor);
            speedInfo.Update();
            wheelController.Update(speedInfo.curForwardSpd, speedInfo.curLeftSpd);
        }

    }
}
