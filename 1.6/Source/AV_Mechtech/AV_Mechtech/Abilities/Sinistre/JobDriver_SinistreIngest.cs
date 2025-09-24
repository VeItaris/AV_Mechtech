using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.AI;
using Verse;

namespace AV_Mechtech
{
    public class JobDriver_SinistreIngest : JobDriver
    {

        public const TargetIndex IngestibleSourceInd = TargetIndex.A;

        private const TargetIndex TableCellInd = TargetIndex.B;

        private const TargetIndex ExtraIngestiblesToCollectInd = TargetIndex.C;


        private Thing IngestibleSource => job.GetTarget(TargetIndex.A).Thing;

        private float ChewDurationMultiplier => 1f / pawn.GetStatValue(StatDefOf.EatingSpeed);

        public int maxAmountToPickup => SinistreUtility.maxAmountToFeedOn(IngestibleSource);
       

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Faction != null)
            {
                Thing ingestibleSource = IngestibleSource;
                
                if (!pawn.Reserve(ingestibleSource, job, 1, Math.Min(ingestibleSource.stackCount, maxAmountToPickup), null, errorOnFailed, ignoreOtherReservations: false))
                {
                    return false;
                }
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

/* 0 */     yield return ReserveFood();
/* 1 */     yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
/* 2 */     yield return ChewIngestible(pawn, ChewDurationMultiplier, TargetIndex.A, TargetIndex.B).FailOn((Toil x) => !IngestibleSource.Spawned && (pawn.carryTracker == null || pawn.carryTracker.CarriedThing != IngestibleSource)).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

            //yield return Toils_Ingest.FinalizeIngest(pawn, TargetIndex.A);
;
            //yield return Toils_Jump.JumpIf(chew, () => job.GetTarget(TargetIndex.A).Thing is Corpse && pawn.needs.food.CurLevelPercentage < 0.9f);
        }


        public static Toil ChewIngestible(Pawn chewer, float durationMultiplier, TargetIndex ingestibleInd, TargetIndex eatSurfaceInd = TargetIndex.None)
        {
            Toil toil = ToilMaker.MakeToil("ChewIngestible");
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Thing thing4 = actor.CurJob.GetTarget(ingestibleInd).Thing;

                toil.actor.pather.StopDead();
                actor.jobs.curDriver.ticksLeftThisToil = (int)(100 * durationMultiplier);
                if (thing4.Spawned)
                {
                    thing4.Map.physicalInteractionReservationManager.Reserve(chewer, actor.CurJob, thing4);
                }
            };
            toil.tickAction = delegate
            {
                if (chewer != toil.actor)
                {
                    toil.actor.rotationTracker.FaceCell(chewer.Position);
                }
                else
                {
                    Thing thing3 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                    if (thing3 != null && thing3.Spawned)
                    {
                        toil.actor.rotationTracker.FaceCell(thing3.Position);
                    }
                    else if (eatSurfaceInd != 0 && toil.actor.CurJob.GetTarget(eatSurfaceInd).IsValid)
                    {
                        toil.actor.rotationTracker.FaceCell(toil.actor.CurJob.GetTarget(eatSurfaceInd).Cell);
                    }
                } 
            };
            
            toil.WithProgressBar(ingestibleInd, delegate
            {
                Thing thing2 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                return (thing2 == null) ? 1f : (1f - (float)toil.actor.jobs.curDriver.ticksLeftThisToil / Mathf.Round((float)SinistreUtility.BioferriteIngestionTime * durationMultiplier));
            });
            
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.FailOnDestroyedOrNull(ingestibleInd);
            toil.AddFinishAction(delegate
            {
                Thing thing = chewer?.CurJob?.GetTarget(ingestibleInd).Thing;
                if (thing != null && chewer.Map.physicalInteractionReservationManager.IsReservedBy(chewer, thing))
                {
                    chewer.Map.physicalInteractionReservationManager.Release(chewer, toil.actor.CurJob, thing);
                }

                

                if (thing.stackCount > SinistreUtility.maxAmountToFeedOn(thing))
                {
                    int amount = SinistreUtility.maxAmountToFeedOn(thing);
                    SinistreUtility.Feed(chewer, thing.def, amount);
                    thing.stackCount -= amount;

                    TrySpawnEssenceFromShardEating(thing, chewer);
                }
                else
                {
                    SinistreUtility.Feed(chewer, thing.def, thing.stackCount);
                    TrySpawnEssenceFromShardEating(thing, chewer);
                    thing.Destroy();
                }


                    
            });
            toil.handlingFacing = true;
            //AddIngestionEffects(toil, chewer, ingestibleInd, eatSurfaceInd);
            return toil;
        }

        private static void TrySpawnEssenceFromShardEating(Thing thing, Pawn chewer)
        {
            if (thing.def == ThingDefOf.Shard)
            {
                Thing essence = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                essence.stackCount = 1;

                GenPlace.TryPlaceThing(essence, chewer.PositionHeld, chewer.MapHeld, ThingPlaceMode.Near, out var lastResultingThing);
                if (lastResultingThing != null)
                {
                    Messages.Message("AV_SinistreGiftOvereating".Translate(), MessageTypeDefOf.PositiveEvent);
                    SinistreUtility.TryCapAtPeak(chewer);
                }
                
            }
        }

        private Toil ReserveFood()
        {
            Toil toil = ToilMaker.MakeToil("ReserveFood");
            toil.initAction = delegate
            {
                if (pawn.Faction != null)
                {
                    Thing thing = job.GetTarget(TargetIndex.A).Thing;
                    if (pawn.carryTracker.CarriedThing != thing)
                    {
                        if (maxAmountToPickup != 0)
                        {
                            if (!pawn.Reserve(thing, job, 1, Math.Min(thing.stackCount, maxAmountToPickup)))
                            {
                                Log.Error(string.Concat("Pawn food reservation for ", pawn, " on job ", this, " failed, because it could not register food from ", thing, " - amount: ", maxAmountToPickup));
                                pawn.jobs.EndCurrentJob(JobCondition.Errored);
                            }
                            job.count = Math.Min(thing.stackCount, maxAmountToPickup);  //do I even need this????
                        }
                    }
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            toil.atomicWithPrevious = true;
            return toil;
        }

    }
}
