using Klei.AI;
using UnityEngine;

namespace ShinebugReactor
{
    public static class SpawnUtility
    {
        public static GameObject SpawnShinebugEggShell(Vector3 position)
        {
            const float shinebugEggShellMass = 0.1f;

            var eggShell = Util.KInstantiate(Assets.GetPrefab("EggShell"), position);
            eggShell.GetComponent<PrimaryElement>().Mass = shinebugEggShellMass;
            eggShell.SetActive(true);

            return eggShell;
        }

        public static GameObject SpawnShinebugEgg(string eggName, Vector3 position)
        {
            var egg = GameUtil.KInstantiate(
                Assets.GetPrefab(eggName),
                Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Front),
                Grid.SceneLayer.Front);

            egg.SetActive(false);
            return egg;
        }
    }
}
