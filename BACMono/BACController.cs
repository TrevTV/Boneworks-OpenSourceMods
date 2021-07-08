using UnityEngine;
using System.Collections.Generic;
using ModThatIsNotMod;

namespace BACMono
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
        public bool hasBrakes;
    }

    public class BACController : MonoBehaviour
    {
        public BACController(System.IntPtr ptr) : base(ptr) { }

        public List<AxleInfo> axleInfos = new List<AxleInfo>();
        public HingeJoint steeringWheelJoint;
        public Transform steeringWheel;
        public float maxMotorTorque = 3000;
        public float maxSteeringAngle = 50;
        public float maxBreakTorque = 5000;
        public bool isActive;

        private float currentBrakeTorque;

        public void Start()
        {
            axleInfos.Add(GenerateAxisInfoForAxle("FrontLeft", "FrontRight", true));
            axleInfos.Add(GenerateAxisInfoForAxle("BackLeft", "BackRight", false));

            steeringWheel = transform.Find("Steering Wheel").Find("WheelOBJ");
            steeringWheelJoint = steeringWheel.GetComponent<HingeJoint>();

            transform.Find("Trigger").gameObject.AddComponent<TriggerCheck>().controller = this;

            PhysicMaterial mat = new PhysicMaterial();
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            mat.dynamicFriction = 0.3f;
            mat.staticFriction = 0.3f;

            foreach (MeshCollider col in gameObject.GetComponentsInChildren<MeshCollider>())
                col.material = mat;
        }

        public void Update()
        {
            if (Player.leftHand == null || !isActive) return;

            if (Player.rightHand.controller.GetSecondaryInteractionButton()) // Input.GetKey(KeyCode.Space)
                currentBrakeTorque = Mathf.Lerp(currentBrakeTorque, maxBreakTorque, 0.25f);
            else
                currentBrakeTorque = 0;
        }

        public void FixedUpdate()
        {         
            if (Player.leftHand != null && isActive)
            {
                float leftAxis = Player.leftHand.controller.GetPrimaryInteractionButtonAxis();
                float rightAxis = Player.rightHand.controller.GetPrimaryInteractionButtonAxis();

                float thumbstickAxis = Player.leftHand.controller.GetThumbStickAxis().x;

                float motor = -(maxMotorTorque * (-leftAxis + rightAxis));
                float steering = maxSteeringAngle * thumbstickAxis;
                //float steering = maxSteeringAngle * (steeringWheelJoint.angle / steeringWheelJoint.limits.max);

                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.steering)
                    {
                        axleInfo.leftWheel.steerAngle = steering;
                        axleInfo.rightWheel.steerAngle = steering;
                    }
                    if (axleInfo.motor)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                    if (axleInfo.hasBrakes)
                    {
                        axleInfo.leftWheel.brakeTorque = currentBrakeTorque;
                        axleInfo.rightWheel.brakeTorque = currentBrakeTorque;
                    }
                }
            }

            ApplyVisuals();
        }

        public void ApplyVisuals()
        {
            foreach (AxleInfo info in axleInfos)
            {
                Apply(info.leftWheel);
                Apply(info.rightWheel);
            }

            void Apply(WheelCollider collider)
            {
                if (collider.transform.childCount == 0) return;

                Transform visualWheel = collider.transform.GetChild(0);

                collider.GetWorldPose(out _, out Quaternion rotation);

                visualWheel.transform.rotation = rotation;
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public AxleInfo GenerateAxisInfoForAxle(string leftTire, string rightTire, bool isDriver)
        {
            AxleInfo info = new AxleInfo
            {
                leftWheel = GetWheel(leftTire),
                rightWheel = GetWheel(rightTire),
                motor = isDriver,
                steering = isDriver,
                hasBrakes = isDriver
            };

            return info;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public WheelCollider GetWheel(string wheel)
        {
            Transform tiresObj = transform.Find("Tires");
            Transform tireObj = tiresObj.Find(wheel);
            WheelCollider collider = tireObj.Find("GameObject").GetComponent<WheelCollider>();
            return collider;
        }
    }

    public class TriggerCheck : MonoBehaviour
    {
        public TriggerCheck(System.IntPtr ptr) : base(ptr) { }

        public BACController controller;

        public void OnTriggerStay(Collider col)
        {
            if (col.name == "Feet")
            {
                controller.isActive = true;
            }
        }

        public void OnTriggerExit(Collider col)
        {
            if (col.name == "Feet")
            {
                controller.isActive = false;
            }
        }
    }
}
