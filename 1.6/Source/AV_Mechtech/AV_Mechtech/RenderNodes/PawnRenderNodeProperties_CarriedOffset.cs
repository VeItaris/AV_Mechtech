using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class PawnRenderNodeProperties_CarriedOffset : PawnRenderNodeProperties
    {
        public PawnRenderNodeProperties_CarriedOffset()
        {
            useGraphic = false;
            baseLayer = 90f;

            drawData = DrawData.NewWithData(new DrawData.RotationalData(Rot4.North, -10f));
        }
    }
}
