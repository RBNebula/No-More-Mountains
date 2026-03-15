using UnityEngine;

namespace NoMoreMountains.Data
{
    internal sealed class ColliderBaseline
    {
        public ColliderBaseline(Collider collider, bool enabled, bool isTrigger)
        {
            Collider = collider;
            Enabled = enabled;
            IsTrigger = isTrigger;
        }

        public Collider Collider { get; }
        public bool Enabled { get; }
        public bool IsTrigger { get; }
    }
}
