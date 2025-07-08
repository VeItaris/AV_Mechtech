using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AV_Mechtech
{

    public class DeathActionWorker_FoamExplosion : DeathActionWorker
    {
        public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

        public override bool DangerousInMelee => false; //changed to false, not sure what the impact is

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
           GenExplosion.DoExplosion(center: corpse.Position, map: corpse.Map, radius: 1.9f, damType: DamageDefOf.Extinguish, instigator: corpse.InnerPawn, explosionSound: SoundDefOf.Explosion_FirefoamPopper, postExplosionSpawnThingDef: ThingDefOf.Filth_FireFoam, postExplosionSpawnThingCount: 1, postExplosionSpawnChance: 1f);
            
        }
    }
}


