using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Infinispeed
{
    [HarmonyPatch(typeof(Workable), nameof(Workable.GetEfficiencyMultiplier))]
    internal class Infinispeed
    {
        public static void Postfix(ref float __result)
        {
            __result = float.PositiveInfinity;
        }
    }
}
