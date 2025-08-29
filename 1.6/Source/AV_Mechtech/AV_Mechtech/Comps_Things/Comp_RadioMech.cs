using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Verse;
using Verse.AI;
using Verse.Noise;
using Verse.Sound;

namespace AV_Mechtech
{
    public class Comp_RadioMech : ThingComp
    {
        public CompProperties_RadioMech Props => (CompProperties_RadioMech)props;

        private Mote mote;

        private int moteTimer = 60;

        private int maxTicks = 60;

        private Pawn RadioTransmitter()
        {
            Pawn mech = (Pawn)parent;
            return mech;
        }

        public override void CompTick()
        {
            if (Props.mote != null && (mote == null || mote.Destroyed) && moteTimer == 0)
            {
                mote = MoteMaker.MakeAttachedOverlay(parent, Props.mote, Vector3.zero);
            }
            if (mote != null && moteTimer < maxTicks)
            {
                moteTimer++; 
                mote.Maintain();
            }
        }


        private int SpaceCounter = 0;

        public override void CompTickRare()
        {
            base.CompTickRare();

            SpaceCounter += 250;

            if(SpaceCounter >= 30000)   // twice a day
            {
                if (parent.Map != null && parent.Map.Tile.LayerDef.isSpace)
                {
                    Vector3 loc = parent.DrawPos;
                    
                    loc.z += 0.75f;
                    MoteMaker.ThrowText(loc, parent.Map, "AV_IMINSPACE".Translate(), UnityEngine.Color.white, 3.0f);
                }
                SpaceCounter = 0;
            }

            
        }



        public void ChangeSeverity(float serverity)
        {
            Hediff hediff = RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff);
            if(hediff == null)
            {
                RadioTransmitter().health.AddHediff(Props.radiohediff, RadioTransmitter().health.hediffSet.GetBrain());
                hediff = RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff);
            }
            hediff.Severity = serverity;

            moteTimer = 0; //resets timer so motes starts

            SoundDef sound = MechtechDefOfs.CombatCommand_Warmup; // ListOfIdeology_Defs.CombatCommand_Warmup();
            sound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
        }


        public void ChooseModule()
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>()
            {
                new FloatMenuOption("AV_LabelGizmoGeneral".Translate(), delegate
                {
                    ChangeSeverity(0.1f);

                }, MenuOptionPriority.High),
                new FloatMenuOption("AV_LabelGizmoTeacher".Translate(), delegate
                {
                    ChangeSeverity(1.0f);
                }, MenuOptionPriority.High),
                new FloatMenuOption("AV_LabelGizmoHacker".Translate(), delegate
                {
                    ChangeSeverity(2.0f);
                }, MenuOptionPriority.High),
                new FloatMenuOption("AV_LabelGizmoMechanitor".Translate(), delegate
                {
                    ChangeSeverity(3.0f);
                }, MenuOptionPriority.High),
                new FloatMenuOption("AV_LabelGizmoMedic".Translate(), delegate
                {
                    ChangeSeverity(4.0f);
                }, MenuOptionPriority.High),
            };
            Find.WindowStack.Add(new FloatMenu(options));           
        }

        private string GetLabel()
        {
            if(RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff) == null)
            {
                ChangeSeverity(0.1f);
            }

            float hediffseverity = RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff).Severity;
            
            if(hediffseverity <= 0.99f)
            {
                return "AV_LabelGizmoModule".Translate() + ": " + "AV_LabelGizmoGeneral".Translate();
            }
            if (hediffseverity <= 1.99f)
            {
                return "AV_LabelGizmoModule".Translate() + ": " + "AV_LabelGizmoTeacher".Translate();
            }
            if (hediffseverity <= 2.99f)
            {
                return "AV_LabelGizmoModule".Translate() + ": " + "AV_LabelGizmoHacker".Translate();
            }
            if (hediffseverity <= 3.99f)
            {
                return "AV_LabelGizmoModule".Translate() + ": " + "AV_LabelGizmoMechanitor".Translate();
            }
            if (hediffseverity <= 4.99f)
            {
                return "AV_LabelGizmoModule".Translate() + ": " + "AV_LabelGizmoMedic".Translate();
            }
            Log.Warning("MechtechMod.Comp_RadioMech: Hediff severity is to high");
            return "Hediff severity is to high";
        }

        private string GetDescription()
        {
            if (RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff) == null)
            {
                ChangeSeverity(0.1f);
            }

            float hediffseverity = RadioTransmitter().health.hediffSet.GetFirstHediffOfDef(Props.radiohediff).Severity;

            if (hediffseverity <= 0.99f)
            {
                return "AV_DescGizmoGeneral".Translate() + "\n" + "AV_DescGizmoChangeModule".Translate().CapitalizeFirst();
            }
            if (hediffseverity <= 1.99f)
            {
                return "AV_DescGizmoTeacher".Translate() + "\n" + "AV_DescGizmoChangeModule".Translate().CapitalizeFirst();
            }
            if (hediffseverity <= 2.99f)
            {
                return "AV_DescGizmoHacker".Translate() + "\n" + "AV_DescGizmoChangeModule".Translate().CapitalizeFirst();
            }
            if (hediffseverity <= 3.99f)
            {
                return "AV_DescGizmoMechanitor".Translate() + "\n" + "AV_DescGizmoChangeModule".Translate().CapitalizeFirst();
            }
            if (hediffseverity <= 4.99f)
            {
                return "AV_DescGizmoMedic".Translate() + "\n" + "AV_DescGizmoChangeModule".Translate().CapitalizeFirst();
            }
            Log.Warning("MechtechMod.Comp_RadioMech: Hediff severity is to high");
            return "EROOR: Hediff severity is to high";
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Faction == Faction.OfPlayer || MechtechSettings.DebugLogging)
            {
                Command_Action choosemodule_Action = new Command_Action();
                choosemodule_Action.defaultLabel = GetLabel();
                choosemodule_Action.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_waves_command");
                choosemodule_Action.defaultDesc = GetDescription();
                choosemodule_Action.action = delegate
                {
                    ChooseModule();
                };
                yield return choosemodule_Action;
            }
        }
    }
}
