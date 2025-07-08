using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using RimWorld;
using UnityEngine;

namespace AV_Mechtech
{
    public class JobDriver_SinistreEssence : JobDriver
    {

        protected const TargetIndex EssenceIndex = TargetIndex.A;

        protected const TargetIndex MechInd = TargetIndex.B;

        protected const TargetIndex CarrierInd = TargetIndex.C;

        protected Thing Essence => base.TargetThingA;

        protected Pawn Mech => base.TargetPawnB;

        protected Pawn Carrier => base.TargetPawnC;

        CompUsableSinistreEssence comp => Essence.TryGetComp<CompUsableSinistreEssence>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(Essence, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            
            if (!pawn.Reserve(base.TargetB, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            /*
            if (!pawn.Reserve(base.TargetC, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            */

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnForbidden(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnForbidden(TargetIndex.B);
            this.FailOn(() => Mech.IsAttacking());

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);

            Toil toil = Toils_General.WaitWith(TargetIndex.B, 600, useProgressBar: false, maintainPosture: true, maintainSleep: true).FailOnCannotTouch(TargetIndex.B, PathEndMode.ClosestTouch).WithProgressBarToilDelay(TargetIndex.A, 600);
            //yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);

           // Toil toil = Toils_General.Wait(600).FailOnCannotTouch(TargetIndex.A, PathEndMode.ClosestTouch).WithProgressBarToilDelay(TargetIndex.A, 600);
            //toil.activeSkill = () => SkillDefOf.Crafting;
            toil.handlingFacing = true;
            toil.WithEffect(EffecterDefOf.MechRepairing, TargetIndex.A);
            toil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
            toil.tickAction = delegate
            {
                pawn.rotationTracker.FaceTarget(job.GetTarget(TargetIndex.B));
            };
            yield return toil;
            yield return Toils_General.DoAtomic(delegate
            {
                comp.OnUsed(Mech);
            });
        }

    }


}
