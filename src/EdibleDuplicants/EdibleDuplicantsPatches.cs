using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaiLib;

namespace EdibleDuplicants
{
    class EdibleDuplicantsPatches
    {
        public static void OnLoad()
        {
            CaiLib.Utils.BuildingUtils.AddBuildingToPlanScreen(CaiLib.Utils.GameStrings.PlanMenuCategory.Food, ButcherTableConfig.Id);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.NAME", ButcherTableConfig.DisplayName);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.EFFECT", "EFFECT");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.DESC", ButcherTableConfig.Description);
        }
    }
}
