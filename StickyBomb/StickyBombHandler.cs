using UnityEngine;
using StressLevelZero.Rig;
using StressLevelZero.Interaction;

namespace StickyBomb
{
    public class StickyBombHandler : MonoBehaviour
    {
        public StickyBombHandler(System.IntPtr ptr) : base(ptr) { }

        public Grip grip;
        public Rigidbody rb;

        public FixedJoint connector;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            grip = GetComponentInChildren<Grip>();
        }

        private void OnCollisionStay(Collision col)
        {
            // Check if the object is attached to the player
            if (col.gameObject.GetComponentInParent<RigManager>()) return;

            Rigidbody colRb = col.gameObject.GetComponentInParent<Rigidbody>();
            if (grip.attachedHands.Count == 0)
                Connect(colRb);
        }

        private void OnEnable() => Disconnect();

        public void Connect(Rigidbody connectRb = null)
        {
            if (connector) return;

            connector = gameObject.AddComponent<FixedJoint>();
            if (connectRb)
                connector.connectedBody = connectRb;
            else
                connector.connectedBody = rb;
        }

        public void Disconnect()
        {
            if (connector)
                Destroy(connector);
        }
    }
}
