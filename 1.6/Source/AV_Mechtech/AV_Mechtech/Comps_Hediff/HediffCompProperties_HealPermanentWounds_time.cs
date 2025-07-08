using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class HediffCompProperties_HealPermanentWounds_time : HediffCompProperties
    {
        public float DaysToHeal = 1f;

        public HediffCompProperties_HealPermanentWounds_time()
        {
            compClass = typeof(HediffComp_HealPermanentWounds_time);
        }

    }
}
