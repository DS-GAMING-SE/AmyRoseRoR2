using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class ProjectileAttachToTargetComponent : MonoBehaviour
    {
        public ProjectileTargetComponent targetComponent;

        public float verticalOffset;

        public void Start()
        {
            targetComponent = base.GetComponent<ProjectileTargetComponent>();

            verticalOffset = AmyStaticValues.specialMultiLockBlastRadius;
            if (targetComponent && targetComponent.target && targetComponent.target.TryGetComponent<Collider>(out Collider collider))
            {
                verticalOffset = Mathf.Max(collider.bounds.extents.y, verticalOffset);
            }
        }

        public void Update()
        {
            if (targetComponent && targetComponent.target)
            {
                base.gameObject.transform.position = targetComponent.target.position + (Vector3.up * verticalOffset);
            }
        }
    }
}