using ModThatIsNotMod;
using MelonLoader;
using StressLevelZero.Props.Weapons;
using StressLevelZero.SFX;
using System;
using UnityEngine;

namespace BetterDualwielding
{
    public class PouchScript : MonoBehaviour
    {
        public PouchScript(IntPtr intPtr) : base(intPtr) { }

        private void OnTriggerEnter(Collider other)
        {
            if (!Main.enabled)
                return;

            try
            {
                if (other.gameObject.GetComponentInParent<Gun>())
                {
                    var gun = other.gameObject.GetComponentInParent<Gun>();

                    if (gun.triggerGrip.attachedHands == null)
                        return;

                    if (gun.magazineSocket.GetMagazine() == null)
                        return;

                    if (gun.magazineSocket.GetMagazine().GetAmmoCount() == 0)
                    {
                        var before = gun.magazineSocket.GetMagazine().GetAmmoCount();

                        gun.magazineSocket.GetMagazine().SetAmmoCount(gun.magazineSocket.GetMagazine().magazineData.AmmoSlots.Length);

                        var clip = gun.GetComponent<GunSFX>().magazineInsert[UnityEngine.Random.Range(0, gun.gunSFX.magazineInsert.Length - 1)];
                        if (clip != null && gun.magazineSocket.GetMagazine().GetAmmoCount() > before)
                        {
                            Main.audioSource.clip = clip;
                            Main.audioSource.Play();
                        }
                    }
                }
            }
            catch { }
        }
    }
}