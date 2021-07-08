using ModThatIsNotMod;
using MelonLoader;
using StressLevelZero.Pool;
using StressLevelZero.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StressLevelZero.Interaction;

namespace Rewind
{
    public static class BuildInfo
    {
        public const string Name = "Rewind";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.1.0";
        public const string DownloadLink = null;
    }

    public class Rewind : MelonMod
    {
        public static List<TimeBody> bodies = new List<TimeBody>();
        private Material vignetterMat;
        private MeshRenderer vignetterRend;

        private float holdTime = 2.0f;
        private float startTime = 0f;

        public override void OnApplicationStart()
        {
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<TimeBody>();
            Hooking.OnGrabObject += OnPlayerGrabObject;
        }

        private void OnPlayerGrabObject(GameObject obj, Hand hand)
        {
            Rigidbody rb = obj.GetComponentInParent<Rigidbody>();
            if (rb)
                AddRBToRewind(rb);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonCoroutines.Start(FindRigidbodies());

            GameObject newVignetter = GameObject.Instantiate(GameObject.Find("Vignetter"), Player.GetRigManager().transform);
            MeshRenderer renderer = newVignetter.GetComponent<MeshRenderer>();
            Material newMaterial = GameObject.Instantiate(renderer.material);
            newMaterial.color = Color.black;
            renderer.material = newMaterial;
            vignetterMat = newMaterial;
            vignetterRend = renderer;
        }

        public IEnumerator FindRigidbodies()
        {
            yield return new WaitForSeconds(5);
            bodies.Clear();
            foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
                AddRBToRewind(rb);

            foreach (var dynamicPool in PoolManager.DynamicPools)
            {
                Pool pool = dynamicPool.value;
                for (int i = 0; i < pool._pooledObjects.Count; i++)
                {
                    GameObject obj = pool._pooledObjects[i].gameObject;
                    Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();
                    for (int o = 0; o < rbs.Length; o++)
                        AddRBToRewind(rbs[o], true);
                }
            }
        }

        private static TimeBody AddRBToRewind(Rigidbody rb, bool isFromPool = false)
        {
            if (!rb.gameObject.GetComponentInParent<RigManager>() && rb.gameObject.GetComponent<TimeBody>() == null)
            {
                TimeBody body = rb.gameObject.AddComponent<TimeBody>();
                if (rb.isKinematic)
                    body.isKinematic = true;
                bodies.Add(body);
                return body;
            }

            return null;
        }

        public override void OnUpdate()
        {
            if (Player.rightHand == null || Player.leftHand == null) return;

            if (Player.rightHand.controller.GetAButtonDown())
                startTime = Time.time;

            if (Player.rightHand.controller.GetAButton())
                if (startTime + holdTime <= Time.time)
                    StartAllBodies();

            if (Player.rightHand.controller.GetAButtonUp())
            {
                startTime = 0f;
                StopAllBodies();
            }
        }

        public void StartAllBodies()
        {
            MelonCoroutines.Start(ToggleVignetter(true));

            foreach (TimeBody body in bodies)
            {
                try
                {
                    if (body != null && body.gameObject.activeInHierarchy)
                        body.StartRewinding();
                }
                catch { }
            }
        }

        public void StopAllBodies()
        {
            MelonCoroutines.Start(ToggleVignetter(false));

            foreach (TimeBody body in bodies)
            {
                try
                {
                    if (body != null && body.gameObject.activeInHierarchy)
                        body.StopRewinding();
                }
                catch { }
            }
        }

        public IEnumerator ToggleVignetter(bool enable)
        {
            if (enable)
            {
                vignetterRend.enabled = true;
                while (!(vignetterMat.GetFloat("_Inner") >= 0.9f))
                {
                    vignetterMat.SetFloat("_Inner", Mathf.Lerp(vignetterMat.GetFloat("_Inner"), 1f, 0.1f));
                    vignetterMat.SetFloat("_Outer", Mathf.Lerp(vignetterMat.GetFloat("_Outer"), 1f, 0.1f));
                    yield return new WaitForEndOfFrame();
                }
                vignetterMat.SetFloat("_Inner", 1);
                vignetterMat.SetFloat("_Outer", 1);
            }
            else
            {
                while (!(vignetterMat.GetFloat("_Inner") <= 0.1f))
                {
                    vignetterMat.SetFloat("_Inner", Mathf.Lerp(vignetterMat.GetFloat("_Inner"), 0f, 0.1f));
                    vignetterMat.SetFloat("_Outer", Mathf.Lerp(vignetterMat.GetFloat("_Outer"), 0f, 0.1f));
                    yield return new WaitForEndOfFrame();
                }
                vignetterMat.SetFloat("_Inner", 0);
                vignetterMat.SetFloat("_Outer", 0);
                vignetterRend.enabled = false;
            }
        }
    }
}