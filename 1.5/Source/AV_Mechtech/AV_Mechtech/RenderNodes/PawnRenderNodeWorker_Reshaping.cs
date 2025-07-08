using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class PawnRenderNodeWorker_Reshaping : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            if (!base.CanDrawNow(node, parms))
            {
                return false;
            }
            if (parms.Portrait || parms.pawn.Dead || !parms.pawn.Spawned)
            {
                return false;
            }
            if(parms.pawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistreMechDeathRefusal) != null)
            {
                return true;
            }
            return false;
        }
    }
}
