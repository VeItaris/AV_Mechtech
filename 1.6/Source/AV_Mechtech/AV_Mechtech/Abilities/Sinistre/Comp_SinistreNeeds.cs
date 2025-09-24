using RimWorld;
using System.Collections.Generic;
using Verse;


namespace AV_Mechtech
{
    public class Comp_SinistreNeeds : ThingComp
    {
        public CompProperties_SinistreNeeds Props => (CompProperties_SinistreNeeds)props;

        public Pawn ParentAsPawn => parent as Pawn;

        public float BioferriteNeed = 0.5f; // we start above 0 so we don't instantly rage

        public float BioterriteNeedPerRareTick => (SinistreUtility.BioterriteNeedPerDay / 24);   //60.000 (1day) / 250(tick rare) = 24  //

       
        public float HungerPerRareTick => BioterriteNeedPerRareTick * SinistreUtility.BioferriteSatisfyNeed;

        public float HungerAmount => Props.maximumNeed - BioferriteNeed;

        public bool IsFull => BioferriteNeed < SinistreUtility.BioferriteFull;

        public bool IsHungry => BioferriteNeed <= SinistreUtility.BioferriteHungryThreshold;

        public bool IsExtremlyHungry => BioferriteNeed <= SinistreUtility.BioferriteExtremlyHungryThreshold;

        public bool MightEat => BioferriteNeed <= SinistreUtility.BioferriteCouldEatThreshold;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTickRare()
        {
            if(ParentAsPawn.Spawned)
            {
                //Log.Message("Sinistre hunger increased by " + (HungerPerRareTick * SinistreUtility.SpawnedHungryFactor) + "\nTotal BioferriteNeed = " + BioferriteNeed);
                BioferriteNeed -= (HungerPerRareTick * SinistreUtility.SpawnedHungryFactor);
            }
            else
            {
                //Log.Message("Sinistre hunger increased by " + (HungerPerRareTick * SinistreUtility.SpawnedHungryFactor) + "\nTotal BioferriteNeed = " + BioferriteNeed);
                BioferriteNeed -= HungerPerRareTick;
            }
        }

        public void EatAmount(float amount)
        {
            BioferriteNeed += amount;
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // fuck this shit, just use "Name Your Entities" instead 
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

        //we spawn essences in jobDriver_SinistreIngest or SinistrePawn!!
        /*
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            if (BioferriteNeed >= 0.9f)
            {
                BioferriteNeed = 0f;

                Thing essence = ThingMaker.MakeThing(MechtechDefOfs.AV_SinistreEssence);
                essence.stackCount = 1;

                GenPlace.TryPlaceThing(essence, parent.PositionHeld, prevMap, ThingPlaceMode.Near, out var lastResultingThing);

                //messages dont work in here?!
                
                if (lastResultingThing != null)
                {
                    Messages.Message("AV_SinistreGift".Translate(), MessageTypeDefOf.PositiveEvent);  
                    Find.LetterStack.ReceiveLetter("sinistre boon", "AV_SinistreGift".Translate(), LetterDefOf.PositiveEvent, lastResultingThing);
                }
                
            }
            base.Notify_Killed(prevMap, dinfo);
        }
        */

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref BioferriteNeed, "BioferriteNeed");
        }
    }


}
