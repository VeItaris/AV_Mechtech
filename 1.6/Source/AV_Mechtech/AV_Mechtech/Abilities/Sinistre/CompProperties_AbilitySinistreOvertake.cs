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
    public class CompProperties_AbilitySinistreOvertake : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySinistreOvertake()
        {
            compClass = typeof(CompAbilityEffect_SinistreOvertake);
        }
    }
}
