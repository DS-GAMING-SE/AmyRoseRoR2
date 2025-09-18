using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using Amy.Survivors.Amy;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class Dizzy : GenericCharacterMain
    {
        public float duration = AmyStaticValues.dizzyDuration;
        
        protected GameObject stunVFXInstance;
        protected EffectManagerHelper _efhStunEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            base.modelLocator.normalizeToFloor = true;
            CreateVFX();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(AmyBuffs.dizzyDebuff);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.isSprinting = false;
            if (fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.modelLocator.normalizeToFloor = false;
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(AmyBuffs.dizzyDebuff);
            }
            ReleaseStunVFX();
            base.OnExit();
        }

        public override bool CanExecuteSkill(GenericSkill skillSlot)
        {
            return false;
        }
        // Mostly copied from StunState
        protected virtual void CreateVFX()
        {
            if (this.duration >= 0f)
            {
                if (!EffectManager.ShouldUsePooledEffect(StunState.stunVfxPrefab))
                {
                    this.stunVFXInstance = GameObject.Instantiate<GameObject>(StunState.stunVfxPrefab, base.transform);
                }
                else
                {
                    this._efhStunEffect = EffectManager.GetAndActivatePooledEffect(StunState.stunVfxPrefab, base.transform, false);
                    this.stunVFXInstance = this._efhStunEffect.gameObject;
                }
                ScaleParticleSystemDuration component = this.stunVFXInstance.GetComponent<ScaleParticleSystemDuration>();
                component.newDuration = this.duration;
                component.UpdateDuration();
            }
        }
        protected virtual void ReleaseStunVFX()
        {
            if (this.stunVFXInstance)
            {
                if (!EffectManager.UsePools)
                {
                    EntityState.Destroy(this.stunVFXInstance);
                }
                else if (this._efhStunEffect != null && this._efhStunEffect.OwningPool != null)
                {
                    if (!this._efhStunEffect.OwningPool.IsObjectInPool(this._efhStunEffect))
                    {
                        this._efhStunEffect.OwningPool.ReturnObject(this._efhStunEffect);
                    }
                }
                else
                {
                    if (this._efhStunEffect != null)
                    {
                        Debug.LogFormat("StunEffect has no owning pool {0} {1}", new object[]
                        {
                            base.gameObject.name,
                            base.gameObject.GetInstanceID()
                        });
                    }
                    EntityState.Destroy(this.stunVFXInstance);
                }
                this._efhStunEffect = null;
                this.stunVFXInstance = null;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Stun;
        }
    }
}
