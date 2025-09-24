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
            Map map = base.Map;

            if(map == null)
            {
                map = base.MapHeld;
                if (map == null)
                {
                    return;
                }
            }

            Comp_SinistreNeeds comp = this.TryGetComp<Comp_SinistreNeeds>();

            if (comp != null && comp.BioferriteNeed >= 0.90f)
            {
                comp.BioferriteNeed -= 1f;

                Thing essence = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                essence.stackCount = 1;

                GenPlace.TryPlaceThing(essence, base.PositionHeld, map, ThingPlaceMode.Near, out var lastResultingThing);
                if (lastResultingThing != null)
                {
                    Messages.Message("AV_SinistreGiftDeath".Translate(), lastResultingThing, MessageTypeDefOf.PositiveEvent);
                    //Find.LetterStack.ReceiveLetter("sinistre boon", "AV_SinistreGift".Translate(), LetterDefOf.PositiveEvent, lastResultingThing);
                }
            }

        }



        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            TryDropEssence();
            if (Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(this);
            }
            else
            {
                base.Destroy(mode);
            }

        }




        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            TryDropEssence();

            if (Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(this);
            }
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
