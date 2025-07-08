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

    //severity does not get changed! Instead old hediff gets removed and new with more severity gets added...


    public class CompAbilityEffect_SetHediffSeverity : CompAbilityEffect_WithDuration
    {
        public new CompProperties_SetHediffSeverity Props => (CompProperties_SetHediffSeverity)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            //
            //("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: base.apply");
            if (Props.applyToSelf || Props.onlyApplyToSelf)
            {
                if(MechtechSettings.DebugLogging)
                {
                    Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: Apply SetHediffSeverity to self");
                }           
                ApplyInner(parent.pawn, target.Pawn);
            }
            if (!Props.onlyApplyToSelf && Props.applyToTarget)
            {
                if (MechtechSettings.DebugLogging)
                {
                    Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: Apply SetHediffSeverity to others");
                }
                ApplyInner(target.Pawn, parent.pawn);
            }
        }

        protected void ApplyInner(Pawn target, Pawn other)
        {
            if (target == null)
            {
                if (MechtechSettings.DebugLogging)
                {
                    Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: no target");
                }
                return;
            }
            Hediff ExistingHediffDef = target.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);

            Hediff newHediff = HediffMaker.MakeHediff(Props.hediffDef, target, Props.onlyBrain ? target.health.hediffSet.GetBrain() : null);

            if (ExistingHediffDef != null)  // Hediff found
            {
                //save severity and remove hediff
                float CurrentSeverity = ExistingHediffDef.Severity;
                newHediff.Severity = CurrentSeverity + Props.SeverityChange;
                target.health.RemoveHediff(ExistingHediffDef);

                HediffComp_Disappears hediffComp_Disappears = newHediff.TryGetComp<HediffComp_Disappears>(); 
                if (hediffComp_Disappears != null)
                {
                    if (MechtechSettings.DebugLogging)
                    {
                        Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: fixing hediff disapearing");
                    }
                    hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(target).SecondsToTicks();   //copied from core game
                }

                if (Props.allowRemoving && newHediff.Severity >= Props.MaxSeverity)
                {
                    if (MechtechSettings.DebugLogging)
                    {
                        Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: removed hediff");
                    }
                    return;
                }

                // apply hediff with set severity, yeay <3
                if (Props.loop)
                {
                    if (newHediff.Severity <= Props.MinSeverity && Props.loop)
                    {
                        newHediff.Severity = Props.MaxSeverity;
                    }
                    else if (newHediff.Severity >= Props.MaxSeverity)
                    {
                        newHediff.Severity = Props.MinSeverity;
                    }
                    target.health.AddHediff(newHediff);
                }
                else //no loop
                {
                    if (newHediff.Severity < Props.MinSeverity)
                    {
                        newHediff.Severity = Props.MaxSeverity;
                    }
                    else if (newHediff.Severity > Props.MaxSeverity)
                    {
                        newHediff.Severity = Props.MinSeverity;
                    }
                    target.health.AddHediff(newHediff);
                }
            }
            else        //no hediff found
            {
                if (MechtechSettings.DebugLogging)
                {
                    Log.Message("[AV]Mechtech.CompAbilityEffect_SetHediffSeverity: adding hediff, no old one found");
                }
                target.health.AddHediff(newHediff);
            }
        }
    }
}

