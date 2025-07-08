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

namespace AV_Mechtech
{
    public class DeathActionProperties_Reshaper : DeathActionProperties
    {
        public List<PawnKindDef> dividePawnKindOptions = new List<PawnKindDef>();



        public int neededResources = 0;

        public List<PawnKindDef> dividePawnKindAdditionalForced = new List<PawnKindDef>();

        public IntRange divideBloodFilthCountRange = new IntRange(1,2);

        public DeathActionProperties_Reshaper()
        {
            workerClass = typeof(DeathActionWorker_Reshaper);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            if (neededResources <= 0)
            {
                yield return "deathActionWorkerClass is DeathActionWorker_Divide or subclass, but neededResources <= 0.";
            }
            if (dividePawnKindOptions.NullOrEmpty() && dividePawnKindAdditionalForced.NullOrEmpty())
            {
                yield return "deathActionWorkerClass is DeathActionWorker_Divide or subclass, but dividePawnKindOptions is null or empty.";
            }
        }
    }
}


