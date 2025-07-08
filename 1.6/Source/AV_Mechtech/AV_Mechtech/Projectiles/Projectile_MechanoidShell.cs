using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace AV_Mechtech
{
    public class Projectile_MechanoidShell : Projectile_Explosive //Bullet
    {
        private static readonly PawnKindDef mech = MechtechDefOfs.Mech_WarUrchin; // PawnKindDef.Named("Mech_WarUrchin");

        private readonly int spawntimes = 1;

        private bool HitThickRoof = false;

        private bool HitImpassable = false; //water is still fine

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
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            //this spawns the shell in ground! (Gen explosion)
            if (!HitThickRoof && !HitImpassable)
            {
                GenExplosion.DoExplosion(base.Position, base.Map, def.projectile.explosionRadius, def.projectile.damageDef, launcher, DamageAmount, ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, postExplosionSpawnThingDefWater: def.projectile.postExplosionSpawnThingDefWater, postExplosionSpawnChance: def.projectile.postExplosionSpawnChance, postExplosionSpawnThingCount: def.projectile.postExplosionSpawnThingCount, postExplosionGasType: def.projectile.postExplosionGasType, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination), ignoredThings: null, affectedAngle: null, doVisualEffects: true, propagationSpeed: def.projectile.damageDef.expolosionPropagationSpeed, excludeRadius: 0f, doSoundEffects: true, screenShakeFactor: def.projectile.screenShakeFactor);
                DeployMech();
            } 
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            Destroy();
        }


        protected override void ImpactSomething()
        {
            if (def.projectile.flyOverhead)
            {
                RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
                if (roofDef != null)
                {
                    if (roofDef.isThickRoof)
                    {
                        HitThickRoof = true;
                        //PollutionUtility.GrowPollutionAt(base.Position, base.Map, pollutedTiles);  //new, rest is vanilla
                        //ThrowDebugText("hit-thick-roof", base.Position);
                        def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(base.Position, base.Map));
                        Destroy();
                        return;
                    }

                    if (base.Position.GetEdifice(base.Map) == null || base.Position.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
                    {
                        RoofCollapserImmediate.DropRoofInCells(base.Position, base.Map);
                        HitThickRoof = false;
                    }
                }
                else
                {
                    HitThickRoof = false;
                }

                if (HitWall())
                {
                    HitImpassable = true;

                    def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(base.Position, base.Map));
                    Destroy();
                    return;
                }
                else
                {
                    HitImpassable = false;
                }
            }
            Impact(null);
        }

        private bool HitWall()
        {
            Building edifice = base.Position.GetEdifice(base.Map);

            if (edifice != null && (edifice.def.IsSmoothed || edifice.def.defName.ToLower().Contains("wall")))
            {
                return true;
            }
            return false;
        }

        private void DeployMech()
        {
            Faction faction = Faction.OfAncientsHostile;

            for (int i = 0; i < spawntimes; i++)
            {
                Pawn pawn = GenSpawn.Spawn(PawnGenerator.GeneratePawn(mech, faction), base.Position, base.Map) as Pawn;

                pawn.SetFaction(faction);

            }
        }
    }
}