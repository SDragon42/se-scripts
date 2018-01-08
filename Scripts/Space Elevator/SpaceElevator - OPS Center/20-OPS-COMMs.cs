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

        //-------------------------------------------------------------------------------
        //  COMMs
        //-------------------------------------------------------------------------------
        void SendCOMMs_DisplayUpdate(string carriageKey, string displayKey, string text) {
            if (_antenna == null) return;
            var msgPayload = new UpdateDisplayMessage(carriageKey, displayKey, text);
            _comms.AddMessageToQueue(msgPayload);
        }

        void SendAllCOMMsDisplays() {
            //var currText = GetDisplayText(carriageKey, displayKey);
            //if (string.Compare(currText, text) == 0) return;
            //var cKey = MakeDisplayKey(carriageKey, displayKey);
            //_displayText[cKey] = text;
            //SendCOMMs_DisplayUpdate(carriageKey, displayKey, _displayText[cKey]);

            foreach (var key in _displayText.Keys) {
                var parts = key.Split('|');
                if (parts.Length != 2) continue;
                SendCOMMs_DisplayUpdate(parts[0], parts[1], _displayText[key]);
            }
        }

        void SendCarriageTo(string carriageKey, string destination) {
            if (_antenna == null) return;
            var msgPayload = new SendCarriageToMessage(destination);
            _comms.AddMessageToQueue(msgPayload, carriageKey);
        }

    }
}
