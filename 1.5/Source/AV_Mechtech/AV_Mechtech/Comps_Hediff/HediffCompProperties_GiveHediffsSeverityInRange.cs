using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static HarmonyLib.Code;

namespace AV_Mechtech
{
    public class HediffCompProperties_GiveHediffsSeverityInRange : HediffCompProperties
    {
        public float range;

        public TargetingParameters targetingParameters;

        public HediffDef hediff_to_set;    //to set on aura

        public HediffDef hediff_to_look;    //to get severity from

        public ThingDef mote;

        public bool hideMoteWhenNotDrafted;

        public float initialSeverity = 1f;

        public bool onlyPawnsInSameFaction = true;

        public HediffCompProperties_GiveHediffsSeverityInRange()
        {
            compClass = typeof(HediffComp_GiveHediffsSeverityInRange);
        }
    }

}

