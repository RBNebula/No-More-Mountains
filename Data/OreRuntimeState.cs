using UnityEngine;

namespace NoMoreMountains.Data
{
    internal sealed class OreRuntimeState
    {
        public OreRuntimeState(Vector3 lastPosition, float lastMotionTime)
        {
            LastPosition = lastPosition;
            LastMotionTime = lastMotionTime;
        }

        public Vector3 LastPosition { get; set; }
        public float LastMotionTime { get; set; }
        public bool HasDropped { get; set; }
    }
}
