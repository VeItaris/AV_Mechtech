using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.AI;
using Verse;
using AV_Framework;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace AV_Mechtech
{
    public class CompUsableUnspecializedMechchip : ThingComp 
    {
        public CompProperties_Usable_UnspecializedMechchip Props => (CompProperties_Usable_UnspecializedMechchip)props;

        protected virtual string FloatMenuOptionLabel(Pawn pawn, ThingDef def)
        {
            if(def == null)
            {
                return Props.useLabel;
            }
            return Props.useLabel + def.label;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (Props.useJob == null)
            {
                yield break;
            }

            foreach(ThingDef thingdef in Props.thingDefs)
            {
                if (thingdef != null && thingdef.HasComp<CompAnalyzableUnlockResearch>())
                {
                    AcceptanceReport acceptanceReport = CanBeUsedBy(myPawn, thingdef, Props.ignoreOtherReservations);
                    FloatMenuOption floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(FloatMenuOptionLabel(myPawn, thingdef), delegate
                    {
                        TryStartUseJob(myPawn, thingdef, Props.ignoreOtherReservations);
                    }, priority: Props.floatMenuOptionPriority, itemIcon: GetIcon(thingdef), iconColor: Color.white), myPawn, parent);

                    if (!acceptanceReport.Accepted)
                    {
                        floatMenuOption.Disabled = true;
                        floatMenuOption.Label = floatMenuOption.Label + " (" + acceptanceReport.Reason + ")";
                    }
                    yield return floatMenuOption;
                }
            }
        }

        private Texture2D GetIcon(ThingDef def)
        {
            if(def.uiIcon != null)
            {
                return def.uiIcon;
            }
            return null;
        }

        public bool IsAnalysed(ThingDef def)
        {
            if (def != null && def.GetCompProperties<CompProperties_CompAnalyzableUnlockResearch>() != null)
            {
                int id = def.GetCompProperties<CompProperties_CompAnalyzableUnlockResearch>().analysisID;
                Find.AnalysisManager.TryGetAnalysisProgress(id, out var details);
                if (details != null && details.Satisfied)
                {
                    return true;
                }

                //Find.AnalysisManager.TryGetAnalysisProgress(requiredAnalyzed[i].GetCompProperties<CompProperties_CompAnalyzableUnlockResearch>()?.analysisID ?? (-1), out var details);

                //return Find.AnalysisManager.IsAnalysisComplete(id); //alwayss true, what is this shit
            }

            return false;
        }

        public virtual void TryStartUseJob(Pawn pawn, ThingDef def, bool forced = false)
        {
            if (Props.useJob == null || !CanBeUsedBy(pawn, def, forced))
            {
                return;
            }

            StartJob();
           
            void StartJob()
            {
                ConvertedTo = def;
                Job job =  JobMaker.MakeJob(Props.useJob, parent);
                job.count = 1;
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            
        }

        public bool DestroyedFromJob = false;
        public virtual void OnAnalyzed()
        {
            DestroyedFromJob = true;
            //parent.stackCount -= 1;
            parent.Destroy();
        }


        public ThingDef ConvertedTo;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if(ConvertedTo != null && DestroyedFromJob)
            {
                Thing chip = ThingMaker.MakeThing(ConvertedTo);
                chip.stackCount = 1;    //it does not care about parent.stacksize in postdesstroy :(
                GenPlace.TryPlaceThing(chip, parent.Position, previousMap, ThingPlaceMode.Near, out var lastResultingThing);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return base.CompGetGizmosExtra();
        }

        public virtual AcceptanceReport CanBeUsedBy(Pawn p, ThingDef def, bool forced = false, bool ignoreReserveAndReachable = false)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.IsMutant && !Props.allowedMutants.Contains(p.mutant.Def))
            {
                return false;
            }

            if (!MechanitorUtility.IsMechanitor(p))
            {
                return "MissingRequiredMechanitor".Translate();
            }

            if (!IsAnalysed(def))
            {
                return "NotStudied".Translate(def.label);
                //return "AV_NotAnalysed".Translate();
            }

            if (!ignoreReserveAndReachable && !p.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
            {
                return "NoPath".Translate();
            }
            if (!ignoreReserveAndReachable && !p.CanReserve(parent, 1, -1, null, forced))
            {
                Pawn pawn = p.Map.reservationManager.FirstRespectedReserver(parent, p) ?? p.Map.physicalInteractionReservationManager.FirstReserverOf(parent);
                if (pawn != null)
                {
                    return "ReservedBy".Translate(pawn.LabelShort, pawn);
                }
                return "Reserved".Translate();
            }
            
            return true;
        }



        public void SendCannotUseMessage(Pawn pawn, string reason)
        {
            Messages.Message(string.Format("{0}: {1}", "CannotGenericWorkCustom".Translate(FloatMenuOptionLabel(pawn, null)).ToLower(), reason.CapitalizeFirst()), pawn, MessageTypeDefOf.RejectInput, historical: false);
        }

        public void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
            }
        }
        




    }
}
