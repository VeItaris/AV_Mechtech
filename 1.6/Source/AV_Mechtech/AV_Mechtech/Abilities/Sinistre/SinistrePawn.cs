using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using AV_Framework;
using UnityEngine;

namespace AV_Mechtech
{
    public class SinistrePawn : Pawn
    {

        public static GameComponent_Sinistre gameComp => Current.Game.GetComponent<GameComponent_Sinistre>();


        public void TryDropEssence()
        {
            //Log.Message("Drop essence");

            Map map = base.Map;
            Map map2 = (prevMap = base.MapHeld);
            Comp_SinistreNeeds comp = this.TryGetComp<Comp_SinistreNeeds>();

            if (comp != null && comp.BioferriteNeed >= 1f)
            {
                if (map != null)
                {
                    comp.BioferriteNeed -= 0.8f;

                    Thing chip = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                    chip.stackCount = 1;

                    GenPlace.TryPlaceThing(chip, base.PositionHeld, map, ThingPlaceMode.Near, out var lastResultingThing);
                }
                else if (map2 != null)
                {
                    comp.BioferriteNeed -= 0.8f;

                    Thing chip = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                    chip.stackCount = 1;

                    GenPlace.TryPlaceThing(chip, base.PositionHeld, map, ThingPlaceMode.Near, out var lastResultingThing);

                }
                else
                {
                    Log.Warning("AV_Mechtech.SinistrePawn.DropEssence: Failed to check sinistre essence drop, skipping...");
                }
            }

        }




        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            TryDropEssence();

            //Log.Warning("SinistrePawn -> Destroy!");
            if (Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(this);
                //DeSpawn();    // throws error
            }
            else
            {
                base.Destroy(mode);
            }

        }

        /*
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
 
         
        */




        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            TryDropEssence();

            if (Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(this);
            }
            //Log.Warning("SinistrePawn -> Kill!");
            base.Kill(dinfo, exactCulprit);
        }
        

        public override void Discard(bool silentlyRemoveReferences = false)
        {
            TryDropEssence();
            if (Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(this);
                //Log.Warning("SinistrePawn -> F U rimworld, no discarding of my pawns!");
                DeSpawn();
            }
            else
            {
                base.Discard(silentlyRemoveReferences);
            }
        }


    }
}
