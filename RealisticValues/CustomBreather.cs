using System;
using UnityEngine;

namespace RealisticValues
{
    class CustomBreather
    {
        public static void SpawnHotO2(Vector3 position, float mass, float initTemp)
        {
            int cell = Grid.CellAbove(Grid.PosToCell(position));
            SimMessages.AddRemoveSubstance(cell, SimHashes.Oxygen, CellEventLogger.Instance.OxygenModifierSimUpdate, mass, initTemp + 0.911547f, byte.MaxValue, 0);
            Console.WriteLine("Adding warmer oxygen: " + mass + "kg at " + initTemp + 0.911547f + "degrees Kelvin");
        }
    }
}
