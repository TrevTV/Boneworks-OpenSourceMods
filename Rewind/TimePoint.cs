using UnityEngine;

namespace Rewind
{
    public class TimePoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public TimePoint(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
