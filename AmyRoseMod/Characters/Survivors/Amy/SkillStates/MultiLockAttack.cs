using  AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using HedgehogUtils;
using HedgehogUtils.Boost;
using RoR2;
using RoR2.Audio;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockAttack : BaseState
    {
        public HurtBox target;
        
        public List<HurtBox> targets;

        protected Vector3 targetLastPosition;

        public bool firstAttack;

        public bool finalAttack;

        public Vector3 orbStartPosition;

        public float orbSpeed = AmyStaticValues.specialMultiLockOrbSpeed;

        public float orbBounceRange;

        protected float predictedTimeUntilArrival;

        protected AmyOrbs.MultiLockOrb orb;

        protected CharacterModel characterModel;

        public MultiLockCameraProvider camera;

        public virtual Type nextStateType { get { return typeof(MultiLockEnd); } }

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                if (firstAttack)
                {
                    EntityStateMachine weaponState = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
                    if (weaponState) { weaponState.SetNextStateToMain(); }
                    if (base.skillLocator)
                    {
                        skillLocator.special.DeductStock(1);
                    }
                    characterBody.OnSkillActivated(base.skillLocator.special);
                }
                finalAttack = targets.Count == 1;
            }
            if (NetworkServer.active)
            {
                FireOrb();
                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
                base.characterBody.AddBuff(DLC3Content.Buffs.Untargetable);
            }
            if (target)
            {
                targetLastPosition = target.transform.position;
                predictedTimeUntilArrival = Vector3.Distance(orbStartPosition, targetLastPosition) / orbSpeed;
                predictedTimeUntilArrival += 0.1f;
            }
            else
            {
                targetLastPosition = base.transform.position;
            }
            if (base.modelLocator && base.modelLocator.modelTransform)
            {
                characterModel = base.modelLocator.modelTransform.GetComponent<CharacterModel>();
                if (characterModel)
                {
                    characterModel.invisibilityCount++;
                }
            }

            if (firstAttack)
            {
                Util.PlaySound("Play_amyrose_multilock_start", base.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (target)
            {
                targetLastPosition = target.transform.position;
                if (camera)
                {
                    camera.targetPosition = targetLastPosition;
                }
            }

            if (predictedTimeUntilArrival > 0 && predictedTimeUntilArrival - fixedAge <= MultiLockCameraProvider.maxLerpTime &&
                AmyConfig.multiLockSmoothCamera.Value != MultiLockCameraProvider.CameraMovementModes.Instant && finalAttack && predictedTimeUntilArrival > 0
                && !camera)
            {
                camera = MultiLockCameraProvider.StartCameraMove(base.gameObject, orbStartPosition, targetLastPosition, predictedTimeUntilArrival - fixedAge);
            }

            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }
                if (fixedAge >= predictedTimeUntilArrival) // on orb hits
                {
                    if (targets.Count > 1)
                    {
                        targets.RemoveAt(0);
                        SanitizeTargets();
                        if (targets.Count > 0)
                        {
                            SetNextStateToOrb();
                            return;
                        }
                    }
                    SetNextStateToEnd();
                    return;
                }
            }
        }

        protected void SanitizeTargets()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (!targets[0] || Vector3.Distance(targets[0].transform.position, targetLastPosition) > orbBounceRange)
                {
                    targets.RemoveAt(0);
                    i--;
                }
                else
                {
                    break;
                }
            }
        }

        public virtual void FireOrb()
        {
            if (!target) { return; }
            orb = AmyOrbs.CreateMultiLockOrb<AmyOrbs.MultiLockOrb>(AmyStaticValues.specialMultiLockDamageCoefficient * damageStat, base.gameObject, this.outer, Util.CheckRoll(this.critStat, base.characterBody.master), 
                AmyAssets.multiLockProjectilePrefab, orbSpeed, orbStartPosition, target, OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect"));
            OrbManager.instance.AddOrb(orb);
        }

        public virtual void SetNextStateToOrb()
        {
            MultiLockAttack nextState = (MultiLockAttack)EntityStateCatalog.InstantiateState(this.GetType());
            nextState.firstAttack = false;
            nextState.orbStartPosition = targetLastPosition;
            nextState.target = targets[0];
            nextState.targets = targets;
            nextState.orbBounceRange = orbBounceRange;
            nextState.orbSpeed = orbSpeed;
            this.outer.SetNextState(nextState);
        }

        public virtual void SetNextStateToEnd()
        {
            MultiLockEnd nextState = (MultiLockEnd)EntityStateCatalog.InstantiateState(nextStateType);
            nextState.teleportPosition = targetLastPosition;
            this.outer.SetNextState(nextState);
        }

        public override void OnExit()
        {
            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
                base.characterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
            }
            if (characterDirection)
            {
                base.characterDirection.forward = (targetLastPosition - orbStartPosition).normalized;
                base.characterDirection.moveVector = (targetLastPosition - orbStartPosition).normalized;
            }
            if (camera)
            {
                camera.EndCameraMove();
                Destroy(camera);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(HurtBoxReference.FromHurtBox(target));
            writer.Write(orbStartPosition);
            writer.Write(firstAttack);
            writer.Write(finalAttack);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            target = reader.ReadHurtBoxReference().ResolveHurtBox();
            orbStartPosition = reader.ReadVector3();
            firstAttack = reader.ReadBoolean();
            finalAttack = reader.ReadBoolean();
        }
    }
    // Nemmerc code looks very sane and normal
    //
    // I could've just made this a class and not a monobehaviour but nooooooo. CameraModePlayerBasic has to be annoying and throw errors every frame if I'm not using
    // UnityEngine.Object as the base class. The error doesn't even do anything, it's just annoying to look at
    public class MultiLockCameraProvider : MonoBehaviour, ICameraStateProvider
    {
        public const float maxLerpTime = 0.35f;
        
        public GameObject user;
        public Vector3 userPosition;

        public Vector3 orbStartPosition;
        
        public Vector3 targetPosition;

        public Vector3 backwardMovementVector;
        
        public static MultiLockCameraProvider StartCameraMove(GameObject user, Vector3 orbStartPosition, Vector3 targetPosition, float lerpTime)
        {
            MultiLockCameraProvider provider = user.AddComponent<MultiLockCameraProvider>();
            provider.orbStartPosition = orbStartPosition;
            provider.targetPosition = targetPosition;
            provider.user = user;
            provider.userPosition = user.transform.position;
            foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
            {
                if (cameraRigController.target == user)
                {
                    cameraRigController.SetOverrideCam(provider, Mathf.Min(lerpTime, maxLerpTime));
                }
                else if (cameraRigController.IsOverrideCam(provider))
                {
                    cameraRigController.SetOverrideCam(null, 0.05f);
                }
            }
            return provider;
        }

        public void EndCameraMove()
        {
            ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                CameraRigController cameraRigController = readOnlyInstancesList[i];
                if (cameraRigController.IsOverrideCam(this))
                {
                    cameraRigController.SetOverrideCam(null, 0.2f);
                }
            }
        }

        public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
        {
            userPosition = user ? user.transform.position : userPosition;

            backwardMovementVector = (orbStartPosition - targetPosition).normalized;
            backwardMovementVector.y = Mathf.Clamp(backwardMovementVector.y, -0.1f, -0.3f);

            Vector3 cameraLocalPos = cameraRigController.targetParams.currentCameraParamsData.idealLocalCameraPos.value;
            cameraLocalPos.y += cameraRigController.targetParams.currentCameraParamsData.pivotVerticalOffset.value + 0.9f;
            cameraLocalPos += (cameraRigController.targetParams.cameraPivotTransform ? cameraRigController.targetParams.cameraPivotTransform.localPosition : Vector3.zero);
            Vector3 cameraPosition = targetPosition;
            cameraPosition += (AmyConfig.multiLockSmoothCamera.Value == CameraMovementModes.MoveAndRotate ? 
                Util.QuaternionSafeLookRotation(backwardMovementVector.normalized) : cameraState.rotation) * cameraLocalPos;

            Vector3 between = cameraPosition - user.transform.position;
            float distanceFromPivot = Raycast(new Ray(userPosition, between.normalized), between.magnitude, 0.09f);
            Vector3 finalPosition = (between.normalized * distanceFromPivot) + userPosition;

            cameraState.position = finalPosition;

            if (AmyConfig.multiLockSmoothCamera.Value == CameraMovementModes.MoveAndRotate)
            {
                cameraState.rotation = Util.QuaternionSafeLookRotation(backwardMovementVector).normalized;
            }
        }
        public float Raycast(Ray ray, float maxDistance, float wallCushion)
        {
            LayerIndex world = LayerIndex.world;
            RaycastHit[] array = Physics.SphereCastAll(ray, wallCushion, maxDistance, world.mask, QueryTriggerInteraction.Ignore);
            float num = maxDistance;
            for (int i = 0; i < array.Length; i++)
            {
                float distance = array[i].distance;
                if (distance < num)
                {
                    Collider collider = array[i].collider;
                    if (collider && !collider.GetComponent<NonSolidToCamera>())
                    {
                        num = distance;
                    }
                }
            }
            return num;
        }

        public bool IsHudAllowed(CameraRigController cameraRigController)
        {
            return true;
        }

        public bool IsUserControlAllowed(CameraRigController cameraRigController)
        {
            return true;
        }

        public bool IsUserLookAllowed(CameraRigController cameraRigController)
        {
            return true;
        }

        public enum CameraMovementModes
        {
            MoveAndRotate,
            Move,
            Instant
        }
    }
}