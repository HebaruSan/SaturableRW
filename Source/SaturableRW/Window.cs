using System.Collections.Generic;
using UnityEngine;

namespace SaturableRW
{
    [KSPAddon (KSPAddon.Startup.Flight, false)]

    class Window : MonoBehaviour
    {
        public static Window Instance
        {
            get; private set;
        }

        public Dictionary<string, VesselInfo> Vessels = new Dictionary<string, VesselInfo> ();

        Rect windowRect;

        public bool showWindow;

        public readonly Texture2D DischargeLabelIcon = GameDatabase.Instance.GetTexture ("Squad/PartList/SimpleIcons/fuels_monopropellant", false);

        void Start ()
        {
            InitWindow ();
        }

        void InitWindow ()
        {
            Instance = this;

            LoadConfig ();
        }

        void LoadConfig ()
        {
            if (SaturableRW.config == null)
            {
                SaturableRW.config = KSP.IO.PluginConfiguration.CreateForType<SaturableRW> ();
            }

            SaturableRW.config.load ();

            windowRect = SaturableRW.config.GetValue ("windowRect", new Rect (500, 500, 300, 0));

            SaturableRW.config ["windowRect"] = windowRect;
        }

        void OnDestroy ()
        {
            SaturableRW.config ["windowRect"] = windowRect;

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
            showWindow = GUILayout.Toggle (showWindow, "Close", GUI.skin.button);

            foreach (KeyValuePair<string, VesselInfo> ves in Vessels)
            {
                DrawVessel (ves.Value);
            }

            GUI.DragWindow ();
        }

        void DrawVessel (VesselInfo ves)
        {
            Color backgroundColour = GUI.backgroundColor;

            if (ves.Vessel == FlightGlobals.ActiveVessel)
            {
                GUI.backgroundColor = XKCDColors.Green;
            }

            ves.DisplayVes = GUILayout.Toggle (ves.DisplayVes, ves.Vessel.vesselName, GUI.skin.button);

            GUI.backgroundColor = backgroundColour;

            if (ves.DisplayVes)
            {
                bool state = GUILayout.Toggle (ves.ForcedActive, "Toggle Vessel Torque");

                if (state != ves.ForcedActive)
                {
                    ves.ForcedActive = state;

                    ModuleReactionWheel.WheelState stateToSet = state ? ModuleReactionWheel.WheelState.Active : ModuleReactionWheel.WheelState.Disabled;

                    foreach (SaturableRW rw in ves.Wheels)
                    {
                        rw.wheelRef.State = stateToSet;
                    }
                }

                GUILayout.BeginHorizontal ();

                GUILayout.Space (25);

                GUILayout.BeginVertical ();

                GUILayout.Space (15);

                foreach (SaturableRW rw in ves.Wheels)
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
                rw.bConsumeResource = GUILayout.Toggle (rw.bConsumeResource, DischargeLabelIcon, GUI.skin.button, GUILayout.Width (40));
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

            GUILayout.Space (15);
        }
    }
}
