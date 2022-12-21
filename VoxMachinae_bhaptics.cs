using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2CppVoxClient;

[assembly: MelonInfo(typeof(VoxMachinae_bhaptics.VoxMachinae_bhaptics), "VoxMachinae_bhaptics", "1.1.0", "Florian Fahrenberger")]
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
            public static void Postfix(VoxSound __instance, string playEventName)
            {
                float max_dist = 100.0f;
                if (!__instance.core.positionSet) return;
                float distance = (__instance.core.sourcePosition - __instance.transform.position).magnitude;
                if (playEventName == "FootImpact") return;
                if (playEventName.Contains("Muzzle")) return;
                if (playEventName.Contains("Reload")) return;
                if (playEventName.Contains("Play_mech_shutdown")) tactsuitVr.StopThreads();
                if (playEventName.Contains("FootStepResonance"))
                {
                    tactsuitVr.PlaybackHaptics("FootStep");
                    return;
                }
                switch (playEventName)
                {
                    case "EjectJet":
                        tactsuitVr.PlaybackHaptics("JumpJetRumble", 1.5f);
                        break;
                    case "BlastShieldClose":
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        tactsuitVr.StopThreads();
                        break;
                    case "BlastShieldOpen":
                        tactsuitVr.PlaybackHaptics("BellyRumble");
                        break;
                    case "OverheatAlarm":
                        tactsuitVr.PlaybackHaptics("HeartBeat");
                        tactsuitVr.StopThreads();
                        break;
                    case "CockpitRattle":
                        //tactsuitVr.LOG("Rattle");
                        tactsuitVr.PlaybackHaptics("BellyRumble", 0.1f);
                        break;
                    case "CockpitShake":
                        //tactsuitVr.LOG("Shake");
                        tactsuitVr.PlaybackHaptics("BellyRumble", 0.2f);
                        break;
                    /*
                    case "InternalFuelStart":
                        //tactsuitVr.LOG("FuelStart");
                        tactsuitVr.PlaybackHaptics("JumpJetRumble");
                        break;
                    case "InternalFuelStop":
                        //tactsuitVr.LOG("FuelStop");
                        tactsuitVr.PlaybackHaptics("JumpJetRumble");
                        break;
                    */
                    default:
                        //if (!playEventName.Contains("Explode") && !playEventName.Contains("Impact")) tactsuitVr.LOG("Play: " + playEventName);
                        break;
                }
                if (distance <= 0.01f) return;
                //tactsuitVr.LOG("Distances: " + __instance.transform.position.ToString() + " " + __instance.core.sourcePosition.ToString() + " " + distance.ToString());
                if (distance >= max_dist) return;
                float intensity = ((max_dist - distance) / max_dist) * ((max_dist - distance) / max_dist);
                if (playEventName.Contains("Impact"))
                {
                    //tactsuitVr.LOG("Impact: " + playEventName + " distance: " + distance.ToString());
                    tactsuitVr.PlaybackHaptics("BellyRumble", intensity);
                    return;
                }
                if (playEventName.Contains("Explode"))
                {
                    //tactsuitVr.LOG("Explode: " + playEventName + " distance: " + distance.ToString());
                    if (distance <= 5.0f) tactsuitVr.PlaybackHaptics("MechGetsHit");
                    tactsuitVr.PlaybackHaptics("ExplosionUp", intensity);
                    return;
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

        [HarmonyPatch(typeof(Cockpit), "ResetDamage", new Type[] { })]
        public class bhaptics_ResetDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Cockpit __instance)
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Cockpit), "UpdateForLaunch", new Type[] { })]
        public class bhaptics_LaunchUpdate
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(EntityInstance), "UpdateJumpJet", new Type[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
        public class bhaptics_UpdateJump
        {
            [HarmonyPostfix]
            public static void Postfix(EntityInstance __instance, bool jump)
            {
                if (__instance.uniqueId != myID) return;
                if (jump) tactsuitVr.StartJumpJet();
                else tactsuitVr.StopJumpJet();
            }
        }

    }
}
