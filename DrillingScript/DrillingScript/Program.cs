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
        List<IMyPistonBase> HPistons = new List<IMyPistonBase>();
        List<IMyPistonBase> VPistons = new List<IMyPistonBase>();
        IMyMotorAdvancedRotor mainRotor;
        List<IMyCargoContainer> stoneCargo = new List<IMyCargoContainer>();
        List<IMyShipDrill> drills = new List<IMyShipDrill>();
        double hPistonsStep = 0.5;
        double vPistonsStep = 0.5;
        int hPistonClock = 0;
        int globalCount = 0;
        int hPistonDrillDownDelay = 20;
        int hPistonRetractDelay = 10;
        bool hPistonsMaxReach = false;
        bool vPistonsMaxReach = false;
        List<IMyPistonBase>.Enumerator itHp;
        List<IMyPistonBase>.Enumerator itVp;
        public Program()
        {
            List<IMyTerminalBlock> pistonsList = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> cargoList = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonsList);
            if (pistonsList.Count == 0) throw new System.ArgumentException("No Connector detected at all...", "original"); ;
            string HPistonsKeyword = "H", VPistonsKeyword = "V", stoneCargoKeyword = "stone";
            pistonsList = FilterBySubstr(pistonsList, HPistonsKeyword);

            foreach (IMyPistonBase piston in pistonsList)
            {
                HPistons.Add(piston);
            }
            itHp = HPistons.GetEnumerator();

            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonsList);
            pistonsList = FilterBySubstr(pistonsList, VPistonsKeyword);
            foreach (IMyPistonBase piston in pistonsList)
            {
                VPistons.Add(piston);
            }
            itVp = VPistons.GetEnumerator();

            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoList);
            cargoList = FilterBySubstr(cargoList, stoneCargoKeyword);
            foreach (IMyCargoContainer cargo in cargoList)
            {
                stoneCargo.Add(cargo);
            }
            List<IMyMotorAdvancedRotor> rotors = new List<IMyMotorAdvancedRotor>();
            GridTerminalSystem.GetBlocksOfType(rotors);
            mainRotor = rotors.First();

            GridTerminalSystem.GetBlocksOfType(drills);
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // About 1.3 sec
            drillDown();
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            if (vPistonsMaxReach)
            {
                retractPiston(VPistons);
                vPistonsMaxReach = false;
                //retracting = true;
            }
            else updateVPistonStatus();
            globalCount++;
        }


        private void drillDown()
        {
            startDrills();
            extendPiston(HPistons);
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

        private void stopDrills()
        {
            foreach(IMyShipDrill drill in drills)
            {
                drill.Enabled = false;
            }
        }

        private void startDrills()
        {
            foreach (IMyShipDrill drill in drills)
            {
                drill.Enabled = true;
            }
        }

        private void retractPiston(List<IMyPistonBase> pistonsList)
        {
            foreach(IMyPistonBase piston in pistonsList)
            {
                piston.Retract();
            }

        }

        private void extendPiston(List<IMyPistonBase> pistonsList)
        {
            foreach (IMyPistonBase piston in pistonsList)
            {
                piston.Extend();
            }

        }

        private void moveHPiston()
        {
            stopDrills();
            hPistonClock++;
            //if (!hPistonsMaxReach)
            //{
            if (hPistonClock > 4) { itHp.Current.Enabled = false; }
            else
                itHp.Current.Enabled = true;
            //itHp.Current.ApplyAction("Stop"); //TODO hum... not sure.
                if (itHp.Current.Status.Equals(PistonStatus.Extended))
                {
                    if (!itHp.MoveNext()) //Reached the end if return false
                    {
                        itHp = HPistons.GetEnumerator();
                        hPistonsMaxReach = true;
                    }
                    else
                    {
                        itHp.Current.Extend();
                        hPistonClock = 0;
                    }
                }
                else
                {
                    itHp.Current.Extend();
                    hPistonClock = 0;
                }
        }

    private void updateVPistonStatus()
    {
        foreach (IMyPistonBase piston in HPistons)
        {
            if (!piston.Status.Equals(PistonStatus.Extended))
            {
                hPistonsMaxReach = false;
                break;
            }
        }
    }

    private void updateHPistonStatus() {
        foreach (IMyPistonBase piston in VPistons)
        {
            if (!piston.Status.Equals(PistonStatus.Extended))
            {
                vPistonsMaxReach = false;
                break;
            }
        }
    }

            //}

        //private void moveRotor(double degree) {

        //    //mainRotor. //TODO
        //}

    }
}
