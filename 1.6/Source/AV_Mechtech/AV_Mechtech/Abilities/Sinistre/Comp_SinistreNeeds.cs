using AV_Framework;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using Verse.Sound;
using static Verse.ThingFilterUI;

namespace AV_Mechtech
{
    public class Comp_SinistreNeeds : ThingComp
    {
        public CompProperties_SinistreNeeds Props => (CompProperties_SinistreNeeds)props;

        public Pawn ParentAsPawn => parent as Pawn;

        public float BioferriteNeed = 0.5f; // we start above 0 so we don't instantly rage

        public float BioterriteNeedPerRareTick => SinistreUtility.BioterriteNeedPerDay / 240;

        public float HungerAmount => (BioferriteNeed - Props.maximumNeed) * -1;

        public bool IsFull => BioferriteNeed < SinistreUtility.BioferriteHungryThreshold;

        public bool IsHungry => BioferriteNeed <= SinistreUtility.BioferriteHungryThreshold;

        public bool IsExtremlyHungry => BioferriteNeed <= SinistreUtility.BioferriteExtremlyHungryThreshold;

        public bool MightEat => BioferriteNeed <= 1f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            /*
            if (ShouldBeDespawned)
            {
                Log.Error("Comp_SinistreNeeds -> despawning");
                parent.DeSpawn();
                ShouldBeDespawned = false;
            }
            */
        }

       // public bool ShouldBeDespawned = false;
        public override void CompTickRare()
        {
            if(ParentAsPawn.Spawned)
            {
                BioferriteNeed -= BioterriteNeedPerRareTick * SinistreUtility.SpawnedHungryFactor;
            }
            else
            {
                BioferriteNeed -= BioterriteNeedPerRareTick;
            }
        }

        public void EatAmount(float amount)
        {
            BioferriteNeed += amount;
        }



        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // fuck this shit, jusst use "Name Your Entities" instead 
            /*

            Command_Action command_Action_Rename = new Command_Action();
            command_Action_Rename.defaultLabel = "Rename".Translate();
            command_Action_Rename.icon = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");
            command_Action_Rename.action = delegate
            {
                //Find.WindowStack.Add(Dialog_RenameSinistre);
                Find.WindowStack.Add(RenameSinistre.NameSinistreDialog(ParentAsPawn));
                Name name = 

                ParentAsPawn.Name = name;

                // Find.WindowStack.Add(ParentAsPawn.NamePawnDialog());
            };

            yield return command_Action_Rename;

            */

            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (DebugSettings.godMode)
            {
                Command_Action command_Action_Feed = new Command_Action();
                command_Action_Feed.defaultLabel = "DEV: Feed till full";
                command_Action_Feed.defaultDesc = BioferriteNeed.ToString();
                command_Action_Feed.action = delegate
                {
                    BioferriteNeed = Props.maximumNeed;
                };
                yield return command_Action_Feed;

                Command_Action command_Action_MakeHungry = new Command_Action();
                command_Action_MakeHungry.defaultLabel = "DEV: make hungry";
                command_Action_MakeHungry.defaultDesc = BioferriteNeed.ToString();
                command_Action_MakeHungry.action = delegate
                {
                    BioferriteNeed = SinistreUtility.BioferriteHungryThreshold;
                };
                yield return command_Action_MakeHungry;

                Command_Action command_Action_MakeExtremlyHungry = new Command_Action();
                command_Action_MakeExtremlyHungry.defaultLabel = "DEV: make extremly hungry";
                command_Action_MakeExtremlyHungry.defaultDesc = BioferriteNeed.ToString();
                command_Action_MakeExtremlyHungry.action = delegate
                {
                    BioferriteNeed = SinistreUtility.BioferriteExtremlyHungryThreshold;
                };
                yield return command_Action_MakeExtremlyHungry;

                Command_Action command_Action_Despawn = new Command_Action();
                command_Action_Despawn.defaultLabel = "DEV: despawn!";
                command_Action_Despawn.defaultDesc = BioferriteNeed.ToString();
                command_Action_Despawn.action = delegate
                {
                    ParentAsPawn.DeSpawn(DestroyMode.Vanish);   //this works !!!!!!!!
                };
                yield return command_Action_Despawn;

            }
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref BioferriteNeed, "BioferriteNeed");
        }

        

    }


}
