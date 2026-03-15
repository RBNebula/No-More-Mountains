using NoMoreMountains.Config;
using UnityEngine;

namespace NoMoreMountains.Services
{
    internal sealed class OreCleanupService
    {
        private readonly NoMoreMountainsConfig _config;

        public OreCleanupService(NoMoreMountainsConfig config)
        {
            _config = config;
        }

        public bool IsEligiblePieceType(OrePiece? ore)
        {
            if (ore == null || !ore.isActiveAndEnabled || !ore.gameObject.activeInHierarchy)
            {
                return false;
            }

            return ore.PieceType switch
            {
                PieceType.Ore => true,
                PieceType.OreCluster => true,
                PieceType.Gem => true,
                PieceType.Crushed => true,
                _ => false
            };
        }

        public bool IsProtectedFromCleanup(OrePiece ore)
        {
            if (ore.BasketsThisIsInside != null && ore.BasketsThisIsInside.Count > 0)
            {
                return true;
            }

            if (ore.CurrentMagnetTool != null)
            {
                return true;
            }

            return ore.CompareTag("MarkedForDestruction");
        }

        public bool HasMovedEnough(OrePiece ore, Vector3 lastPosition)
        {
            float movementThreshold = Mathf.Max(0.0005f, _config.MovementThreshold.Value);
            float movementThresholdSqr = movementThreshold * movementThreshold;
            Vector3 currentPosition = ore.transform.position;

            if ((currentPosition - lastPosition).sqrMagnitude >= movementThresholdSqr)
            {
                return true;
            }

            if (ore.Rb == null)
            {
                return false;
            }

            float velocityThreshold = Mathf.Max(0.001f, _config.VelocityThreshold.Value);
            float velocityThresholdSqr = velocityThreshold * velocityThreshold;

            if (ore.Rb.linearVelocity.sqrMagnitude >= velocityThresholdSqr)
            {
                return true;
            }

            return ore.Rb.angularVelocity.sqrMagnitude >= velocityThresholdSqr;
        }

        public void DropOreThroughWorld(OrePiece ore)
        {
            ore.BaseSellValue = 0f;

            if (ore.CurrentMagnetTool != null && ore.Rb != null)
            {
                ore.CurrentMagnetTool.DetachBody(ore.Rb);
            }

            Collider[] colliders = ore.GetComponentsInChildren<Collider>(includeInactive: false);
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider collider = colliders[index];
                if (collider == null)
                {
                    continue;
                }

                collider.enabled = true;
                collider.isTrigger = true;
            }

            if (ore.Rb == null)
            {
                return;
            }

            ore.Rb.detectCollisions = true;

            if (ore.Rb.isKinematic)
            {
                ore.Rb.isKinematic = false;
            }

            Vector3 velocity = ore.Rb.linearVelocity;
            if (velocity.y > -0.25f)
            {
                velocity.y = -0.25f;
                ore.Rb.linearVelocity = velocity;
            }

            ore.Rb.WakeUp();
        }
    }
}
