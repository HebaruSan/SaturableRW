using System.Collections.Generic;
using UnityEngine;

namespace SaturableRW
{
    [KSPAddon (KSPAddon.Startup.Flight, false)]

    class Window : MonoBehaviour
    {
        static Window instance;

        public static Window Instance
        {
            get 
            {
                return instance;
            }
        }

        public Dictionary<string, VesselInfo> Vessels = new Dictionary<string, VesselInfo>();

        Rect windowRect;

        public bool showWindow;

        void Start ()
        {
            InitWindow();
        }

        void InitWindow ()
        {
            instance = this;
            
            LoadConfig ();
        }

        void LoadConfig ()
        {
            if (SaturableRW.config == null)
            {
                SaturableRW.config = KSP.IO.PluginConfiguration.CreateForType<SaturableRW>();
            }

            SaturableRW.config.load ();

            windowRect = SaturableRW.config.GetValue ("windowRect", new Rect (500, 500, 300, 0));

            SaturableRW.config["windowRect"] = windowRect;
        }

        void OnDestroy ()
        {
            SaturableRW.config["windowRect"] = windowRect;

            SaturableRW.config.save ();
        }

        void OnGUI ()
        {
            if (showWindow)
            {
                windowRect = GUILayout.Window (573638, windowRect, DrawWindow, "Semi-Saturable Reaction Wheels", GUILayout.Height (0));
            }
        }

        void DrawWindow (int id)
        {
            showWindow &= !GUI.Button (new Rect (windowRect.width - 25, 5, 20, 20), "x");

            foreach (KeyValuePair<string, VesselInfo> ves in Vessels)
            {
                DrawVessel (ves.Value);
            }

            GUI.DragWindow ();
        }

        void DrawVessel (VesselInfo ves)
        {
            Color backgroundColour = GUI.backgroundColor;

            if (ves.vessel == FlightGlobals.ActiveVessel)
            {
                GUI.backgroundColor = XKCDColors.Green;
            }

            ves.displayVes = GUILayout.Toggle (ves.displayVes, ves.vessel.vesselName, GUI.skin.button);

            GUI.backgroundColor = backgroundColour;

            if (ves.displayVes)
            {
                bool state = GUILayout.Toggle (ves.forcedActive, "Toggle Vessel Torque");

                if (state != ves.forcedActive)
                {
                    ves.forcedActive = state;

                    ModuleReactionWheel.WheelState stateToSet = state ? ModuleReactionWheel.WheelState.Active : ModuleReactionWheel.WheelState.Disabled;

                    foreach (SaturableRW rw in ves.wheels)
                    {
                        rw.wheelRef.State = stateToSet;
                    }
                }

                GUILayout.BeginHorizontal ();

                GUILayout.Space (20);

                GUILayout.BeginVertical ();

                foreach (SaturableRW rw in ves.wheels)
                {
                    DrawWheel (rw);
                }

                GUILayout.EndVertical ();

                GUILayout.EndHorizontal ();
            }
        }

        void DrawWheel (SaturableRW rw)
        {
            GUILayout.BeginHorizontal ();

            Color backgroundColour = GUI.backgroundColor;

            if (rw.wheelRef.State == ModuleReactionWheel.WheelState.Active)
            {
                    GUI.backgroundColor = XKCDColors.Green;
            }

            bool tmp = GUILayout.Toggle (rw.drawWheel, rw.part.partInfo.title, GUI.skin.button);

            GUI.backgroundColor = backgroundColour;

            if (tmp != rw.drawWheel)
            {
                if (Event.current.button == 0)
                {
                    rw.drawWheel = tmp;
                }
                else if (Event.current.button == 1)
                {
                    rw.wheelRef.State = rw.wheelRef.State == ModuleReactionWheel.WheelState.Disabled ? ModuleReactionWheel.WheelState.Active : ModuleReactionWheel.WheelState.Disabled;
                }
            }

            if (rw.canForceDischarge)
            {
                rw.bConsumeResource = GUILayout.Toggle (rw.bConsumeResource, "", GUI.skin.button, GUILayout.Width(40));
            }

            GUILayout.EndHorizontal ();

            if (!rw.drawWheel)
            {
                return;
            }
     
            GUILayout.Label ("<b>Axis</b>\t<b>Available</b>\t<b>Max</b>");

            GUILayout.Label (string.Format ("{0}\t{1:0.0}kN\t{2:0.0}kN", "Pitch", rw.availablePitchTorque, rw.maxPitchTorque));
            GUILayout.Label (string.Format ("{0}\t{1:0.0}kN\t{2:0.0}kN", "Yaw", rw.availableYawTorque, rw.maxYawTorque));
            GUILayout.Label (string.Format ("{0}\t{1:0.0}kN\t{2:0.0}kN", "Roll", rw.availableRollTorque, rw.maxRollTorque));
        }
    }
}
