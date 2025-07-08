using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;




namespace AV_Mechtech
{
    public class Comp_GiveHediffsWhenAlive_Reshaper : ThingComp
    {
        public CompProperties_GiveHediffsWhenAlive_Reshaper Props => (CompProperties_GiveHediffsWhenAlive_Reshaper)props;

        public int TicksForCooldown = 0;
        public bool FirstApply = true;

        public Pawn ParentAsPawn
        {
            get
            {
                Pawn parent_pawn = parent as Pawn;

                if (parent_pawn == null)
                {
                    Log.Error("[AV]Mechtech.Comp_GiveHediffsWhenAlive: parent.ParentAsPawn is null");
                }
                return parent_pawn;
            }
        }

        Comp_WanderingSinistre comp => ParentAsPawn.TryGetComp<Comp_WanderingSinistre>();

        public bool CanBeApplied => !comp.Sinistre.Spawned;


        public override void PostExposeData()
        {
            Scribe_Values.Look(ref TicksForCooldown, "TicksForCooldown");
            Scribe_Values.Look(ref FirstApply, "FirstApply");
        }

        public override void CompTick()
        {
            //Log.Message("[AV]Mechtech: Comptick normal");
        }

        public override void CompTickRare()
        {
            if (ParentAsPawn == null)
            {
                Log.Error("[AV]Mechtech.Comp_GiveHediffsWhenAlive: parent.ParentAsPawn is null");
                return;
            }

            if (!ParentAsPawn.Awake() || ParentAsPawn.health == null || ParentAsPawn.health.InPainShock || !ParentAsPawn.Spawned || ParentAsPawn.Dead)
            {
                return;
            }

            if (Props.UseCooldown)
            {
                if (TicksForCooldown >= Props.CooldownTicks || FirstApply)
                {
                    GiveHediff();
                }
                else if (ParentAsPawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff) != null)   //cooldown only ticks when the pawn has no hediff!
                {
                    TicksForCooldown += 250; // tick rare = 250
                }

            }
            else
            {
                GiveHediff();
            }


        }

        public void GiveHediff()
        {
            if(CanBeApplied)
            {
                Hediff hediff = ParentAsPawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);

                if (hediff == null)
                {
                    ParentAsPawn.health.AddHediff(Props.hediff, ParentAsPawn.health.hediffSet.GetBrain());
                    if (Props.NeedsGraphicUpdate)
                    {
                        ParentAsPawn.Drawer.renderer.SetAllGraphicsDirty();
                    }
                    FirstApply = false;
                    TicksForCooldown = 0;
                }
            }
            
        }


        //NeedsGraphicUpdate
        public override void CompTickLong()
        {
            //doesnt work here
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!(parent is Pawn pawn) || !DebugSettings.godMode)
            {
                yield break;
            }
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }

            Command_Action command_Action2 = new Command_Action();
            command_Action2.defaultLabel = "DEV: Give " + Props.hediff.label;
            if (Props.UseCooldown)
            {
                command_Action2.defaultDesc = "Applied automaticly in " + (Props.CooldownTicks - TicksForCooldown) + "ticks";
            }
            command_Action2.action = delegate
            {
                GiveHediff();
            };

            yield return command_Action2;

            /*
            if (!pawn.IsColonyMech || pawn.GetOverseer() == null)
            {
                yield break;
            }

            */
        }

    }

}

