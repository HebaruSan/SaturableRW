namespace SaturableRW
{
    public class MomentumDischargeThruster : ModuleRCS
    {
        public override string GetInfo ()
        {
            string baseInfo = base.GetInfo ();

            int index = baseInfo.IndexOf ("<color=#99ff00ff><b>Requires:</b></color>", System.StringComparison.Ordinal);

            string resourceRates = baseInfo.Substring (index);

            return string.Format ("Thruster used to remove accumulated momentum from a RW.\r\n<b>Discharge Rate:</b> {0}% / s\r\n\r\n{1}", (thrusterPower * 100).ToString ("0.0"), resourceRates);
        }

        public override void OnAwake ()
        {
            if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
            {
                // Hide the "RCS ISP" field.

                foreach (BaseField f in Fields)
                {
                    f.guiActive = false;
                    f.guiActiveEditor = false;
                }

                // Hide the "Disable RCS port" button.

                foreach (BaseEvent e in Events)
                {
                    e.guiActive = false;
                    e.guiActiveEditor = false;
                    e.guiActiveUnfocused = false;
                }
            }

            base.OnAwake ();
        }
    }
}
