using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using AV_Framework;

namespace AV_Mechtech
{
    public class Verb_CastAbilityRocketswarm : Verb_LaunchProjectile
    {
        protected override int ShotsPerBurst => verbProps.burstShotCount;

        public override ThingDef Projectile => verbProps.defaultProjectile;

        public int ResourceCostPerShot = TarantulaUtility.ResourceCostPerShot;


        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (currentTarget.Thing is Pawn pawn && !pawn.Downed && !pawn.IsColonyMech && CasterIsPawn && CasterPawn.skills != null)
            {
                float num = (pawn.HostileTo(caster) ? 170f : 20f);
                float num2 = verbProps.AdjustedFullCycleTime(this, CasterPawn);
                CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2);
            }
        }

        protected IntVec3 GetForcedMissTarget(float forcedMissRadius)   //throws errors from base, probably neeed nugget
        {
            /*
            if (verbProps.forcedMissEvenDispersal)
            {
                if (forcedMissTargetEvenDispersalCache.Count <= 0)
                {
                    forcedMissTargetEvenDispersalCache.AddRange(GenerateEvenDispersalForcedMissTargets(currentTarget.Cell, forcedMissRadius, burstShotsLeft));
                    forcedMissTargetEvenDispersalCache.SortByDescending((IntVec3 p) => p.DistanceToSquared(Caster.Position));
                }
                if (forcedMissTargetEvenDispersalCache.Count > 0)
                {
                    return forcedMissTargetEvenDispersalCache.Pop();
                }
            }
            */
            int maxExclusive = GenRadial.NumCellsInRadius(forcedMissRadius);
            int num = Rand.Range(0, maxExclusive);
            return currentTarget.Cell + GenRadial.RadialPattern[num];
        }

        CompMechReloadableResourceHolder ResourceHolder => caster.TryGetComp<CompMechReloadableResourceHolder>();

        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            ThingDef projectile = Projectile;
            if (projectile == null)
            {
                return false;
            }

            if(ResourceHolder == null)
            {
                return false;
            }

            ShootLine resultingLine;
            bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out resultingLine);
            if (verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }

            lastShotTick = Find.TickManager.TicksGame;
            Thing manningPawn = caster;

            Vector3 drawPos = caster.DrawPos + TarantulaUtility.Rocketoffset(CasterPawn, ResourceHolder);
            Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, resultingLine.Source, caster.Map);

            if (verbProps.ForcedMissRadius > 0.5f)
            {
                float num = verbProps.ForcedMissRadius;

                float num2 = VerbUtility.CalculateAdjustedForcedMiss(num, currentTarget.Cell - caster.Position);
                if (num2 > 0.5f)
                {
                    IntVec3 forcedMissTarget = GetForcedMissTarget(num2);
                    if (forcedMissTarget != currentTarget.Cell)
                    {
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }

                        if (!canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                        if (PayForProjectileOrDontShoot())
                        {
                            projectile2.Launch(manningPawn, drawPos, forcedMissTarget, currentTarget, projectileHitFlags, preventFriendlyFire);
                            //Log.Warning("AV_Mechtech.Verb_CastAbilityRocketswarm.TryCastShot() -> 0");
                        }
                            
                        return true;
                    }
                }
            }

            ShotReport shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = randomCoverToMissInto?.def;
            if (verbProps.canGoWild && !Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                bool flyOverhead = projectile2?.def?.projectile != null && projectile2.def.projectile.flyOverhead;
                resultingLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget, flyOverhead, caster.Map);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                if (PayForProjectileOrDontShoot())
                {
                    projectile2.Launch(manningPawn, drawPos, resultingLine.Dest, currentTarget, projectileHitFlags2, preventFriendlyFire, null, targetCoverDef);

                    //Log.Warning("AV_Mechtech.Verb_CastAbilityRocketswarm.TryCastShot() -> 1");
                }
                    

                return true;
            }

            if (currentTarget.Thing != null && currentTarget.Thing.def.CanBenefitFromCover && !Rand.Chance(shotReport.PassCoverChance))
            {
                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
                if (PayForProjectileOrDontShoot())
                {
                    projectile2.Launch(manningPawn, drawPos, randomCoverToMissInto, currentTarget, projectileHitFlags3, preventFriendlyFire, null, targetCoverDef);
                    //Log.Warning("AV_Mechtech.Verb_CastAbilityRocketswarm.TryCastShot() -> 2");
                }
                   
                return true;
            }

            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }

            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }


            if (currentTarget.Thing != null)
            {
                if (PayForProjectileOrDontShoot())
                {
                    projectile2.Launch(manningPawn, drawPos, currentTarget, currentTarget, projectileHitFlags4, preventFriendlyFire, null, targetCoverDef);
                    //Log.Warning("AV_Mechtech.Verb_CastAbilityRocketswarm.TryCastShot() -> 3");
                }
                    
            }
            else
            {
                if (PayForProjectileOrDontShoot())
                {
                    projectile2.Launch(manningPawn, drawPos, resultingLine.Dest, currentTarget, projectileHitFlags4, preventFriendlyFire, null, targetCoverDef);
                    //Log.Warning("AV_Mechtech.Verb_CastAbilityRocketswarm.TryCastShot() -> 4");
                }
                
            }

            return true;
        }

        public bool PayForProjectileOrDontShoot()
        {
            if(ResourceHolder.HasResources(ResourceCostPerShot))
            {
                ResourceHolder.UseResources(ResourceCostPerShot);

                CasterPawn.Drawer.renderer.SetAllGraphicsDirty();
                return true;
            }
            else
            {
                //Log.Warning("PayForProjectileOrDontShoot() not enough resources");
                return false;
            }
        }


        public override bool Available()
        {
            if (ResourceHolder == null || !ResourceHolder.HasResources(ResourceCostPerShot))
            {
                return false;
            }

            //Log.Warning("Available resources: " + ResourceHolder.IngredientCount);

            if (!base.Available())
            {
                return false;
            }

            if (CasterIsPawn)
            {
                Pawn casterPawn = CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && !verbProps.ai_ProjectileLaunchingIgnoresMeleeThreats && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }

            return Projectile != null;
        }


    }
}
