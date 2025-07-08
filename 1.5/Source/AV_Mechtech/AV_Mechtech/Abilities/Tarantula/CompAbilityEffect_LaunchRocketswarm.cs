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
    public class CompAbilityEffect_LaunchRocketswarm : CompAbilityEffect
    {
        public new CompProperties_AbilityLaunchRocketswarm Props => (CompProperties_AbilityLaunchRocketswarm)props;

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
            LaunchProjectile(target);
        }

        private void LaunchProjectile(LocalTargetInfo target)
        {
            if (Props.projectileDef != null)
            {
                Pawn pawn = parent.pawn;

                ((Projectile)GenSpawn.Spawn(Props.projectileDef, pawn.Position, pawn.Map)).Launch(pawn, pawn.DrawPos, target, target, ProjectileHitFlags.IntendedTarget);
            }
        }

        //this shit is an int...
        /*
        private IntVec3 RocketStartPos(Pawn pawn)
        {
            IntVec3 firingPosition = pawn.Position;


            if (pawn.Rotation == Rot4.East)
            {
                firingPosition += TarantulaUtility.WeaponOffsetEast;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                firingPosition += TarantulaUtility.WeaponOffsetWest;

            }
            else if (pawn.Rotation == Rot4.North)
            {
                firingPosition += TarantulaUtility.WeaponOffsetNorth;
            }
            else if (pawn.Rotation == Rot4.South)
            {
                firingPosition += TarantulaUtility.WeaponOffsetSouth;

            }

            return firingPosition;
        }
        */


        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (ResourceHolder == null || !ResourceHolder.HasResources(ResourceCostPerShot))
            {
                return false;
            }

            if (!TarantulaUtility.HasUseableMissileSiloBodyPart(parent.pawn))
            {
                return false;
            }

            if(!MechtechSettings.AllowTarantulaPlayerAIMissiles && parent.pawn.IsColonyMech) // && !parent.pawn.Drafted)
            {
                return false;
            }

            return target.Pawn != null;
        }


    }
}
