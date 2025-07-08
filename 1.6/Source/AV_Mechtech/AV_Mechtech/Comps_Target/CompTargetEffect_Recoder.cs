using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using static RimWorld.MechClusterSketch;
using static RimWorld.RitualStage_InteractWithRole;
using static UnityEngine.GraphicsBuffer;

namespace AV_Mechtech
{
    public class CompTargetEffect_Recoder : CompTargetEffect
    {

        public override void DoEffectOn(Pawn user, Thing target)
        {
            if(target is Apparel || target.def?.equipmentType == EquipmentType.Primary)
            {
                CompBiocodable compBiocodable = target.TryGetComp<CompBiocodable>();
                if (compBiocodable != null)
                {
                    Pawn wielder = compBiocodable.CodedPawn;

                    if (wielder != null)
                    {
                        compBiocodable.CodeFor(user);

                        Messages.Message("AV_Message_RetuningSuccessful".Translate(user.Named("PAWN"), target.Named("ITEM")).AdjustedFor(user), user, MessageTypeDefOf.NeutralEvent);
                    }
                }
                
            }

        }

        public override bool CanApplyOn(Thing target)
        {
            if (target is Apparel || target.def?.equipmentType == EquipmentType.Primary)
            {
                CompBiocodable compBiocodable = target.TryGetComp<CompBiocodable>();
                if (compBiocodable != null)
                {
                    Pawn wielder = compBiocodable.CodedPawn;

                    if (wielder != null)
                    {
                        return true;
                    }
                }
            }
            return false;       
        }
        
    }
}
