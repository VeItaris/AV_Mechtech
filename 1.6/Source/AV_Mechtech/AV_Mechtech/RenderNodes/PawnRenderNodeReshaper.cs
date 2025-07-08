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
    public class PawnRenderNodeReshaper : PawnRenderNode_AnimalPart
    {
        public PawnRenderNodeReshaper(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {

        }

        public static Shader shader = MechtechDefOfs.CutoutWithOverlay.Shader;

        public static string Maskpath = "Things/Pawn/Mechanoid/AllegianceOverlays/AV_Reshaper";

        public static string Path = "Things/Pawn/Mechanoid/AV_Reshaper/AV_Reshaper";

        public static string Path_Bioferrite = "Things/Pawn/Mechanoid/AV_Reshaper/AV_Reshaper_Bioferrite";

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (!pawn.TryGetComp<CompMechReloadableResourceHolder>(out var ResourceHolder))
            {
                return null;
            }

            PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;

            Graphic graphic = curKindLifeStage.bodyGraphicData.Graphic;

            if(ResourceHolder.IngredientCount >= 20)
            {
                return GetGraphic(graphic, Path_Bioferrite, Maskpath);
            }

            return GetGraphic(graphic, Path, Maskpath);
        }
    
    
        public Graphic GetGraphic(Graphic baseGraphic, string texpath, string maskpath = null)
        {
            if(maskpath == null)
            {
                maskpath = Maskpath;
            }
            return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, baseGraphic.color, baseGraphic.colorTwo, baseGraphic.data, maskpath);
        }
    }
}
