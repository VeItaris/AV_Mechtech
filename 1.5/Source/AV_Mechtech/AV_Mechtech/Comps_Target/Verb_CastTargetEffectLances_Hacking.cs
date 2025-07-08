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
    public class Verb_CastTargetEffectLances_Hacking : Verb_CastTargetEffect
    {

        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && verbProps.targetParams.CanTarget(target.ToTargetInfo(caster.Map)))
            {
                Pawn pawn = target.Pawn;
                if (pawn != null)
                {
                    bool flag = target.Pawn.kindDef.isBoss;
                    foreach (CompTargetEffect comp in base.EquipmentSource.GetComps<CompTargetEffect>())
                    {
                        if (!comp.CanApplyOn(pawn))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (pawn.Faction.IsPlayer)
                        {
                            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                            Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_Overseen".Translate(), 0f, -20f);
                        }
                        else
                        {
                            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                            if (!string.IsNullOrEmpty(verbProps.invalidTargetPawn))
                            {
                                Widgets.MouseAttachedLabel(verbProps.invalidTargetPawn.CapitalizeFirst(), 0f, -20f);
                            }
                        }  
                    }
                    else if (pawn.Faction.IsPlayer)
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_Overseen".Translate(), 0f, -20f);
                    }
                    else if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0f)
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                        Widgets.MouseAttachedLabel("CannotShootPawnIsPsychicallyDeaf".Translate(pawn), 0f, -20f);
                    }
                    else if (!HasEnoughChargesFor(target))
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                        Widgets.MouseAttachedLabel("AV_MouseAttachedLabel_NoCharges".Translate(), 0f, -20f);
                    }
                    else if(CanHitTarget(target) && verbProps.targetParams.CanTarget(target.ToTargetInfo(caster.Map)))
                    {
                        float chargeCost = 1;
                        string chargeString;
                        if (target.Pawn.GetStatValue(StatDefOf.BandwidthCost) >= 1)
                        {
                            chargeCost = target.Pawn.GetStatValue(StatDefOf.BandwidthCost);

                            chargeString = "AV_MouseAttachedLabel_ShowChargecost".Translate(chargeCost.Named("CHARGECOST"));
                        }
                        else
                        {
                            chargeString = "AV_MouseAttachedLabel_ShowChargecost".Translate(chargeCost.Named("CHARGECOST"));
                        }

                        Widgets.MouseAttachedLabel(chargeString, 0f, -20f);
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

        private bool HasEnoughChargesFor(LocalTargetInfo target)
        {
            CompApparelReloadable comp = base.EquipmentSource.TryGetComp<CompApparelReloadable>();
            float bandwidth_cost = target.Pawn.GetStatValue(StatDefOf.BandwidthCost);
            if (comp != null)
            {
                if (comp.RemainingCharges >= bandwidth_cost)
                {
                    return true;
                }
            }

            return false;
        }


        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Pawn != null)
            {
                if (target.Pawn.kindDef.isBoss)
                {
                    return false;
                }
                if (target.Pawn.Faction.IsPlayer)
                {
                    return false;
                }
                if (target.Pawn.RaceProps.IsMechanoid && !HasEnoughChargesFor(target))
                {
                    return false;
                }
                if (target.Pawn.GetStatValue(StatDefOf.BandwidthCost) > MechtechSettings.HackingLanceBandwidth)
                {
                    return false;
                }
            }
            return base.ValidateTarget(target, showMessages);
        }
    }


}

