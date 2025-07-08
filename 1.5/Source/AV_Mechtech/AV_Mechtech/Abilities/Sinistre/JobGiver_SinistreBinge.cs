using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace AV_Mechtech
{
    public class JobGiver_SinistreBinge : ThinkNode_JobGiver
    {
        private const int BaseIngestInterval = 600;

        private static readonly SimpleCurve IngestIntervalFactorCurve_DrugOverdose = new SimpleCurve
    {
        new CurvePoint(0f, 1f),
        new CurvePoint(1f, 5f)
    };

        //public int maxAmountToFeedOn => SinistreUtility.maxAmountToFeedOn(IngestibleSource);

        protected int IngestInterval(Pawn pawn)
        {
            int num = (int)((float)BaseIngestInterval * IngestIntervalFactorCurve_DrugOverdose.Evaluate(1f));
            return num;
        }

        protected Thing BestIngestTarget(Pawn pawn)
        {
            Predicate<Thing> validator = delegate (Thing t)
            {
                if (!IgnoreForbid(pawn) && t.IsForbidden(pawn))
                {
                    return false;
                }
                if (!pawn.CanReserve(t))
                {
                    return false;
                }
                /*
                if (!pawn.Position.InHorDistOf(t.Position, 60f) && !t.Position.Roofed(t.Map) && !pawn.Map.areaManager.Home[t.Position] && t.GetSlotGroup() == null)
                {
                    return false;
                }
                */
                return true;
            };



            Thing shard = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Shard), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);

            if(shard != null)
            {
                return shard;
            }
                
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Bioferrite), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);
        }

        protected bool IgnoreForbid(Pawn pawn)
        {

            return pawn.InMentalState || pawn.def == MechtechDefOfs.AV_Sinistre;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if(pawn.def != MechtechDefOfs.AV_Sinistre)
            {
                Log.Error("AV_Mechtech.JobGiver_SinistreBinge.TryGiveJob tried to give the job to a not sinistre pawn: " + pawn.Label);
                return null;
            }

            if (Find.TickManager.TicksGame - pawn.mindState.lastIngestTick > IngestInterval(pawn))
            {
                Job job = IngestJob(pawn);  
                if (job != null)
                {
                    return job;
                }
            }

            return null;
        }

        private Job IngestJob(Pawn pawn)    // this failss when the item is not ingestable
        {
            Thing thing = BestIngestTarget(pawn);
            if (thing == null)
            {
                return null;
            }

            //ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing);
            Job job = JobMaker.MakeJob(MechtechDefOfs.AV_SinistreIngest, thing);


            //job.count = finalIngestibleDef.ingestible.defaultNumToIngestAtOnce;
            job.ignoreForbidden = IgnoreForbid(pawn);
            //job.overeat = true;
            return job;
        }

    }
}
