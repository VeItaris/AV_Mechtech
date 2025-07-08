using AV_Framework;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class CompAbilityEffect_SinistreOvertake : CompAbilityEffect
    {
        public new CompProperties_AbilitySinistreOvertake Props => (CompProperties_AbilitySinistreOvertake)props;

        private CompMechReloadableResourceHolder ResourceHolder => parent.pawn.TryGetComp<CompMechReloadableResourceHolder>();

        private Comp_WanderingSinistre wanderingSinistre => parent.pawn.TryGetComp<Comp_WanderingSinistre>();

        public readonly int ResourceCostPerCast = SinistreUtility.ResourceCost_CallSinistre;

        public readonly int PowerStageNeeded = SinistreUtility.PowerStageNeeded_CallSinistre;

        public override bool ShouldHideGizmo => HideGizmo();

        public override bool GizmoDisabled(out string reason)
        {
            if (ResourceHolder != null && !ResourceHolder.HasResources(ResourceCostPerCast))
            {
                reason = "MechCarrierNotEnoughResources".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        private bool HideGizmo()
        {
            if (ResourceHolder == null || wanderingSinistre == null)
            {
                return true;
            }

            if(!wanderingSinistre.SinistreAttached ||  wanderingSinistre.powerStage < PowerStageNeeded)
            {
                return true;
            }

            return false;
        }


        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            parent.pawn.drafter.Drafted = true;
            Hediff overtake = parent.pawn.health.GetOrAddHediff(MechtechDefOfs.AV_SinistreOvertake);

           HediffComp_Disappears hediffComp_Disappears = overtake.TryGetComp<HediffComp_Disappears>();
            if (hediffComp_Disappears != null)
            {
                hediffComp_Disappears.ticksToDisappear = SinistreUtility.SinistreOvertakeDuration;
            }
        }

    }
}
