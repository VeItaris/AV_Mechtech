using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using static RimWorld.MechClusterSketch;
using static RimWorld.RitualStage_InteractWithRole;
using static UnityEngine.GraphicsBuffer;

namespace AV_Mechtech
{
    public class CompTargetEffect_Hack : CompTargetEffect
    {
        private int goodwillcostPerBandwidth = 25;

        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;   // specifies or converts? target as ParentAsPawn instead of thing!

            if (!pawn.Dead)
            {
                if (pawn.RaceProps.IsMechanoid)
                {
                    if (ModsConfig.IsActive("Veltaris.Mechcrate"))
                    {
                        HediffDef HiveHediff = MechtechDefOfs.AV_MechMightConnectToHive;

                        if(HiveHediff != null)
                        {
                            RemoveHediff(pawn, HiveHediff);
                        }
                        HiveHediff = MechtechDefOfs.AV_Databreach;
                        if (HiveHediff != null)
                        {
                            RemoveHediff(pawn, HiveHediff);
                        }
                    }

                    CompApparelReloadable compReloadable = parent.TryGetComp<CompApparelReloadable>();
                    //int chargesremaining = compReloadable.RemainingCharges;

                    
                    float bandwidth_cost = pawn.GetStatValue(StatDefOf.BandwidthCost);
                    /*
                    if (bandwidth_cost > MechtechSettings.HackingLanceBandwidth)    //centerpede is 4
                    {
                        Messages.Message("AV_DescHackingBandwithFailOne".Translate().CapitalizeFirst() + " " + MechtechSettings.HackingLanceBandwidth.ToString() + "AV_DescHackingBandwithFailTwo".Translate(), pawn, MessageTypeDefOf.NegativeEvent);

                        return;
                    }
                    if (bandwidth_cost > chargesremaining) 
                    {
                        Messages.Message("AV_DescHackingBandwithFailCharge".Translate().CapitalizeFirst(), pawn, MessageTypeDefOf.NegativeEvent);
                        return;
                    }
                    else
                    {*/
                    if (bandwidth_cost > 1)
                        {
                            int ChargeCost = (int)bandwidth_cost - 1;

                            for (int i = 0; i < ChargeCost; i++)
                            {
                                compReloadable.UsedOnce();
                            }

                        }
                        ChangeFaction(user, pawn, target);
                    //}
                    if (pawn.mindState.mentalStateHandler.InMentalState == true)
                    {
                        pawn.mindState.mentalStateHandler.Reset();
                        return;
                    }
                    Messages.Message("AV_DescHackingBandwithSuccess".Translate().CapitalizeFirst(), pawn, MessageTypeDefOf.PositiveEvent);
                    //ChangeFaction(user, ParentAsPawn, target);
                }
            }
        }

        private void ChangeFaction(Pawn user, Pawn targetpawn, Thing target)
        {
            Pawn overseer;
            Faction casterfaction = user.Faction;

            if (targetpawn.Faction == casterfaction)
            {
                if (targetpawn.drafter.Drafted)
                {
                    targetpawn.drafter.Drafted = false;
                }


                overseer = targetpawn.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer);

                if (overseer != null)
                {
                    targetpawn.relations.TryRemoveDirectRelation(PawnRelationDefOf.Overseer, overseer);   //remove overseer
                }
            }
            else
            {
                float goodwillcost = -goodwillcostPerBandwidth * targetpawn.GetStatValue(StatDefOf.BandwidthCost);
                user.Faction.TryAffectGoodwillWith(targetpawn.Faction, (int)goodwillcost);

                Find.BattleLog.Add(new BattleLogEntry_ItemUsed(user, target, parent.def, RulePackDefOf.Event_ItemUsed));
                targetpawn.SetFaction(casterfaction);     //set faction to users

                targetpawn.GetLord()?.Notify_PawnLost(targetpawn, PawnLostCondition.ChangedFaction);
            
                if(ModsConfig.IsActive("Veltaris.Mechcrate"))
                {
                    if(MechtechDefOfs.AV_MechMightConnectToHive != null)
                    {
                        targetpawn.health.AddHediff(MechtechDefOfs.AV_MechMightConnectToHive);
                    }
                }
            }
        }

        private void RemoveHediff(Pawn pawn, HediffDef hediffdef)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffdef);

            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
                Messages.Message("AV_DescHackingHediffRemoved".Translate(pawn.Named("PAWN")).AdjustedFor(pawn), pawn, MessageTypeDefOf.NeutralEvent);
            }
        }
        
        public override bool CanApplyOn(Thing target)
        {
            if (target is Pawn)
            {
                Pawn myTarget = target as Pawn;
                if (myTarget.Faction.IsPlayer)
                {
                    return false;
                }

                if (myTarget.RaceProps.IsMechanoid)
                {
                    float bandwidth_cost = myTarget.GetStatValue(StatDefOf.BandwidthCost);

                    if (bandwidth_cost <= MechtechSettings.HackingLanceBandwidth)
                    {
                        return true;
                    }
                }

            }

            return false;       
        }
        
    }
}
