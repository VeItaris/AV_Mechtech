using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace AV_Mechtech
{
    public class Verb_CastTargetEffectLances_Recoder : Verb_CastTargetEffect
    {

        public override void OnGUI(LocalTargetInfo target)
        {
            if (IsBiocodedForCaster(target))
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_Biocoded".Translate(), 0f, -20f);
            }
            else if (IsBiocoded(target))
            {
                Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_CanRecode".Translate(), 0f, -20f);
            }
            else if(!IsBiocodeable(target) || !IsBiocoded(target))
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_NotRecodeable".Translate(), 0f, -20f);
            }
            else
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
            }

        }

        public bool IsApparel(LocalTargetInfo target)
        {
            if (target.Thing is Apparel)
            {
                return true;
            }
            return false;
        }

        public bool IsPrimary(LocalTargetInfo target)
        {
            if (target.Thing?.def?.equipmentType == EquipmentType.Primary)
            {
                return true;
            }
            return false;
        }

        public bool IsBiocoded(LocalTargetInfo target)
        {
            if (IsApparel(target) || IsPrimary(target))
            {
                CompBiocodable compBiocodable = target.Thing.TryGetComp<CompBiocodable>();
                if(compBiocodable != null)
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

        public bool IsBiocodeable(LocalTargetInfo target)
        {
            if (IsApparel(target) || IsPrimary(target))
            {
                CompBiocodable compBiocodable = target.Thing.TryGetComp<CompBiocodable>();
                if (compBiocodable != null)
                {
                    return true;
                }
            }
            return false;
        }


        
        public bool IsBiocodedForCaster(LocalTargetInfo target)
        {
            if (IsApparel(target) || IsPrimary(target))
            {
                CompBiocodable compBiocodable = target.Thing.TryGetComp<CompBiocodable>();
                if (compBiocodable != null)
                {
                    Pawn wielder = compBiocodable.CodedPawn;

                    if (wielder == CasterPawn)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
                    



        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!IsApparel(target) && !IsPrimary(target))
            {
                return false;
            }
            if (IsBiocodedForCaster(target))
            {
                return false;
            }
            if (IsBiocodeable(target) && !IsBiocoded(target))
            {
                return false;
            }
            if (!IsBiocodeable(target))
            {
                return false;
            }
            if (!IsBiocoded(target))
            {
                return false;
            }

            return base.ValidateTarget(target, showMessages);
        }
    }


}

