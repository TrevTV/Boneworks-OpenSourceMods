using System;
using UnityEngine;
using System.Collections.Generic;
using StressLevelZero.Interaction;
using StressLevelZero.Rig;

namespace Rewind
{
    public class TimeBody : MonoBehaviour
    {
        public TimeBody(IntPtr ptr) : base(ptr) { }

        public Grip[] grips;
        public bool isKinematic;
        public Rigidbody rigidbody;
        
        private bool rewinding;
        private bool attachedToRig;
        private Transform thisTransform;
        private List<TimePoint> points = new List<TimePoint>();

        public void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            grips = GetComponentsInChildren<Grip>();
            thisTransform = transform;
        }

        public void Update()
        {
            if (!rewinding)
                RecordTransform();
            else
                Rewind();
        }

        public void StartRewinding()
        {
            if (rigidbody != null)
            {
                attachedToRig = rigidbody.GetComponentInParent<RigManager>();
                rewinding = true;
            }
            else
                Destroy(this);
        }

        public void StopRewinding()
        {
            rewinding = false;
            attachedToRig = rigidbody.GetComponentInParent<RigManager>();
            if (!isKinematic)
                rigidbody.isKinematic = false;
        }

        public void Rewind()
        {
            if (grips.Length > 0)
                foreach (Grip grip in grips)
                    if (grip.attachedHands.Count > 0)
                        return;

            if (attachedToRig)
                return;

            if (points.Count > 0)
            {
                rigidbody.isKinematic = true;
                TimePoint point = points[0];
                thisTransform.position = point.position;
                thisTransform.rotation = point.rotation;
                points.RemoveAt(0);
            }
            else
                StopRewinding();
        }

        public void RecordTransform()
        {
            //MelonLogger.Log("recording with pos " + thisTransform.position.ToString() + " and rot " + thisTransform.rotation.eulerAngles.ToString());
            if (points.Count > Mathf.Round(5f / Time.fixedDeltaTime))
                points.RemoveAt(points.Count - 1);

            points.Insert(0, new TimePoint(thisTransform.position, thisTransform.rotation));
        }
    }
}