using RoR2;
using UnityEngine;

namespace Amy.Survivors.Amy.Components
{
    public class LeanIntoVelocityModelTransform : MonoBehaviour
    {
        public Transform modelTransform;
        public Transform modelParentTransform;

        public ModelLocator modelLocator;

        public bool active = false;
        public bool frozen;

        public Vector3 currentNormal;

        private CharacterBody characterBody;

        private const float maxAngleLean = 18f;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void Activate()
        {
            if (modelLocator)
            {
                modelLocator.autoUpdateModelTransform = false;
            }
            active = true;
        }

        public void Deactivate()
        {
            if (modelLocator)
            {
                modelLocator.autoUpdateModelTransform = true;
            }
            active = false;
        }

        public void LateUpdate()
        {
            if (active)
            {
                UpdateModelTransform();
            }
        }

        private void UpdateModelTransform()
        {
            Vector3 position = this.modelParentTransform.position;
            Quaternion quaternion = this.modelParentTransform.rotation;
            
            if (!frozen)
            {
                if (characterBody && characterBody.characterMotor)
                {
                    float velocityReachedMovementSpeed = Mathf.Clamp01(characterBody.characterMotor.velocity.magnitude / (characterBody.moveSpeed * 0.7f));
                    currentNormal = Vector3.RotateTowards(Vector3.up, characterBody.characterMotor.velocity.normalized, Mathf.Lerp(0, maxAngleLean * 0.017453292f, velocityReachedMovementSpeed), float.PositiveInfinity);
                }
                else
                {
                    currentNormal = Vector3.up;
                }
            }
            
            quaternion = Quaternion.FromToRotation(Vector3.up, this.currentNormal) * quaternion;
            this.modelTransform.SetPositionAndRotation(position, quaternion);
        }
    }
}