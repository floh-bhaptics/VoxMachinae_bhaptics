using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
[assembly: MelonInfo(typeof(VoxMachinae_bhaptics.VoxMachinae_bhaptics), "VoxMachinae_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("SpaceBulletDynamicsCorporation", "VoxMachinae")]


namespace VoxMachinae_bhaptics
{
    public class VoxMachinae_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        /*
        [HarmonyPatch(typeof(VertigoPlayer), "Die", new Type[] { })]
        public class bhaptics_PlayerDies
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }
        */
    }
}
