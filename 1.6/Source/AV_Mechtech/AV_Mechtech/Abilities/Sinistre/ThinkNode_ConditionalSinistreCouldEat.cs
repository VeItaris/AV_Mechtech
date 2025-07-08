using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace AV_Mechtech
{
    internal class ThinkNode_ConditionalSinistreCouldEat : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            Comp_SinistreNeeds comp = pawn.GetComp<Comp_SinistreNeeds>();
            if (comp == null)
            {
                return false;
            }
            return comp.MightEat;
        }
    }
}