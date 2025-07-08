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

    public static class TarantulaUtility
    {
        public static int ResourceCostPerShot = 15;

        public static int MissileDamage = 24;

        public static float MissileExtraSelfDamage = MissileDamage * 1.0f;

        public static Vector3 WeaponOffsetEast = new Vector3(0.7f, 0, -0.2f);

        public static Vector3 WeaponOffsetWest = new Vector3(-0.7f, 0, -0.2f);

        public static Vector3 WeaponOffsetNorth = new Vector3(0, 0, 0.4f);

        public static Vector3 WeaponOffsetSouth = new Vector3(0, 0, -0.55f);


        public static Vector3 RocketOffsetEast = new Vector3(0.5f, 0, 0.5f);

        public static Vector3 RocketOffsetWest = new Vector3(0.5f, 0, 0.5f);

        public static Vector3 RocketOffsetNorth_Left = new Vector3(0.5f, 0, 0.5f);

        public static Vector3 RocketOffsetNorth_Right = new Vector3(-0.5f, 0, 0.5f);

        public static Vector3 RocketOffsetSouth_Left = new Vector3(0.5f, 0, 0.5f);

        public static Vector3 RocketOffsetSouth_Right = new Vector3(-0.5f, 0, 0.5f);


        public static Vector3 Rocketoffset(Pawn pawn, CompMechReloadableResourceHolder comp)
        {
            Vector3 Offset = Vector3.zero;

            if (pawn.Rotation == Rot4.East)
            {
                Offset += RocketOffsetEast;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                Offset += RocketOffsetWest;
            }
            else if (pawn.Rotation == Rot4.North)
            {
                if (CurrentMissileToFire(pawn, comp) % 2 == 0)   //is even
                {
                    Offset += RocketOffsetNorth_Left;
                }
                else
                {
                    Offset += RocketOffsetNorth_Right;
                }
            }
            else if (pawn.Rotation == Rot4.South)
            {
                if(CurrentMissileToFire(pawn, comp) % 2 == 0)   //is even
                {
                    Offset += RocketOffsetSouth_Right;
                }
                else
                {
                    Offset += RocketOffsetSouth_Left;
                }

                /*
                if (Rand.Chance(0.5f))
                {
                    Offset += RocketOffsetSouth_Right;
                }
                else
                {
                    Offset += RocketOffsetSouth_Left;
                }
                */
            }

            return Offset;
        }


        public static int MissilesInLauncher(CompMechReloadableResourceHolder comp)
        {
            return comp.IngredientCount / ResourceCostPerShot;
        }
        public static int CurrentMissileToFire(Pawn pawn, CompMechReloadableResourceHolder comp)
        {

            int useableMissiles = MissilesInLauncher(comp);

            if (useableMissiles > 16)
            { useableMissiles = 16; }
            if (useableMissiles < 0)
            { useableMissiles = 0; }

            return useableMissiles;
        }

        public static bool OwnsMissileSilo(Pawn pawn, out BodyPartRecord silo)
        {
            List<BodyPartRecord> list = pawn.def.race.body.GetPartsWithDef(MechtechDefOfs.AV_MissileSilo);

            if(!list.NullOrEmpty())
            {
                silo = list.First();
            }
            else
            {
                silo = null;
            }

            return !list.NullOrEmpty();
        }

        public static bool HasHealthyMissileSilo(Pawn pawn, out bool HasSilo)
        {
            HasSilo = false;

            List<BodyPartRecord> list = pawn.def.race.body.GetPartsWithDef(MechtechDefOfs.AV_MissileSilo);

            if (!list.NullOrEmpty())
            {
                HasSilo = true;

                if(pawn.health.hediffSet.PartIsMissing(list.First()))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public static BodyPartRecord GetMissileSiloBodyPartRecord(Pawn pawn)
        {
            List<BodyPartRecord> list = pawn.def.race.body.GetPartsWithDef(MechtechDefOfs.AV_MissileSilo);

            if (!list.NullOrEmpty())
            {
                BodyPartRecord launcher = list.First();
                if (pawn.health.hediffSet.PartIsMissing(launcher))
                {
                    return null;
                }
                return launcher;
            }
            return null;
        }


        public static bool HasUseableMissileSiloBodyPart(Pawn pawn)
        {
            bool useable = HasHealthyMissileSilo(pawn, out bool hasSilo);

            return useable && hasSilo;
        }
    
        public static bool IsSiloDestroyed(Pawn pawn, CompMechReloadableResourceHolder comp)
        {
            if(OwnsMissileSilo(pawn, out BodyPartRecord silo) && silo != null)
            {
                return pawn.health.hediffSet.PartIsMissing(silo);
            }
            return false;
        }

        public static void ExplodeAllRemainingMissiles(Pawn pawn, CompMechReloadableResourceHolder comp)
        {
            int useableMissiles = MissilesInLauncher(comp);

            if (useableMissiles > 0)
            {
                comp.UseAllResources();

                DamageInfo dinfo = new DamageInfo(DamageDefOf.Bomb, MissileExtraSelfDamage);

                for (int i = 0; i < useableMissiles; i++)
                {
                    GenExplosion.DoExplosion(center: pawn.Position, map: pawn.Map, radius: 3.9f, damType: DamageDefOf.Bomb, pawn, explosionSound: MechtechDefOfs.Explosion_Rocket, damAmount: MissileDamage);

                    if(!pawn.Dead)
                    {
                        pawn.TakeDamage(dinfo);
                    }
                }
            }
        }
    
    
    }
}
