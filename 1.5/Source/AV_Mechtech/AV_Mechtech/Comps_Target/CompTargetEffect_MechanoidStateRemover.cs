using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AV_Mechtech
{
    public class CompTargetEffect_MechanoidStateRemover : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;   // specifies or converts? target as ParentAsPawn instead of thing!

            if (!pawn.Dead)
            {
                CompProperties_MechPowerCell compProperties = pawn.kindDef.race.GetCompProperties<CompProperties_MechPowerCell>();
                if (compProperties != null)
                {
                    pawn.Kill(null);
                }
                else if (pawn.mindState.mentalStateHandler.InMentalState == true)
                {
                    pawn.mindState.mentalStateHandler.Reset();
                }
                else if (MechtechDefOfs.AV_MechMightConnectToHive != null || MechtechDefOfs.AV_Databreach != null)
                {
                    RemoveHediff(pawn);
                }

                Find.BattleLog.Add(new BattleLogEntry_ItemUsed(user, target, parent.def, RulePackDefOf.Event_ItemUsed)); 
            }
        }

        private void RemoveHediff(Pawn pawn)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_MechMightConnectToHive);

            if(hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
                Messages.Message("AV_Message_OldConnectionDataWiped".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.NeutralEvent);
                if (pawn.Faction != Faction.OfPlayer)
                {
                    pawn.SetFaction(Faction.OfPlayer);     //set faction to users
                }
                // MechtechSettings.FailSafeLanceIsPermanent
 
            }

            hediff = pawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_Databreach);

            if(hediff != null)
            {
                if (pawn.Faction != Faction.OfPlayer)
                {
                    pawn.SetFaction(Faction.OfPlayer);     //set faction to users
                }
                Messages.Message("AV_Message_DatabreachFixed".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.NeutralEvent);
                pawn.health.RemoveHediff(hediff);
            }
 
        }
    }
}
