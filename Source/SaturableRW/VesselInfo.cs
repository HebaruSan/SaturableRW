using System.Collections.Generic;

namespace SaturableRW
{
    // Global vars for a vessel and a list of all RW's.

    public class VesselInfo
    {
        public Vessel Vessel
        {
            get; set;
        }

        public bool ForcedActive
        {
            get; set;
        }

        public List<SaturableRW> Wheels
        {
            get; set;
        }

        public bool DisplayVes
        {
            get; set;
        }

        public VesselInfo (Vessel ves, bool active)
        {
            Vessel = ves;

            ForcedActive = active;

            Wheels = new List<SaturableRW> ();
        }
    }
}
