using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AV_Mechtech
{
    public class Projectile_ToxicShell : Projectile_Explosive
    {
        private int pollutedTiles = 18;


        protected override void Explode()
        {
            Map map = base.Map;
            Destroy();
            if (def.projectile.explosionEffect != null)
            {
                Effecter effecter = def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
                effecter.Cleanup();
            }
            GenExplosion.DoExplosion(base.Position, map, def.projectile.explosionRadius, def.projectile.damageDef, launcher, DamageAmount, ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, postExplosionSpawnThingDefWater: def.projectile.postExplosionSpawnThingDefWater, postExplosionSpawnChance: def.projectile.postExplosionSpawnChance, postExplosionSpawnThingCount: def.projectile.postExplosionSpawnThingCount, postExplosionGasType: def.projectile.postExplosionGasType, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination), ignoredThings: null, affectedAngle: null, doVisualEffects: true, propagationSpeed: def.projectile.damageDef.expolosionPropagationSpeed, excludeRadius: 0f, doSoundEffects: true, screenShakeFactor: def.projectile.screenShakeFactor);
        }

        protected override void ImpactSomething()
        {

            //bool m = GridsUtility.CanPollute(base.Position, base.Map);
            //Log.Message("[AV]Mechtech.Projectile_ToxicShell: Can Pollute is " + m.ToString() + ", patch PollutionGrid.EverPollutable to override vanilla behaivior and make walls polluteable?");
            PollutionUtility.GrowPollutionAt(base.Position, base.Map, pollutedTiles);  //new, rest is vanilla   // mountaintile


            if (def.projectile.flyOverhead)
            {
                RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
                if (roofDef != null)
                {
                    if (roofDef.isThickRoof)
                    {
                        //PollutionUtility.GrowPollutionAt(base.Position, base.Map, pollutedTiles);  //new, rest is vanilla
                        //ThrowDebugText("hit-thick-roof", base.Position);
                        def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(base.Position, base.Map));
                        Destroy();
                        return;
                    }

                    if (base.Position.GetEdifice(base.Map) == null || base.Position.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
                    {
                        RoofCollapserImmediate.DropRoofInCells(base.Position, base.Map);
                    }
                }
            }

            if (usedTarget.HasThing && CanHit(usedTarget.Thing))
            {
                Pawn pawn = usedTarget.Thing as Pawn;
                if (pawn != null && pawn.GetPosture() != 0 && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && !Rand.Chance(0.2f))
                {
                    //ThrowDebugText("miss-laying", base.Position);
                    Impact(null);
                }
                else
                {
                    Impact(usedTarget.Thing);
                }

                return;
            }

            List<Thing> list = VerbUtility.ThingsToHit(base.Position, base.Map, CanHit);
            list.Shuffle();
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                Pawn pawn2 = thing as Pawn;
                float num;
                if (pawn2 != null)
                {
                    num = 0.5f * Mathf.Clamp(pawn2.BodySize, 0.1f, 2f);
                    if (pawn2.GetPosture() != 0 && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f)
                    {
                        num *= 0.2f;
                    }

                    if (launcher != null && pawn2.Faction != null && launcher.Faction != null && !pawn2.Faction.HostileTo(launcher.Faction))
                    {
                        num *= VerbUtility.InterceptChanceFactorFromDistance(origin, base.Position);
                    }
                }
                else
                {
                    num = 1.5f * thing.def.fillPercent;
                }

                if (Rand.Chance(num))
                {
                    //ThrowDebugText("hit-" + num.ToStringPercent(), base.Position);
                    Impact(list.RandomElement());
                    return;
                }

                //ThrowDebugText("miss-" + num.ToStringPercent(), base.Position);
            }

            Impact(null);
        }


    }
}