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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        bool prevIsDocked, isDocked;
        List<IMyPistonBase> hangarDoorPistons;
        IMyShipConnector shipConnector;

        public Program()
        {
            List<IMyTerminalBlock> connectorList = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectorList);

            //This is bad
            shipConnector = connectorList[0] as IMyShipConnector;

            //TODO optimal
            //shipConnector = GridTerminalSystem.GetBlockWithName("*Hangar Connector*");
            //foreach (IMyShipConnector connector in connectorList){

            //}

            prevIsDocked = isDocked = shipConnector.Status.Equals(MyShipConnectorStatus.Connected);
            hangarDoorPistons = GridTerminalSystem.GetBlockGroupWithName("*Hangar Pistons*") as List<IMyPistonBase>;
            //GridTerminalSystem.SearchBlocksOfName("*Hangar Pistons*", hangarDoorPistons);
            //TODO test if best to auto-update
            //Runtime.UpdateFrequency = UpdateFrequency.Update100;

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (prevIsDocked)
            {
                if (!isDocked)
                {
                    switch (hangarDoorPistons[0].Status)
                    {
                        case PistonStatus.Extended:
                        case PistonStatus.Extending:
                            OpenHangarDoor();
                            break;

                        default:
                            break;
                   
                    }
                }
            }
            prevIsDocked = isDocked;
        }

        private void OpenHangarDoor()
        {
            foreach(IMyPistonBase piston in hangarDoorPistons)
            {
                piston.Retract();
            }
        }
    }
}
