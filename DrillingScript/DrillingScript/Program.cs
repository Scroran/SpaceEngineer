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
        IMyMotorAdvancedStator headRotor;
        string rotorKeyword = "rotorHead";

        string drillKeyword = "drillHead";
        List<IMyShipDrill> headDrills = new List<IMyShipDrill>();

        Boolean isRevolving = false;
        float initialRotorRad;
        float revolutionSpeed = 0.01;

        public Program()
        {
            headDrills = ListDrillsMatching(drillsKeyword);
            headRotor = ListRotorsMatching(rotorKeyword).First;

            ValidateInitialState();


            GridTerminalSystem.GetBlocksOfType(drills);
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // About 1.3 sec
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (isRevolving){
                if (IsRevolutionComplete()){
                    StopDrills();
                    StopRevolution();
                }
            }
            else{
                InitiateRevolution();
                StartDrills();
            }
        }

        private Boolean IsRevolutionComplete(){
            return initialRotorRad == headRotor.Angle;
        }
        
        private void StartDrills(){
            foreach(IMyShipDrill drill in headDrills)
            {
                drill.Enabled = true;
            }
        }

        private void StopDrills(){
            foreach(IMyShipDrill drill in headDrills)
            {
                drill.Enabled = false;
            }
        }

        private void InitiateRevolution(){
            initialRotorRad = headRotor.Angle;

            headRotor.TargetVelocityRPM = revolutionSpeed
            isRevolving = true;
        }

        private void StopRevolution(){
            isRevolving = false;
            headRotor.TargetVelocityRPM = 0;
        }

        private void ValidateInitialState(){
            if(headRotor == null) throw new System.ArgumentException("No head rotor located");
            if(headDrills.Count != 0) throw new System.ArgumentException("No head drills located");
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

        private List<IMyMotorAdvancedStator> ListRotorsMatching(string keyword){
            List<IMyMotorAdvancedStator> rotors = new List<IMyMotorAdvancedStator>();
            GridTerminalSystem.GetBlocksOfType(rotors);

            return FilterBySubstr(rotors, keyword);
        } 

        private List<IMyShipDrill> ListDrillsMatching(string keyword){
            List<IMyShipDrill> drills = new List<IMyShipDrill>();
            GridTerminalSystem.GetBlocksOfType(drills);

            return FilterBySubstr(drills, keyword);
        } 
}
