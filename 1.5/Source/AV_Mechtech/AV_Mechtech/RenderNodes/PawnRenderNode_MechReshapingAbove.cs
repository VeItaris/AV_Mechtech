using AV_Framework;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AV_Mechtech
{
    [StaticConstructorOnStartup]
    public class PawnRenderNode_MechReshapingAbove : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_MechReshapingAbove(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {

        }

        public static Shader shader = MechtechDefOfs.CutoutWithOverlay.Shader;

        public static string Path = "Things/Pawn/Mechanoid/AV_Reshaper/AV_Sinistrepower";

        public static string Path_Overtake = "Things/Pawn/Mechanoid/AV_Reshaper/AV_Overtake";

        public static string Maskpath = "Things/Pawn/Mechanoid/AllegianceOverlays/AV_Sinistre";

        public override Graphic GraphicFor(Pawn pawn)
        {
            PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;

            Graphic graphic = curKindLifeStage.bodyGraphicData.Graphic;


            if (pawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistreOvertake) != null)
            {
                // return GraphicDatabase.Get<Graphic_Multi>(Path_Overtake);

                return GetGraphic(graphic, Path_Overtake, Maskpath);
            }

            if (pawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistrePower) != null)
            {
                return GetGraphic(graphic, Path, Maskpath);

                //return GraphicDatabase.Get<Graphic_Multi>(Path);

            }


            return null;


        }

        public Graphic GetGraphic(Graphic baseGraphic, string texpath, string maskpath = null)
        {

            //return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, Color.white);

            return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, baseGraphic.color, baseGraphic.colorTwo, baseGraphic.data, maskpath);
        }
    }
}
