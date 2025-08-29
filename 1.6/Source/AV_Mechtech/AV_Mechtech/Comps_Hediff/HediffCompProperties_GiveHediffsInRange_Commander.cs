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
    public class HediffCompProperties_GiveHediffsInRange_Commander : HediffCompProperties
    {
        public float range;

        public TargetingParameters targetingParameters;

        public HediffDef hediffMechs;

        public HediffDef hediffHumans;

        public ThingDef mote;

        public bool hideMoteWhenNotDrafted;

        public float initialSeverity = 1f;

        public bool onlyPawnsInSameFaction = true;

        public HediffCompProperties_GiveHediffsInRange_Commander()
        {
            compClass = typeof(HediffComp_GiveHediffsInRange_Commander);
        }
    }
}
