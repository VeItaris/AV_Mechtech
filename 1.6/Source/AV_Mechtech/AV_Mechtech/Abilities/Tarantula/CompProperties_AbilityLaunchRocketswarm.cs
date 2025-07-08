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
    public class CompProperties_AbilityLaunchRocketswarm : CompProperties_AbilityEffect
    {
        public ThingDef projectileDef;

        public CompProperties_AbilityLaunchRocketswarm()
        {
            compClass = typeof(CompAbilityEffect_LaunchRocketswarm);
        }
    }
}
