using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace AV_Mechtech
{
    public class PawnRenderNodeWorker_CarriedOffset : PawnRenderNodeWorker_Carried
    {
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 anchorOffset = Vector3.zero;

            if (parms.facing == Rot4.East)
            {
                anchorOffset += TarantulaUtility.WeaponOffsetEast;
            }
            else if(parms.facing == Rot4.West)
            {
                anchorOffset += TarantulaUtility.WeaponOffsetWest;
            }
            else if(parms.facing == Rot4.North)
            {
                anchorOffset += TarantulaUtility.WeaponOffsetNorth;
            }
            else if (parms.facing == Rot4.South)
            {
                anchorOffset += TarantulaUtility.WeaponOffsetSouth;
            }

            Vector3 result = base.OffsetFor(node, parms, out pivot);
            pivot =  PivotFor(node, parms);
            anchorOffset.y = AltitudeFor(node, parms);
            return anchorOffset;
        }
    }
}
