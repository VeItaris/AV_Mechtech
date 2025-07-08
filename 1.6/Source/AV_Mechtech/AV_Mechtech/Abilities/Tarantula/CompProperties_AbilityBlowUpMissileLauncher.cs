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
    public class CompProperties_AbilityBlowUpMissileLauncher : CompProperties_AbilityEffect
    {
        public float smokeRadius;

        public CompProperties_AbilityBlowUpMissileLauncher()
        {
            compClass = typeof(CompAbilityEffect_BlowUpMissileLauncher);
        }
    }
}
