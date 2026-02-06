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
    public class PawnRenderNodeTarantula : PawnRenderNode_AnimalPart
    {
        public PawnRenderNodeTarantula(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {

        }

        public static Shader shader = MechtechDefOfs.CutoutWithOverlay.Shader;

        public static string Maskpath = "Things/Pawn/Mechanoid/AllegianceOverlays/AV_Tarantula";
        public static string Maskpath_Destroyed = "Things/Pawn/Mechanoid/AllegianceOverlays/AV_Tarantula_Destroyed";

        public static string Path_Empty = "Things/Pawn/Mechanoid/AV_Tarantula/Empty/AV_Tarantula";

        public static string Path_One = "Things/Pawn/Mechanoid/AV_Tarantula/One/AV_Tarantula";
        public static string Path_Two = "Things/Pawn/Mechanoid/AV_Tarantula/Two/AV_Tarantula";
        public static string Path_Three = "Things/Pawn/Mechanoid/AV_Tarantula/Three/AV_Tarantula";
        public static string Path_Four = "Things/Pawn/Mechanoid/AV_Tarantula/Four/AV_Tarantula";
        public static string Path_Five = "Things/Pawn/Mechanoid/AV_Tarantula/Five/AV_Tarantula";
        public static string Path_Six = "Things/Pawn/Mechanoid/AV_Tarantula/Six/AV_Tarantula";
        public static string Path_Seven = "Things/Pawn/Mechanoid/AV_Tarantula/Seven/AV_Tarantula";
        public static string Path_Eight = "Things/Pawn/Mechanoid/AV_Tarantula/Eight/AV_Tarantula";
        public static string Path_Nine = "Things/Pawn/Mechanoid/AV_Tarantula/Nine/AV_Tarantula";
        public static string Path_Ten = "Things/Pawn/Mechanoid/AV_Tarantula/Ten/AV_Tarantula";

        public static string Path_Eleven = "Things/Pawn/Mechanoid/AV_Tarantula/Eleven/AV_Tarantula";
        public static string Path_Twelve = "Things/Pawn/Mechanoid/AV_Tarantula/Twelve/AV_Tarantula";
        public static string Path_Thirteen = "Things/Pawn/Mechanoid/AV_Tarantula/Thirteen/AV_Tarantula";
        public static string Path_Fourteen = "Things/Pawn/Mechanoid/AV_Tarantula/Fourteen/AV_Tarantula";
        public static string Path_Fifteen = "Things/Pawn/Mechanoid/AV_Tarantula/Fifteen/AV_Tarantula";

        public static string Path_Full = "Things/Pawn/Mechanoid/AV_Tarantula/Full/AV_Tarantula";

        public static string Path_Destroyed = "Things/Pawn/Mechanoid/AV_Tarantula/Destroyed/AV_Tarantula";



        public static string PathAncient_Empty = "Things/Pawn/Mechanoid/AV_Tarantula/Empty/AV_TarantulaAncient";

        public static string PathAncient_One = "Things/Pawn/Mechanoid/AV_Tarantula/One/AV_TarantulaAncient";
        public static string PathAncient_Two = "Things/Pawn/Mechanoid/AV_Tarantula/Two/AV_TarantulaAncient";
        public static string PathAncient_Three = "Things/Pawn/Mechanoid/AV_Tarantula/Three/AV_TarantulaAncient";
        public static string PathAncient_Four = "Things/Pawn/Mechanoid/AV_Tarantula/Four/AV_TarantulaAncient";
        public static string PathAncient_Five = "Things/Pawn/Mechanoid/AV_Tarantula/Five/AV_TarantulaAncient";
        public static string PathAncient_Six = "Things/Pawn/Mechanoid/AV_Tarantula/Six/AV_TarantulaAncient";
        public static string PathAncient_Seven = "Things/Pawn/Mechanoid/AV_Tarantula/Seven/AV_TarantulaAncient";
        public static string PathAncient_Eight = "Things/Pawn/Mechanoid/AV_Tarantula/Eight/AV_TarantulaAncient";
        public static string PathAncient_Nine = "Things/Pawn/Mechanoid/AV_Tarantula/Nine/AV_TarantulaAncient";
        public static string PathAncient_Ten = "Things/Pawn/Mechanoid/AV_Tarantula/Ten/AV_TarantulaAncient";

        public static string PathAncient_Eleven = "Things/Pawn/Mechanoid/AV_Tarantula/Eleven/AV_TarantulaAncient";
        public static string PathAncient_Twelve = "Things/Pawn/Mechanoid/AV_Tarantula/Twelve/AV_TarantulaAncient";
        public static string PathAncient_Thirteen = "Things/Pawn/Mechanoid/AV_Tarantula/Thirteen/AV_TarantulaAncient";
        public static string PathAncient_Fourteen = "Things/Pawn/Mechanoid/AV_Tarantula/Fourteen/AV_TarantulaAncient";
        public static string PathAncient_Fifteen = "Things/Pawn/Mechanoid/AV_Tarantula/Fifteen/AV_TarantulaAncient";

        public static string PathAncient_Full = "Things/Pawn/Mechanoid/AV_Tarantula/Full/AV_TarantulaAncient";

        public static string PathAncient_Destroyed = "Things/Pawn/Mechanoid/AV_Tarantula/Destroyed/AV_TarantulaAncient";


        public override Graphic GraphicFor(Pawn pawn)
        {
            if (!pawn.TryGetComp<CompMechReloadableResourceHolder>(out var ResourceHolder))
            {
                return null;
            }

            PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;

            Graphic graphic = curKindLifeStage.bodyGraphicData.Graphic;

            if (TarantulaUtility.IsSiloDestroyed(pawn, ResourceHolder))
            {

                if (curKindLifeStage == pawn.kindDef.lifeStages.First())
                {
                    return GetGraphic(pawn, graphic, Path_Destroyed, Maskpath_Destroyed);
                }
                else
                {
                    return GetGraphic(pawn, graphic, PathAncient_Destroyed, Maskpath_Destroyed);
                }
            }
            else
            {
                int useableMissiles = ResourceHolder.IngredientCount / TarantulaUtility.ResourceCostPerShot;

                if (useableMissiles > 16)
                { useableMissiles = 16; }
                if (useableMissiles < 0)
                { useableMissiles = 0; }

                if(curKindLifeStage == pawn.kindDef.lifeStages.First())
                {
                    switch (useableMissiles)
                    {
                        case 0:
                            return GetGraphic(pawn, graphic, Path_Empty);
                        case 1:
                            return GetGraphic(pawn, graphic, Path_Fifteen);
                        case 2:
                            return GetGraphic(pawn, graphic, Path_Fourteen);
                        case 3:
                            return GetGraphic(pawn, graphic, Path_Thirteen);
                        case 4:
                            return GetGraphic(pawn, graphic, Path_Twelve);
                        case 5:
                            return GetGraphic(pawn, graphic, Path_Eleven);
                        case 6:
                            return GetGraphic(pawn, graphic, Path_Ten);
                        case 7:
                            return GetGraphic(pawn, graphic, Path_Nine);
                        case 8:
                            return GetGraphic(pawn, graphic, Path_Eight);
                        case 9:
                            return GetGraphic(pawn, graphic, Path_Seven);
                        case 10:
                            return GetGraphic(pawn, graphic, Path_Six);
                        case 11:
                            return GetGraphic(pawn, graphic, Path_Five);
                        case 12:
                            return GetGraphic(pawn, graphic, Path_Four);
                        case 13:
                            return GetGraphic(pawn, graphic, Path_Three);
                        case 14:
                            return GetGraphic(pawn, graphic, Path_Two);
                        case 15:
                            return GetGraphic(pawn, graphic, Path_One);
                        case 16:
                            return GetGraphic(pawn, graphic, Path_Full);
                            break;

                    }

                }
                else
                {
                    switch (useableMissiles)
                    {
                        case 0:
                            return GetGraphic(pawn, graphic, PathAncient_Empty);
                        case 1:
                            return GetGraphic(pawn, graphic, PathAncient_Fifteen);
                        case 2:
                            return GetGraphic(pawn, graphic, PathAncient_Fourteen);
                        case 3:
                            return GetGraphic(pawn, graphic, PathAncient_Thirteen);
                        case 4:
                            return GetGraphic(pawn, graphic, PathAncient_Twelve);
                        case 5:
                            return GetGraphic(pawn, graphic, PathAncient_Eleven);
                        case 6:
                            return GetGraphic(pawn, graphic, PathAncient_Ten);
                        case 7:
                            return GetGraphic(pawn, graphic, PathAncient_Nine);
                        case 8:
                            return GetGraphic(pawn, graphic, PathAncient_Eight);
                        case 9:
                            return GetGraphic(pawn, graphic, PathAncient_Seven);
                        case 10:
                            return GetGraphic(pawn, graphic,  PathAncient_Six);
                        case 11:
                            return GetGraphic(pawn, graphic, PathAncient_Five);
                        case 12:
                            return GetGraphic(pawn, graphic,  PathAncient_Four);
                        case 13:
                            return GetGraphic(pawn, graphic, PathAncient_Three);
                        case 14:
                            return GetGraphic(pawn, graphic, PathAncient_Two);
                        case 15:
                            return GetGraphic(pawn, graphic, PathAncient_One);
                        case 16:
                            return GetGraphic(pawn, graphic, PathAncient_Full);
                            break;
                    }
                }
            }

            return null;
        }
    
    
        public Graphic GetGraphic(Pawn pawn, Graphic baseGraphic, string texpath,  string maskpath = null)
        {
            if(maskpath == null)
            {
                maskpath = Maskpath;
                return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, baseGraphic.color, baseGraphic.colorTwo, baseGraphic.data, maskpath);
            }
            //return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, baseGraphic.color, baseGraphic.colorTwo, baseGraphic.data, maskpath);



            return GraphicDatabase.Get<Graphic_Multi>(texpath, shader, baseGraphic.drawSize, baseGraphic.color, baseGraphic.colorTwo, baseGraphic.data);
        }
    }
}
