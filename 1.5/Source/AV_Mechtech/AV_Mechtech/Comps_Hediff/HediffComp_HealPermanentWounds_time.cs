using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class HediffComp_HealPermanentWounds_time : HediffComp
    {
        private int ticksToHeal;

        public HediffCompProperties_HealPermanentWounds_time Props => (HediffCompProperties_HealPermanentWounds_time)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetTicksToHeal();
        }

        private void ResetTicksToHeal()
        {
            ticksToHeal = (int)(Props.DaysToHeal * 60000);  // Rand.Range(15, 30) * 60000;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {

            ticksToHeal--;
            if (ticksToHeal <= 0)
            {
                TryHealRandomPermanentWound(parent.pawn, parent.LabelCap);
                ResetTicksToHeal();
            }
 
        }


        public List<Hediff> hediffsToHeal = new List<Hediff>();

        public void UpdateHediffsToHeal(Pawn pawn)
        {
            hediffsToHeal.Clear();

            if (hediffsToHeal.NullOrEmpty())
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if(hediff.def.countsAsAddedPartOrImplant)
                    {
                        continue;
                    }

                    //hediff.Part != null && Props.bodyPartsToRestore != null && Props.bodyPartsToRestore.Contains(hediff.Part.def) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part)


                    if (hediff.IsPermanent())   //scars
                    {
                        hediffsToHeal.Add(hediff);
                    }
                    /*
                    if (hediff.def.chronic)
                    {
                        hediffsToHeal.Add(hediff);
                    }
                    if (hediff is Hediff_MissingPart)
                    {
                        hediffsToHeal.Add(hediff);
                    }

                    if(hediff is Hediff_Injury && hediff.IsPermanent())
                    {
                        hediffsToHeal.Add(hediff);
                    }
                    */
                }
            }

            if(MechtechSettings.DebugLogging)
            {
                foreach (Hediff hediff2 in hediffsToHeal)
                {
                    Log.Message("AV_Mechtech.HediffComp_HealPermanentWounds_time: possible scar to heal for neuroferium: " + hediff2);
                }
            }

            
        }

        public void TryHealRandomPermanentWound(Pawn pawn, string cause)
        {

            if (pawn.health == null)
            {
                return;
            }

            //try heal scars
            UpdateHediffsToHeal(pawn);
            if (hediffsToHeal.Any())
            {
                //try heal scars
                Hediff hediff = hediffsToHeal.First();
                HealthUtility.Cure(hediff);
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, hediff.Label, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
            else
            {
                //try heal one of the rest
                TaggedString taggedString = HealthUtility.FixWorstHealthCondition(pawn);
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Messages.Message(taggedString + " " + "AV_Message_CauseNeuroferium".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
                }
            }





            /*

            hediffsToHeal.Clear();
            UpdateHediffsToHeal(pawn);
            try
            {
                foreach (Hediff tmpHediff in hediffsToHeal)
                {
                    if (WillHeal(pawn, tmpHediff))
                    {
                        HealthUtility.Cure(tmpHediff);
                    }
                    else if (tmpHediff is Hediff_MissingPart hediff_MissingPart && hediff_MissingPart.IsFresh)
                    {
                        hediff_MissingPart.IsFresh = false;
                        pawn.health.Notify_HediffChanged(hediff_MissingPart);
                    }
                }
                if (hediffsToHeal.TryRandomElement(out var result))
                {
                    HealthUtility.Cure(result);
                    Messages.Message("BiosculpterHealCompletedWithCureMessage".Translate(pawn.Named("PAWN"), result.Named("HEDIFF")), pawn, MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("BiosculpterHealCompletedMessage".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
            finally
            {
                hediffsToHeal.Clear();
            }
            */


            /*
            UpdateHediffsToHeal(pawn);

            if (pawn.health.hediffSet.hediffs.Where((Hediff hd) => hd.IsPermanent() || hd.def.chronic || GetMissingBodyPartsPermanent(hd)).TryRandomElement(out var result))
            {
                
                if(result != null)
                {
                    HealthUtility.Cure(result);
                    pawn.health.Notify_HediffChanged(result);
                }
                
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    if(GetMissingBodyPartsPermanent(result))
                    {
                        string missing = "AV_Message_missing".Translate() + " " + result.Part.Label;
                        Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, missing, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
                    }
                    else
                    {
                        Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, result.Label, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
                    } 
                }
            }
            */
        }

        /*
        private static bool GetMissingBodyPartsPermanent(Hediff hediff)
        {
            if (hediff is Hediff_MissingPart)
            {
                return true;
            }

            return false;
        }
        */
        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksToHeal, "ticksToHeal", 0);
        }

        public override string CompDebugString()
        {
            return "ticksToHeal: " + ticksToHeal;
        }


        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEV: heal wound";
                command_Action.action = delegate
                {
                    ticksToHeal = 0;
                };
                yield return command_Action;
            }
        }




    }
}
