using EntityStates;
using HedgehogUtils.Boost.EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HedgehogUtils.Boost;

namespace Amy.Survivors.Amy
{
    public static class AmySkillDefs
    {
        public interface IAmyBoost
        {
            SkillDef hammerSpinSkillDef { get; set; }
        }
        public class AmyBoostSkillDef : HedgehogUtils.Boost.SkillDefs.BoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSpinSkillDef { get; set; }
        }

        public class AmyRequiresFormBoostSkillDef : HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSpinSkillDef { get; set; }
        }
    }
}
