using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class CompProperties_UseEffectUpgradeMechlink : CompProperties_UseEffect
    {
        public HediffDef hediffDef;

        public HediffDef hediffDefToRemove;

        public BodyPartDef bodyPart;

        public bool canUpgrade;

        public bool allowNonColonists;

        public bool requiresExistingHediff;

        public float maxSeverity = float.MaxValue;

        public float minSeverity;

        public bool requiresPsychicallySensitive;

        public CompProperties_UseEffectUpgradeMechlink()
        {
            compClass = typeof(CompUseEffect_UpgradeMechlink);
        }
    }
}
