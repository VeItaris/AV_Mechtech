using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Verse;
using Verse.AI;
using Verse.Noise;


namespace AV_Mechtech
{
    [Obsolete]
    public class HediffComp_MechlinkUnlimiter : HediffComp
    {
        public HediffCompProperties_MechlinkUnlimiter Props => (HediffCompProperties_MechlinkUnlimiter)props;


        private Mote mote;

        private int moteTimer = 240;
        private int maxTicks  = 240;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Props.mote != null && (mote == null || mote.Destroyed) && moteTimer == 0)
            {
                mote = MoteMaker.MakeAttachedOverlay(parent.pawn, Props.mote, Vector3.zero);
            }
            if (mote != null && moteTimer < maxTicks)
            {
                moteTimer++; 
                mote.Maintain();
            }
        }

        public void ChangeSeverity(float serverity)     
        {
            Hediff hediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
            if(hediff == null)
            {
                parent.pawn.health.AddHediff(Props.hediff, parent.pawn.health.hediffSet.GetBrain());    // Thats never the case in a hediff comp...
            }
            hediff.Severity = serverity;

            moteTimer = 0; //resets timer so motes starts
        }


        public void ChooseModule()
        {
            Hediff hediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);

            List<FloatMenuOption> options = new List<FloatMenuOption>();


            if (hediff.Severity <= 0f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedZero".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(0.1f);

                }));
            }
            if (hediff.Severity <= 0.9f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedOne".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(1.0f);

                }));
            }
            if (hediff.Severity <= 1.9f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedTwo".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(2.0f);

                }));
            }
            if (hediff.Severity <= 2.9f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedThree".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(3.0f);

                }));
            }
            if (hediff.Severity <= 3.9f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedFour".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(4.0f);

                }));
            }
            if (hediff.Severity <= 4.9f || MechtechSettings.DebugLogging)
            {
                options.Add(new FloatMenuOption("AV_DescGizmoUnlimitedFive".Translate().CapitalizeFirst(), delegate
                {
                    ChangeSeverity(5.0f);

                }));
            }
            if(options.Count >= 1)
            {
                Find.WindowStack.Add(new FloatMenu(options));
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (parent.pawn.Faction.def == FactionDefOf.PlayerColony || MechtechSettings.DebugLogging)
            {
                Command_Action choosemodule_Action = new Command_Action();
                choosemodule_Action.defaultLabel = "AV_LabelGizmoUnlimited".Translate().CapitalizeFirst();
                choosemodule_Action.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_Unlimiter");
                choosemodule_Action.defaultDesc = "AV_DescGizmoUnlimited".Translate().CapitalizeFirst();
                choosemodule_Action.action = delegate
                {
                    ChooseModule();
                };
                yield return choosemodule_Action;
            }
        }
    }
}
