using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using KMod;
using Steamworks;

namespace DuplicantIntelligenceTester
{
    [HarmonyPatch(typeof(Steam), "UpdateMods")]
    public class Crash
    {
        public static void Postfix(IEnumerable<PublishedFileId_t> removed)
        {
            if(removed.)
            Sim.SIM_DebugCrash();
        }
    }
}