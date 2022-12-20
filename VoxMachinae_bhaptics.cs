using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2CppVoxClient;

[assembly: MelonInfo(typeof(VoxMachinae_bhaptics.VoxMachinae_bhaptics), "VoxMachinae_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("SpaceBulletDynamicsCorporation", "VoxMachinae")]


namespace VoxMachinae_bhaptics
{
    public class VoxMachinae_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;
        public static int myID = 0;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        
        [HarmonyPatch(typeof(VoxSound), "Play", new Type[] { typeof(string) })]
        public class bhaptics_PlaySound
        {
            [HarmonyPostfix]
            public static void Postfix(string playEventName)
            {
                if (playEventName == "FootImpact")
                {
                    tactsuitVr.PlaybackHaptics("FootStep");
                    return;
                }
                if (playEventName.Contains("Impact"))
                {
                    tactsuitVr.PlaybackHaptics("BellyRumble");
                    return;
                }
                if (playEventName.Contains("Explode"))
                {
                    tactsuitVr.PlaybackHaptics("BellyRumble");
                    return;
                }
                if (playEventName.Contains("Muzzle"))
                {
                    return;
                }
                if (playEventName.Contains("Reload"))
                {
                    return;
                }
                if (playEventName.Contains("FootStepResonance"))
                {
                    return;
                }
                switch (playEventName)
                {
                    case "EjectJet":
                        tactsuitVr.PlaybackHaptics("JumpJetRumble", 1.5f);
                        break;
                    case "MeleeImpact":
                        tactsuitVr.PlaybackHaptics("MechGetsHit");
                        break;
                    case "BlastShieldClose":
                        tactsuitVr.StopThreads();
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        break;
                    case "BlastShieldOpen":
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        break;
                    case "OverheatAlarm":
                        tactsuitVr.PlaybackHaptics("HeartBeat");
                        tactsuitVr.StopThreads();
                        break;
                    case "CockpitRattle":
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        break;
                    case "CockpitShake":
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        break;
                    case "InternalFuelStart":
                        tactsuitVr.StartJumpJet();
                        break;
                    case "InternalFuelStop":
                        tactsuitVr.StopJumpJet();
                        break;
                    default:
                        tactsuitVr.LOG("Play: " + playEventName);
                        break;
                }
            }
        }
        
        [HarmonyPatch(typeof(Weapon), "Fire", new Type[] { })]
        public class bhaptics_FireWeapon
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon __instance)
            {
                if (__instance.sourceEntityId != myID) return;
                tactsuitVr.Recoil(true);
                tactsuitVr.Recoil(false);
            }
        }

        [HarmonyPatch(typeof(Client), "EntityUpdate", new Type[] { typeof(EntityNetworkUpdate) })]
        public class bhaptics_EntityUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(Client __instance)
            {
                myID = __instance.mechId;
            }
        }

        [HarmonyPatch(typeof(Client), "Disconnect", new Type[] { typeof(int) })]
        public class bhaptics_Disconnect
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

    }
}
