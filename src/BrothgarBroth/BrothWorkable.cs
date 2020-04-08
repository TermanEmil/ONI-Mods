using Klei.AI;
using UnityEngine;

namespace BrothgarBroth
{
    public class BrothWorkable : Workable
    {
        public int basePriority = 20;
        private int ticks;

        public BrothWorkable()
        {
            Debug.Log("CONSTRUCTOR");
        }

        protected override void OnPrefabInit()
        {
            Debug.Log("Prefabinit");
            base.OnPrefabInit();
            overrideAnims = new[] {Assets.GetAnim("anim_interacts_juicer_kanim")};
            synchronizeAnims = false;
            SetWorkTime(300f);
        }

        /*protected override bool OnWorkTick(Worker worker, float dt)
        {
            ticks++;
            return ticks >= 300;
        }*/

        protected override void OnCompleteWork(Worker worker)
        {
            var effects = worker.GetComponent<Effects>();
            effects.Add(BrothEffects.BrothCooldownEffect, true);
            effects.Add(BrothEffects.BrothSpeedEffect, true);
            effects.Add(BrothEffects.BrothStaminaEffect, true);
            Object.Destroy(gameObject);
        }
    }
}
