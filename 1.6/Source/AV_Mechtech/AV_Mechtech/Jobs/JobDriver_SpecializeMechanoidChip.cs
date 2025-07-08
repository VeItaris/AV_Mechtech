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
    public class JobDriver_SpecializeMechanoidChip : JobDriver
    {

        protected const TargetIndex ThingToStudyIndex = TargetIndex.A;

        protected const TargetIndex ResearchBenchInd = TargetIndex.B;

        protected const TargetIndex HaulCell = TargetIndex.C;

        protected Thing ThingToStudy => base.TargetThingA;

        protected CompUsableUnspecializedMechchip comp => ThingToStudy.TryGetComp<CompUsableUnspecializedMechchip>();

        // protected bool CanStudyInPlace => ThingToStudy.TryGetComp<CompUsableUnspecializedMechchip>().DestroyedFromJob;


        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(ThingToStudy, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            /*
            if (!pawn.Reserve(base.TargetB, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            if (!pawn.Reserve(base.TargetC, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            */

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            
            Toil toil = Toils_General.Wait(600).FailOnCannotTouch(TargetIndex.A, PathEndMode.ClosestTouch).WithProgressBarToilDelay(TargetIndex.A, 600);
            //toil.activeSkill = () => SkillDefOf.Crafting;
            toil.handlingFacing = true;
            toil.tickAction = delegate
            {
                pawn.rotationTracker.FaceTarget(job.GetTarget(TargetIndex.A));
            };
            yield return toil;
            yield return Toils_General.DoAtomic(delegate
            {
                comp.OnAnalyzed();
            });
        }

    }


}
