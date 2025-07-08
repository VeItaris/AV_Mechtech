using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class HediffComp_GiveHediffsInRange_Commander : HediffComp
    {
        private Mote mote;
        public HediffCompProperties_GiveHediffsInRange_Commander Props => (HediffCompProperties_GiveHediffsInRange_Commander)props;

        int counter = 0;

        readonly int ticksToCheck = 60;

        public override void CompPostTick(ref float severityAdjustment)
        {

            if (!parent.pawn.Awake() || parent.pawn.health == null || parent.pawn.health.InPainShock || !parent.pawn.Spawned)
            {
                return;
            }

            if (!Props.hideMoteWhenNotDrafted || parent.pawn.Drafted)   //circle, not the lines!
            {
                if (Props.mote != null && (mote == null || mote.Destroyed))
                {
                    mote = MoteMaker.MakeAttachedOverlay(parent.pawn, Props.mote, Vector3.zero);
                }
                mote?.Maintain();   //only runs if mote exists
            }

            counter++;
            if (counter >= ticksToCheck)
            {
                counter = 0;
                CheckForAura();
            }        
        }

        public void CheckForAura()
        {
            IReadOnlyList<Pawn> readOnlyList = ((!Props.onlyPawnsInSameFaction || parent.pawn.Faction == null) ? parent.pawn.Map.mapPawns.AllPawnsSpawned : parent.pawn.Map.mapPawns.SpawnedPawnsInFaction(parent.pawn.Faction));

            foreach (Pawn pawn in readOnlyList)
            {
                if (!pawn.RaceProps.Humanlike && !pawn.RaceProps.IsMechanoid || pawn.Dead || pawn.health == null || pawn == parent.pawn || !(pawn.Position.DistanceTo(parent.pawn.Position) <= Props.range) || !Props.targetingParameters.CanTarget(pawn))
                {
                    continue;
                }

                if (pawn.RaceProps.IsMechanoid)
                {
                    AddAuraBuff(pawn);
                    continue;
                }

 
                if (pawn.RaceProps.Humanlike && (MechanitorUtility.IsMechanitor(pawn) || HasUseableApparel(pawn)))
                {
                    AddAuraBuff(pawn);
                }
            }
        }

        public bool HasUseableApparel(Pawn pawn)
        {
            if(MechtechDefOfs.Apparel_Gunlink != null)
            {
                foreach (Apparel item in pawn.apparel.WornApparel)
                {
                    if (item.def == MechtechDefOfs.Apparel_Gunlink)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public void AddAuraBuff(Pawn pawn)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
            if (hediff == null)
            {
                hediff = pawn.health.AddHediff(Props.hediff, pawn.health.hediffSet.GetBrain());
                hediff.Severity = Props.initialSeverity;
                HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
                if (hediffComp_Link != null)
                {
                    hediffComp_Link.drawConnection = true;
                    hediffComp_Link.other = parent.pawn;
                }
            }
            HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
            if (hediffComp_Disappears == null)
            {
                Log.Error("HediffComp_GiveHediffsInRange has a hediff in props which does not have a HediffComp_Disappears");
            }
            else
            {
                hediffComp_Disappears.ticksToDisappear = 120;
            }
        }

    }
}
