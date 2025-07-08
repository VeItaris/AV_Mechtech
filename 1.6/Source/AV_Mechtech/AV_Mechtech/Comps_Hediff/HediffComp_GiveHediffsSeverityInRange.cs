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
    public class HediffComp_GiveHediffsSeverityInRange : HediffComp
    {
        private Mote mote;

        private int timer = 0;
        private int maxtimer = 20;

        public HediffCompProperties_GiveHediffsSeverityInRange Props => (HediffCompProperties_GiveHediffsSeverityInRange)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            //look if ParentAsPawn is able to use aura effect
            if (!parent.pawn.Awake() || parent.pawn.health == null || parent.pawn.health.InPainShock || !parent.pawn.Spawned)
            {
                return;
            }

            //performance optimizing
            if (timer > 0)
            {
                timer--;
                return;
            }
            else
            {
                timer = maxtimer;
            }


            //create mote
            if (!Props.hideMoteWhenNotDrafted || parent.pawn.Drafted)
            {
                if (Props.mote != null && (mote == null || mote.Destroyed))
                {
                    mote = MoteMaker.MakeAttachedOverlay(parent.pawn, Props.mote, Vector3.zero);
                }
                if (mote != null)
                {
                    mote.Maintain();
                }
            }

            Hediff ExistingHediffDefwithSeverity = parent.pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff_to_look);  //new

            //aura magic begins here
            IReadOnlyList<Pawn> list = null;

            list = (!Props.onlyPawnsInSameFaction || parent.pawn.Faction == null) ? parent.pawn.Map.mapPawns.AllPawnsSpawned : parent.pawn.Map.mapPawns.SpawnedPawnsInFaction(parent.pawn.Faction);
            foreach (Pawn item in list)
            {
                if (!item.RaceProps.Humanlike || item.Dead || item.health == null || item == parent.pawn || !(item.Position.DistanceTo(parent.pawn.Position) <= Props.range) || !Props.targetingParameters.CanTarget(item))
                {
                    continue;
                }
                Hediff hediff = item.health.hediffSet.GetFirstHediffOfDef(Props.hediff_to_set);
                // add hefiff with new severity when target ParentAsPawn has no hediff
                if (hediff == null)
                {
                    hediff = item.health.AddHediff(Props.hediff_to_set, item.health.hediffSet.GetBrain());
                    hediff.Severity = ExistingHediffDefwithSeverity.Severity;
                    HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
                    if (hediffComp_Link != null)
                    {
                        hediffComp_Link.drawConnection = true;
                        hediffComp_Link.other = parent.pawn;
                    }
                }
                else //new -> change severity when hediff is already on target ParentAsPawn
                {
                    HediffComp_Disappears comp = hediff.TryGetComp<HediffComp_Disappears>();

                    if (comp != null && comp.ticksToDisappear >= 120 && comp.ticksToDisappear > comp.disappearsAfterTicks - 60) //so we don't constantly overwrite when we have multiple sources of different severity
                    {
                        if(hediff.Severity != ExistingHediffDefwithSeverity.Severity)
                        {
                            hediff.Severity = ExistingHediffDefwithSeverity.Severity;
                        }
                    }
                    ChangeTicksToDisappear(hediff);
                }
                
            }
        }
    
        private void ChangeTicksToDisappear(Hediff hediff)
        {
            HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
            if (hediffComp_Disappears == null)
            {
                Log.Error("[AV]Mechtech.HediffComp_GiveHediffsSeverityInRange: has a hediff in props which does not have a HediffComp_Disappears");
            }
            else
            {
                hediffComp_Disappears.ticksToDisappear = maxtimer + 5;  // 65 ticks
            }
        }

    }
}

