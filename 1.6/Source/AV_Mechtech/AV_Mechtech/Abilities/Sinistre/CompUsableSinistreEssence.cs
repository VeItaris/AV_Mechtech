using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;
using static RimWorld.MechClusterSketch;

namespace AV_Mechtech
{
    public class CompUsableSinistreEssence : ThingComp 
    {
        public CompProperties_Usable_SinistreEssence Props => (CompProperties_Usable_SinistreEssence)props;

        protected virtual string FloatMenuOptionLabel(Pawn pawn, ThingDef def)
        {
            if(def == null)
            {
                return Props.useLabel;
            }
            return Props.useLabel + def.label;
        }

        public Pawn User;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            User = myPawn;
            FloatMenuOption floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(FloatMenuOptionLabel(myPawn, null), delegate
            {
                Find.Targeter.BeginTargeting(TargetingParameters(), TryStartUseJob, Highlight, CanTarget);

            }), myPawn, parent);

            AcceptanceReport acceptanceReport = CanBeUsedBy(myPawn, Props.ignoreOtherReservations);

            if (!acceptanceReport.Accepted)
            {
                floatMenuOption.Disabled = true;
                floatMenuOption.Label = floatMenuOption.Label +" (" + acceptanceReport.Reason + ")";
            }
            yield return floatMenuOption;

        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return base.CompGetGizmosExtra();
        }

        public virtual AcceptanceReport CanBeUsedBy(Pawn p, bool forced = false, bool ignoreReserveAndReachable = false)
        {
            if (!p.RaceProps.Humanlike)
            {
                //Log.Message("CompUsableSinistreEssence -> CanBeUsedBy -> 0");
                return false;
            }
            if (p.IsMutant && !Props.allowedMutants.Contains(p.mutant.Def))
            {
               // Log.Message("CompUsableSinistreEssence -> CanBeUsedBy -> 1");
                return false;
            }

            if (!ignoreReserveAndReachable && !p.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
            {
                //Log.Message("CompUsableSinistreEssence -> CanBeUsedBy -> 2");
                return "NoPath".Translate();
            }
            if (!ignoreReserveAndReachable && !p.CanReserve(parent, 1, -1, null, forced))
            {
                //Log.Message("CompUsableSinistreEssence -> CanBeUsedBy -> 3");
                Pawn pawn = p.Map.reservationManager.FirstRespectedReserver(parent, p) ?? p.Map.physicalInteractionReservationManager.FirstReserverOf(parent);
                if (pawn != null)
                {
                    //Log.Message("CompUsableSinistreEssence -> CanBeUsedBy -> 4");
                    return "ReservedBy".Translate(pawn.LabelShort, pawn);
                }
                return "Reserved".Translate();
            }

            

            return true;
        }


        #region TakeControlRemote 

        private void Highlight(LocalTargetInfo target)          //simplified CompPlantable targeter
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);        //this is the circle over the targeted pawn!
            }
        }

        public TargetingParameters TargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                canTargetHumans = false,
                canTargetMechs = true,
                canTargetAnimals = false,
                canTargetLocations = false,
                validator = (TargetInfo x) => CanTarget((LocalTargetInfo)x)
            };
        }

        public bool CanTarget(LocalTargetInfo target)
        {
            if (target.Pawn == null || !target.Pawn.IsColonyMech || target.Pawn.Dead)
            {
                return false;
            }

            if (target.Pawn.def == MechtechDefOfs.AV_Mech_Reshaper)
            {
                if(target.Pawn.TryGetComp<Comp_WanderingSinistre>()?.powerStage >= SinistreUtility.MaxPowerStage)
                {
                    return false; //"Already at maximum power";
                }
                else
                {
                    return true;
                }
            }

            if (target.Pawn.health.hediffSet.HasHediff(MechtechDefOfs.AV_SinistreMechDeathRefusal) )
            {
                return false; //"Already applied";
            }

            return true;
        }


        public virtual void TryStartUseJob(LocalTargetInfo target)
        {
            //Log.Message("CompUsableSinistreEssence -> TryStartUseJob -> 1");
            if (Props.useJob == null || !CanBeUsedBy(User))
            {
                //Log.Message("CompUsableSinistreEssence -> TryStartUseJob -> stoped");
                return;
            }

            if (target.Pawn != null)
            {
                if (target.Pawn.def == MechtechDefOfs.AV_Mech_Reshaper)
                {
                    Log.Message("Targeting reshaper");
                }
            }


            //Log.Message("CompUsableSinistreEssence -> TryStartUseJob -> 2");
            StartJob();

            void StartJob()
            {
                Job job = JobMaker.MakeJob(Props.useJob, parent, target, User);
                job.count = 1;
                User.jobs.TryTakeOrderedJob(job, JobTag.Misc);
               // Log.Message("CompUsableSinistreEssence -> TryStartUseJob -> 3");
            }

        }

        #endregion

        public void OnUsed(Pawn mech)
        {
            if (mech.def == MechtechDefOfs.AV_Mech_Reshaper)
            {
                Comp_WanderingSinistre comp = mech.TryGetComp<Comp_WanderingSinistre>();
                if (comp != null)
                {
                    comp.TryIncreaseSinistrePower();
                }
            }
            else
            {
                mech.health.AddHediff(MechtechDefOfs.AV_SinistreMechDeathRefusal);
            }
            SoundDef sound = SoundDefOf.ControlMech_Complete;
            sound.PlayOneShot(new TargetInfo(mech.Position, mech.Map));

            if(parent.stackCount > 1)
            {
                parent.stackCount--;
            }
            else
            {
                parent.Destroy();
            }
            
        }



        public void SendCannotUseMessage(Pawn pawn, string reason)
        {
            Messages.Message(string.Format("{0}: {1}", "CannotGenericWorkCustom".Translate(FloatMenuOptionLabel(pawn, null)).ToLower(), reason.CapitalizeFirst()), pawn, MessageTypeDefOf.RejectInput, historical: false);
        }

        public void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
            }
        }
        
       
        




    }
}
