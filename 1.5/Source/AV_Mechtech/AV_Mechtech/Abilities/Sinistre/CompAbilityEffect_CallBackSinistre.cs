using AV_Framework;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class CompAbilityEffect_CallBackSinistre : CompAbilityEffect
    {
        public new CompProperties_AbilityCallBackSinistre Props => (CompProperties_AbilityCallBackSinistre)props;

        Comp_WanderingSinistre wanderingSinistre => parent.pawn.TryGetComp<Comp_WanderingSinistre>();


        int PowerStageNeeded = SinistreUtility.PowerStageNeeded_CallSinistre;

        public override bool ShouldHideGizmo => HideGizmo();

        private bool HideGizmo()
        {
            if (wanderingSinistre == null || wanderingSinistre.SinistreAttached || !wanderingSinistre.Sinistre.Spawned || wanderingSinistre.Sinistre.Dead)
            {
                return true;
            }

            if(wanderingSinistre.powerStage < PowerStageNeeded)
            {
                return true;
            }

            return false;
        }


        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            wanderingSinistre.Sinistre.Kill(null);

            parent.pawn.health.AddHediff(MechtechDefOfs.AV_SinistreMechDeathRefusal);
            Hediff powerhediff = parent.pawn.health.AddHediff(MechtechDefOfs.AV_SinistrePower);
            powerhediff.Severity = wanderingSinistre.powerStage;
        }


    }
}
