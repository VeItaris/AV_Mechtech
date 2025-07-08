using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AV_Mechtech
{
    public class Verb_CastTargetEffectLances_MechanoidStateRemover : Verb_CastTargetEffect
    {

        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && verbProps.targetParams.CanTarget(target.ToTargetInfo(caster.Map)))
            {
                Pawn pawn = target.Pawn;
                if (pawn != null)
                {
                    if(pawn.def.HasComp<CompMechPowerCell>())
                    {
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_Overload".Translate(), 0f, -20f);
                    }
                    else if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0f)
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                        Widgets.MouseAttachedLabel("CannotShootPawnIsPsychicallyDeaf".Translate(pawn), 0f, -20f);
                    }
                    else if (pawn.mindState.mentalStateHandler.InMentalState)
                    {
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_FixMentalState".Translate(), 0f, -20f);
                    }
                    else if (MechtechDefOfs.AV_MechMightConnectToHive != null && pawn.health.hediffSet.HasHediff(MechtechDefOfs.AV_MechMightConnectToHive))
                    {
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_OldConnectionData".Translate(), 0f, -20f);
                    }
                    else if (MechtechDefOfs.AV_Databreach != null && pawn.health.hediffSet.HasHediff(MechtechDefOfs.AV_Databreach))
                    {
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_DataBreach".Translate(), 0f, -20f);
                    }
                    else
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                    }
                }
                else
                {
                    base.OnGUI(target);
                }
            }
            else
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
            }
        }


        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Pawn != null)
            {
                if (target.Pawn.kindDef.isBoss)
                {
                    return false;
                }

                if (!target.Pawn.RaceProps.IsMechanoid)
                {
                    return false;
                }

                if (   target.Pawn.def.HasComp<CompMechPowerCell>()
                    || target.Pawn.mindState?.mentalStateHandler?.InMentalState == true
                    || MechtechDefOfs.AV_MechMightConnectToHive != null && target.Pawn.health.hediffSet.HasHediff(MechtechDefOfs.AV_MechMightConnectToHive)
                    || MechtechDefOfs.AV_Databreach != null && target.Pawn.health.hediffSet.HasHediff(MechtechDefOfs.AV_Databreach)
                    )
                {
                    return base.ValidateTarget(target, showMessages);
                }
            }

            return false;
            
        }
    }


}

