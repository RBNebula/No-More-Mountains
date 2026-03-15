using System.Collections.Generic;

namespace NoMoreMountains.Data
{
    internal sealed class OreBaselineState
    {
        public OreBaselineState(float baseSellValue, bool? detectCollisions, List<ColliderBaseline> colliders)
        {
            BaseSellValue = baseSellValue;
            DetectCollisions = detectCollisions;
            Colliders = colliders;
        }

        public float BaseSellValue { get; }
        public bool? DetectCollisions { get; }
        public List<ColliderBaseline> Colliders { get; }
    }
}
