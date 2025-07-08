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
using AV_Framework;

namespace AV_Mechtech
{
    public class DeathActionWorker_Reshaper : DeathActionWorker
    {
        public DeathActionProperties_Reshaper Props => (DeathActionProperties_Reshaper)props;

        private bool SpawningAllowed = false;

        private int TimesToTrigger = 0;

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            if (!ModLister.CheckAnomaly("Pawn dividing"))
            {
                return;
            }
            Pawn innerPawn = corpse.InnerPawn;
            if (innerPawn == null)
            {
                return;
            }

            SpawningAllowed = false;

            if (Props.neededResources > 0)
            {
                if (innerPawn.HasComp<CompMechReloadableResourceHolder>())
                {
                    CompMechReloadableResourceHolder comp = innerPawn.TryGetComp<CompMechReloadableResourceHolder>();

                    if (comp != null)
                    {
                        if (comp.IngredientCount >= Props.neededResources)
                        {

                            TimesToTrigger = comp.IngredientCount / Props.neededResources;

                            comp.UseResources(Props.neededResources);
                            SpawningAllowed = true;
                        }

                    }
                }
            }
            else
            {
                SpawningAllowed = true;
            }

            if (SpawningAllowed)
            {
                //int dividePawnCount = Props.dividePawnCount;
                for (int i = 0; i < TimesToTrigger; i++)
                {
                    Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.dividePawnKindOptions.RandomElement(), corpse.InnerPawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f));
                    SpawnPawn(child, innerPawn, corpse.PositionHeld, corpse.MapHeld, prevLord);
                }
                foreach (PawnKindDef item in Props.dividePawnKindAdditionalForced)
                {
                    Pawn child2 = PawnGenerator.GeneratePawn(new PawnGenerationRequest(item, corpse.InnerPawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f));
                    SpawnPawn(child2, innerPawn, corpse.PositionHeld, corpse.MapHeld, prevLord);
                }
                MechSplatter(Props.divideBloodFilthCountRange.RandomInRange, corpse.PositionHeld, corpse.MapHeld);
                FilthMaker.TryMakeFilth(corpse.PositionHeld, corpse.MapHeld, ThingDefOf.Filth_MachineBits);
            }
        
        
            /*
            if(innerPawn.Faction == Faction.OfPlayer)
            {
                ResurrectionUtility.TryResurrect(innerPawn);
                innerPawn.DeSpawn();
            }
            */
        
        }

        private void SpawnPawn(Pawn child, Pawn parent, IntVec3 position, Map map, Lord lord)
        {
            GenSpawn.Spawn(child, position, map, WipeMode.VanishOrMoveAside);
            lord?.AddPawn(child);
            CompInspectStringEmergence compInspectStringEmergence = child.TryGetComp<CompInspectStringEmergence>();
            if (compInspectStringEmergence != null)
            {
                compInspectStringEmergence.sourcePawn = parent;
            }
            FleshbeastUtility.SpawnPawnAsFlyer(child, map, position);
        }

        public void MechSplatter(int filthCount, IntVec3 pos, Map map)
        {
            EffecterDefOf.MetalhorrorDeath.Spawn(pos, map).Cleanup();

            CellRect cellRect = new CellRect(pos.x, pos.z, 3, 3).ClipInsideMap(map);
            for (int i = 0; i < filthCount; i++)
            {
                IntVec3 randomCell = cellRect.RandomCell;
                ThingDef filthDef = (Rand.Bool ? ThingDefOf.Filth_MetalhorrorDebris : ThingDefOf.Filth_MachineBits);
                if (randomCell.InBounds(map) && GenSight.LineOfSight(randomCell, pos, map))
                {
                    FilthMaker.TryMakeFilth(randomCell, map, filthDef);
                }
            }
        }

    }
}


