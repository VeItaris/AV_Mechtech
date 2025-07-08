using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI.Group;
using Verse;
using RimWorld.Planet;

namespace AV_Mechtech
{
    public class DeathActionWorker_Sinistre : DeathActionWorker
    {
        //public GameComponent_Sinistre gameComp => Current.Game.GetComponent<GameComponent_Sinistre>();

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {

            MechSplatter(5, corpse.PositionHeld, corpse.MapHeld);

            Comp_SinistreNeeds comp = corpse.InnerPawn.TryGetComp<Comp_SinistreNeeds>();

            Log.Message("DeathActionWorker_Sinistre 1");

            if (comp != null && comp.BioferriteNeed >= 1f)
            {
                comp.BioferriteNeed -= 1f;

                Thing chip = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                chip.stackCount = 1;

                GenPlace.TryPlaceThing(chip, corpse.PositionHeld, corpse.MapHeld, ThingPlaceMode.Near, out var lastResultingThing);
            }

            Log.Message("DeathActionWorker_Sinistre 2");


            if (corpse.InnerPawn.Faction == Faction.OfPlayer)
            {
                //ResurrectionUtility.TryResurrect(corpse.InnerPawn);

                //corpse.InnerPawn.DeSpawn();

                //corpse.DeSpawn();
            }
            else
            {
                corpse.Destroy(); //destoy discards the pawn
            }

            //
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
