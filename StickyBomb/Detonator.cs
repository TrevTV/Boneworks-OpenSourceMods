using System.Collections;
using UnityEngine;
using StressLevelZero.Pool;
using WNP78.Grenades;
using StressLevelZero.Interaction;

namespace StickyBomb
{
    public class Detonator : MonoBehaviour
    {
        public Detonator(System.IntPtr ptr) : base(ptr) { }

        public Grip grip;

        private bool isDetonating;

        private int clicked = 0;
        private float clicktime = 0;
        private const float clickdelay = 0.5f;

        public void Start()
        {
            grip = transform.parent.Find("Colliders").Find("gripPoint").GetComponent<Grip>();
        }

        public void Update()
        {
            if (grip.attachedHands.Count == 0) return;

            if (grip.attachedHands[0].controller.GetPrimaryInteractionButtonDown())
            {
                clicked++;
                if (clicked == 1) clicktime = Time.time;

                if (clicked > 1 && Time.time - clicktime < clickdelay)
                {
                    clicked = 0;
                    clicktime = 0;

                    ExplodeAll();
                }
                else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
            }
        }

        public void OnTriggerEnter(Collider col)
        {
            if (grip.attachedHands.Count == 0) return;

            if (grip.attachedHands[0].otherHand == col.gameObject.GetComponentInParent<Hand>())
                ExplodeAll();
        }

        public void ExplodeAll()
        {
            if (isDetonating) return;
            MelonLoader.MelonCoroutines.Start(CoExplodeAll());
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        private IEnumerator CoExplodeAll()
        {
            isDetonating = true;
            var poolees = PoolManager.GetPool("Pipe Bomb")._spawnedObjects;
            foreach (Poolee poolee in poolees)
            {
                try
                {
                    Grenade nade = poolee.GetComponent<Grenade>();
                    nade.explosion.Explode();
                    poolee.GetComponent<StickyBombHandler>().Disconnect();
                }
                catch { }
                yield return new WaitForSeconds(1);
            }
            isDetonating = false;
        }
    }
}
