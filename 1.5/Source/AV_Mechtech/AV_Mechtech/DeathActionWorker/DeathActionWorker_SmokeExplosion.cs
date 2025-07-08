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
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static HarmonyLib.Code;
using Verse.AI.Group;

namespace AV_Mechtech
{

    public class DeathActionWorker_SmokeExplosion : DeathActionWorker
    {
        public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

        public override bool DangerousInMelee => false; //changed to false, not sure what the impact is

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            GenExplosion.DoExplosion(center: corpse.Position, map: corpse.Map, radius: 1.9f, damType: DamageDefOf.Smoke, null, -1, -1f, null, null, null, null, null, 0f, 1, GasType.BlindSmoke);

            //GenExplosion.DoExplosion(center: corpse.Position, map: corpse.Map, radius: 1.9f, damType: DamageDefOf.Smoke, instigator: corpse.InnerPawn);    //explosionSound: SoundDefOf.pop   add soundeffect?
        }
    }
}


