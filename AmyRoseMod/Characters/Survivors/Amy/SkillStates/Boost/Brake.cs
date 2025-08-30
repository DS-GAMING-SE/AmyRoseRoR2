using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;

namespace Amy.Survivors.Amy.SkillStates
{
    public class Brake : HedgehogUtils.Boost.EntityStates.Brake
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = true;
            }
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }
    }
}