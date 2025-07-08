using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Verse.AI.Group;
using Verse;
using AV_Framework;


namespace AV_Mechtech
{
    [Obsolete]
    public class Ability_Rocketswarm : Ability, IVerbOwner, IExposable, ILoadReferenceable
    {
        CompMechReloadableResourceHolder ResourceHolder => pawn.TryGetComp<CompMechReloadableResourceHolder>();

        int ResourceCostPerShot = TarantulaUtility.ResourceCostPerShot;

        public override bool CanCast
        {
            get
            {
                if (ResourceHolder == null || !ResourceHolder.HasResources(ResourceCostPerShot))
                {
                    return false;
                }

                return base.CanCast;
            }
        }

        public override bool GizmoDisabled(out string reason)   //this is not even called -> used CompProperties_AbilityLaunchRocketswarm to hide gizmo instead
        {
            if (ResourceHolder == null || !ResourceHolder.HasResources(ResourceCostPerShot))
            {
                reason = "AbilityNoCharges".Translate();
                return true;
            }

            bool hasSilo = TarantulaUtility.HasHealthyMissileSilo(pawn, out bool shouldHaveSilo);

            if (shouldHaveSilo && !hasSilo)
            {
                reason = "MissingBodyPart".Translate();
                return true;
            }

            return base.GizmoDisabled(out reason);

        }


    }
}
