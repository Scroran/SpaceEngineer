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
        List<IMyPistonBase> hangarDoorPistons = new List<IMyPistonBase>();
        List<IMyShipConnector> shipHangarConnectors = new List<IMyShipConnector>();
        List<bool> prevConnectorState = new List<bool>();


        //On launch
        public Program()
        {
            List<IMyTerminalBlock> connectorList = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectorList);
            string hangarKeyword = "Hangar";
            if (connectorList.Count == 0) throw new System.ArgumentException("No Connector detected at all...", "original"); ;

            connectorList = FilterBySubstr(connectorList, hangarKeyword);
            if (connectorList.Count == 0) throw new System.ArgumentException("No Connector with tag *Hangar*", "original"); ;

            foreach (IMyShipConnector connector in connectorList)
            {
                shipHangarConnectors.Add(connector);
                prevConnectorState.Add(connector.Status.Equals(MyShipConnectorStatus.Connected));
            }
            if (shipHangarConnectors.Count != 0)
                prevIsDocked = isDocked = shipHangarConnectors[0].Status.Equals(MyShipConnectorStatus.Connected);
            else prevIsDocked = isDocked = false;
            List<IMyTerminalBlock> pistonList = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonList);

            pistonList = FilterBySubstr(pistonList, hangarKeyword);

            foreach (IMyPistonBase piston in pistonList)
            {
                hangarDoorPistons.Add(piston);
            }

            //TODO test if best to auto-update
            //Runtime.UpdateFrequency = UpdateFrequency.Update100;

        }
 
        public void Main(string argument, UpdateType updateSource)
        {
            if (IsAnyShipUndocking() && hangarDoorPistons.Count != 0)
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
            prevIsDocked = isDocked;
        }
        
        //@Post : update prevConnectorState
        private bool IsAnyShipUndocking()
        {
            bool AShipIsUndocking = false;
            bool checkedConnectorConnected = false;
            for (int i = 0; i< shipHangarConnectors.Count; i++)
            {
                checkedConnectorConnected = shipHangarConnectors[i].Status.Equals(MyShipConnectorStatus.Connected);
                if (prevConnectorState[i] != checkedConnectorConnected && !checkedConnectorConnected)
                {
                    AShipIsUndocking = true;
                }
                prevConnectorState[i] = checkedConnectorConnected;
            }
            return AShipIsUndocking;
        }

        private List<IMyTerminalBlock> FilterBySubstr(List<IMyTerminalBlock> blockList, string subs)
        {

            List<IMyTerminalBlock> filteredList = new List<IMyTerminalBlock>();
            foreach (IMyTerminalBlock block in blockList)
            {
                if (block.DisplayNameText.Contains(subs))
                {
                    filteredList.Add(block);

                }
            }
            return filteredList;
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
