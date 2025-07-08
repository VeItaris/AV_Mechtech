using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using AV_Framework;

namespace AV_Mechtech
{
    public static class SinistreUtility
    {
        public static readonly int ResourceCost_CallSinistre = 30;
        public static readonly int PowerStageNeeded_CallSinistre = 3;

        public static readonly int ResourceCost_CallBackSinistre = 0;
        public static readonly int PowerStageNeeded_CallBackSinistre = 3;

        public static readonly int ResourceCost_CallSinistreShadows = 15;    // per shadow
        public static readonly int PowerStageNeeded_CallSinistreShadows = 2;

        public static readonly int ResourceCost_SinistreOvertake = 30;
        public static readonly int PowerStageNeeded_SinistreOvertake = 4;

        public static readonly int SinistreOvertakeDuration = 12000;

        public static readonly int MaxPowerStage = 4;

        public static readonly float BioferriteHungryThreshold = 0.5f; // below 50% == hungry
        public static readonly float BioferriteExtremlyHungryThreshold = 0f;
        public static readonly float BioterriteNeedPerDay =  5; // 5 bioferrite per day -> 25% hunger per day
        public static readonly float SpawnedHungryFactor = 3f;  // more hunger when spawned

        public static readonly float BioferriteSatisfyNeed = 0.05f;  // 5%
        public static readonly float ShardSatisfyNeed = 1.0f;    // 100%



        public static readonly int ComfyBioferriteCount = 50;
        public static readonly int ComfyShardCount = 2;

        public static readonly int BioferriteIngestionTime = 100;




        public static int AmountOfBioferriteToEat(float hunger)
        {
            return (int)((float)hunger / BioferriteSatisfyNeed);

        }

        public static int maxAmountToFeedOn(Thing IngestibleSource)
        {
            if (IngestibleSource.def == ThingDefOf.Shard)
            {
                return 1;
            }
            if (IngestibleSource.def == ThingDefOf.Bioferrite)
            {
                return 3;
            }

            return 1;
        }

        public static void Feed(Pawn p, ThingDef def, int count)
        {
            Comp_SinistreNeeds comp = p.TryGetComp<Comp_SinistreNeeds>();

            if(comp == null)
            {
                Log.Error("AV_Mechtech.SinistreUtility.Feed: tried to feed a pawn without Comp_SinistreNeeds");
                return;
            }
            if(def != ThingDefOf.Shard && def != ThingDefOf.Bioferrite)
            {
                Log.Error("AV_Mechtech.SinistreUtility.Feed: tried to feed Comp_SinistreNeeds with wrong food");
                return;
            }

            if (def == ThingDefOf.Shard)
            {
                comp.EatAmount(count * ShardSatisfyNeed);
            }
            else if (def == ThingDefOf.Bioferrite)
            {
                comp.EatAmount(count * BioferriteSatisfyNeed);
            }
        }

    }
}
