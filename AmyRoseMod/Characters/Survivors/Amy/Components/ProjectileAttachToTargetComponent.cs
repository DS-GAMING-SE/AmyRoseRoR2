using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class ProjectileAttachToTargetComponent : NetworkBehaviour
    {
        public ProjectileTargetComponent targetComponent;

        public HurtBox target;
        private bool targetDirty;
        private bool targetLost;

        public float verticalOffset = AmyStaticValues.specialMultiLockBlastRadius;

        public void Awake()
        {
            targetComponent = base.GetComponent<ProjectileTargetComponent>();
        }
        
        public void Start()
        {
            if (!target)
            {
                if (!FindTarget())
                {
                    targetLost = true;
                    return;
                }
            }
            else
            {
                VerticalOffset();
            }
        }

        public void VerticalOffset()
        {
            verticalOffset = Mathf.Max(target.collider.bounds.extents.y, verticalOffset);
        }

        public bool FindTarget()
        {
            if (targetComponent && targetComponent.target)
            {
                if (targetComponent.target.TryGetComponent(out HurtBox hurtBox))
                {
                    target = hurtBox;
                    targetDirty = true;
                    VerticalOffset();
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            if (target)
            {
                base.gameObject.transform.position = target.transform.position + (Vector3.up * verticalOffset);
            }
            else if (!targetLost)
            {
                targetLost = !FindTarget();
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(HurtBoxReference.FromHurtBox(target));
                return true;
            }
            if (targetDirty)
            {
                writer.Write(targetDirty);
                targetDirty = false;
                writer.Write(HurtBoxReference.FromHurtBox(target));
                return true;
            }
            return false;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                target = reader.ReadHurtBoxReference().ResolveHurtBox();
                if (target) VerticalOffset();
                return;
            }
            if (reader.ReadBoolean())
            {
                target = reader.ReadHurtBoxReference().ResolveHurtBox();
                if (target) VerticalOffset();
            }
        }
    }
}