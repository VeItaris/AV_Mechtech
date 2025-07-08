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
    public class CompAbilityEffect_BlowUpMissileLauncher : CompAbilityEffect
    {
        public new CompProperties_AbilityBlowUpMissileLauncher Props => (CompProperties_AbilityBlowUpMissileLauncher)props;

        CompMechReloadableResourceHolder ResourceHolder => parent.pawn.TryGetComp<CompMechReloadableResourceHolder>();

        public int ResourceCostPerShot = TarantulaUtility.ResourceCostPerShot;

        public override bool ShouldHideGizmo => HideGizmo();

        public override bool GizmoDisabled(out string reason)
        {
            if (ResourceHolder != null && !ResourceHolder.HasResources(ResourceCostPerShot))
            {
                reason = "MechCarrierNotEnoughResources".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        private bool HideGizmo()
        {
            if (ResourceHolder == null)
            {
                return true;
            }

            if (!TarantulaUtility.HasUseableMissileSiloBodyPart(parent.pawn))
            {
                return true;
            }

            return false;
        }


        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            BodyPartRecord launcher = TarantulaUtility.GetMissileSiloBodyPartRecord(parent.pawn);

            if (launcher != null)
            {
                parent.pawn.health.AddHediff(HediffDefOf.MissingBodyPart, launcher);
            }
        }


    }
}
