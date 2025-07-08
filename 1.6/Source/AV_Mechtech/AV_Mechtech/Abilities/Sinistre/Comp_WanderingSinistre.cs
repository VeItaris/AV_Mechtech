using AV_Framework;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using Verse.Sound;

namespace AV_Mechtech
{
    public class Comp_WanderingSinistre : ThingComp
    {
        public CompProperties_WanderingSinistre Props => (CompProperties_WanderingSinistre)props;

        public int TotalTicksToSpawn = 60;

        public int TicksToSpawn = 0;

        public Pawn Cachedsinistre;

       // public string SinistreID = "empty";

        public Pawn Sinistre
        {
            get
            {
                if (Cachedsinistre == null)
                {
                    /*
                    Pawn maybeSaved = gameComp.GetPawnById(SinistreID);

                    if(maybeSaved != null)
                    {
                        Cachedsinistre = maybeSaved;
                    }
                    else
                    {
                        Log.Error("Generating new sinistre for " + ParentAsPawn.Label);
                        Cachedsinistre = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.pawnKindDef, parent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f));
                        SinistreID = Cachedsinistre.ThingID;
                    }
                    */
                    //Log.Error("Generating new sinistre for " + ParentAsPawn.Label);
                    Cachedsinistre = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.pawnKindDef, parent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f));

                    //test!!
                    gameComp.SavePawn(Cachedsinistre);

                }
                return Cachedsinistre;
            }
        }

        public Hediff powerHediff;

        public float powerStage = 1;



        Pawn ParentAsPawn => parent as Pawn;

        CompMechReloadableResourceHolder comp => ParentAsPawn.TryGetComp<CompMechReloadableResourceHolder>();

        Comp_SinistreNeeds comp_SinistreNeeds => Sinistre.TryGetComp<Comp_SinistreNeeds>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            TotalTicksToSpawn = Props.TimeToSpawn.RandomInRange;

            if(parent is Pawn && comp != null)
            { 
            }
            else
            {
                Log.Error(parent.ToString() + " has AV_Mechtech.Comp_WanderingSinistre but is no pawn or has no CompMechReloadableResourceHolder. Destroying parent.");
                parent.Destroy();
            }
        }


        public bool SinistreAttached => ParentAsPawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistreMechDeathRefusal) != null;

        public bool HasSinistrePower => ParentAsPawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistrePower) != null;

        //private int TicksTillRimworldThicksItShouldDiscard = 0;





        public override void CompTick()
        {
            if (Sinistre.Dead && !Sinistre.Discarded)
            {
                TryReviveSinistre(); //discarding does not care
            }
            Sinistre.markedForDiscard = false; //discarding does not care

            /*
            if (Sinistre.Discarded)
            {
                Log.Error("AV_Mechtech.Comp_WanderingSinistre: sinistre was dicarded after ~" + TicksTillRimworldThicksItShouldDiscard + " ticks");
                
            }
            else
            {
                TicksTillRimworldThicksItShouldDiscard++;
            }
            */
        }

        public override void CompTickRare()
        {
            if (SinistreAttached)
            {
                TryAddSinistrePowerHediff();

                if (comp.IngredientCount == comp.Props.maxIngredientCount)
                {
                    TicksToSpawn += 250;
                    if (TicksToSpawn >= TotalTicksToSpawn && parent.Map != null)
                    {
                        TicksToSpawn = -Props.TimeToSpawn.RandomInRange;   //don't spawn for a longer time
                        SpawnSinistre(roam: true);
                    }
                }

                if(!Sinistre.Spawned && comp_SinistreNeeds != null) //just to be sure
                {
                    comp_SinistreNeeds.CompTickRare();

                    if (!Sinistre.Spawned && comp.HasResources(1))  //not spawned after tick checks
                    {
                        if(comp_SinistreNeeds.IsHungry)
                        {
                            TryFeedSinistre();
                        }
                    }

                    if (comp_SinistreNeeds.IsExtremlyHungry)
                    {
                        SpawnSinistre(addMentalState: true);
                        //spawn with mental state
                    }

                }

            }
            else if (HasSinistrePower)
            {
                Hediff h = ParentAsPawn.health.AddHediff(MechtechDefOfs.AV_SinistrePower);
                ParentAsPawn.health.RemoveHediff(h);
            }
        }

        public void TryFeedSinistre()
        {
            int amountToEat = SinistreUtility.AmountOfBioferriteToEat(comp_SinistreNeeds.HungerAmount);

            for (int i = 0; i < amountToEat; i++)
            {
                if(comp.HasResources(1))
                {
                    comp_SinistreNeeds.EatAmount(SinistreUtility.BioferriteSatisfyNeed);
                    comp.UseResources(1);
                }
            }
        }


        public void TryAddSinistrePowerHediff()
        {
            if (!HasSinistrePower)
            {
                Hediff h = ParentAsPawn.health.AddHediff(MechtechDefOfs.AV_SinistrePower);
                h.Severity = powerStage;
            }
        }

        public void TryIncreaseSinistrePower()
        {
            if (HasSinistrePower)
            {
                Hediff h = ParentAsPawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistrePower);
                
                powerStage += 1;

                if(powerStage >= h.def.maxSeverity)
                {
                    powerStage = h.def.maxSeverity;
                }

                h.Severity = powerStage;
                Messages.Message("AV_Message_ReshaperPowerUp".Translate(ParentAsPawn.Named("PAWN")), ParentAsPawn, MessageTypeDefOf.NeutralEvent);
            }
        }



        public void TryReviveSinistre()
        {
            if (Sinistre.Dead)
            {
                ResurrectionUtility.TryResurrect(Sinistre, new ResurrectionParams
                {
                    gettingScarsChance = 0,
                    canKidnap = false,
                    canTimeoutOrFlee = false,
                    useAvoidGridSmart = true,
                    canSteal = false,
                    invisibleStun = false,
                    restoreMissingParts = true
                });

                ResetSinistreHediffs();
            }
        }


        public void SpawnSinistre(bool addMentalState = false, bool addLord = true, bool consumeAllBioferrite = false, bool roam = false)
        {
            if(Sinistre.Spawned)
            {
                //Log.Warning("Comp_WanderingSinistre.SpawnSinistre: sinistre is already spawned, skipping...");
                return;
            }
            // mapIndexOrState -1 = despawned
            // mapIndexOrState -2 = destroyed
            // mapIndexOrState -3 = map removed

            if (Sinistre.Discarded)
            {
                Log.Error("AV_Mechtech.Comp_WanderingSinistre: FU rimworld, don't discard my pawns! Recreating a new one instead of error spam...");
                /*
                Cachedsinistre = null;
                Cachedsinistre = Sinistre;
                */
                Sinistre.ForceSetStateToUnspawned();
            }
            
            TryReviveSinistre();

            gameComp.RemovePawn(Sinistre);

            if (Sinistre.Faction != ParentAsPawn.Faction)
            {
                Sinistre.SetFaction(ParentAsPawn.Faction);
            }

            GenSpawn.Spawn(Sinistre, ParentAsPawn.Position, ParentAsPawn.Map, WipeMode.FullRefund);

            if(addLord)
            {
                ParentAsPawn.GetLord()?.AddPawn(Sinistre);
            }
            
            CompInspectStringEmergence compInspectStringEmergence = Sinistre.TryGetComp<CompInspectStringEmergence>();
            if (compInspectStringEmergence != null)
            {
                compInspectStringEmergence.sourcePawn = ParentAsPawn;
            }

            if(consumeAllBioferrite)
            {
                comp.UseAllResources();
            }

            if (roam)
            {
                Messages.Message("AV_Message_SinistreRoam".Translate(ParentAsPawn.Named("PAWN")), ParentAsPawn, MessageTypeDefOf.NeutralEvent);
            }
            

            Hediff hediff = ParentAsPawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistreMechDeathRefusal);
            if (hediff != null)
            {
                ParentAsPawn.health.RemoveHediff(hediff);
            }
            Hediff hediff_power = ParentAsPawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistrePower);
            if (hediff_power != null)
            {
                ParentAsPawn.health.RemoveHediff(hediff_power);
            }

            if(addMentalState)
            {
                Sinistre.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, forced: false, forceWake: true);
                Messages.Message("AV_Message_SinistreRoamHostile".Translate(ParentAsPawn.Named("PAWN")), ParentAsPawn, MessageTypeDefOf.NeutralEvent);
                ParentAsPawn.Kill(null);
            }
        }





        public void ResetSinistreHediffs()
        {
            List<Hediff> hediffsBefore = Sinistre.health.hediffSet.hediffs;

            foreach (Hediff hediff in hediffsBefore)
            {
                //if(hediff.def != MechtechDefOfs.AV_NeedsBioferrite)
                //{
                    Sinistre.health.RemoveHediff(hediff);
                //}
            }
        }



        public void SpawnSinistreShadows(int shouldSpawn = -1)
        {
            if(shouldSpawn == -1)
            {
                shouldSpawn = (int)Math.Floor(comp.ResourceCount / (float)SinistreUtility.ResourceCost_CallSinistreShadows);
            }

            for (int i = 0 ; i < shouldSpawn; i++)
            {
                //Log.Warning("SpawnSinistreShadows " + i);
                //Log.Warning("shouldSpawn " + shouldSpawn);
                if (comp.ResourceCount >= SinistreUtility.ResourceCost_CallSinistreShadows)
                {
                    comp.UseResources(SinistreUtility.ResourceCost_CallSinistreShadows);
                    Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.pawnKindDefShadow, parent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f));
                    GenSpawn.Spawn(child, ParentAsPawn.Position, ParentAsPawn.Map);
                    ParentAsPawn.GetLord()?.AddPawn(child);
                }
            }
        }


        public static GameComponent_Sinistre gameComp => Current.Game.GetComponent<GameComponent_Sinistre>();


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref TotalTicksToSpawn, "TotalTicksToSpawn");
            Scribe_Values.Look(ref TicksToSpawn, "TicksToSpawn");

            Scribe_Values.Look(ref powerStage, "powerStage");
            
            if(Scribe.mode == LoadSaveMode.Saving)
            {
                Scribe_References.Look(ref Cachedsinistre, "sinistre", true);

            }
            else
            {
                Scribe_References.Look(ref Cachedsinistre, "sinistre");
            }
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            base.CompGetGizmosExtra();

            if (DebugSettings.godMode)
            {
                Command_Action command_Action_Spawn_Sinistre = new Command_Action();
                command_Action_Spawn_Sinistre.defaultLabel = "DEV: Spawn Sinistre";
                command_Action_Spawn_Sinistre.defaultDesc = TicksToSpawn + " / " + TotalTicksToSpawn;
                command_Action_Spawn_Sinistre.action = delegate
                {
                    if(Sinistre.Spawned)
                    {
                        Messages.Message(Sinistre.Label + " is already spawned.", Sinistre, MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        SpawnSinistre();
                    }  
                };
                yield return command_Action_Spawn_Sinistre;

                Command_Action command_Action_Spawn_Sinistre_Roam = new Command_Action();
                command_Action_Spawn_Sinistre_Roam.defaultLabel = "DEV: Spawn roaming Sinistre";
                command_Action_Spawn_Sinistre_Roam.defaultDesc = TicksToSpawn + " / " + TotalTicksToSpawn;
                command_Action_Spawn_Sinistre_Roam.action = delegate
                {
                    if (Sinistre.Spawned)
                    {
                        Messages.Message(Sinistre.Label + " is already spawned.", Sinistre, MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        SpawnSinistre();
                    }
                };
                yield return command_Action_Spawn_Sinistre_Roam;


                Command_Action command_Action_IncreasePower = new Command_Action();
                command_Action_IncreasePower.defaultLabel = "DEV: Increase sinistre power";
                command_Action_IncreasePower.action = delegate
                {
                    if (!SinistreAttached)
                    {
                        Messages.Message(Sinistre.Label + " is not attached.", Sinistre, MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        if(ParentAsPawn?.health?.hediffSet?.GetFirstHediffOfDef(MechtechDefOfs.AV_SinistrePower) == null)
                        {
                            TryAddSinistrePowerHediff();
                        }
                        else
                        {
                            TryIncreaseSinistrePower();
                        }
                    }
                };
                yield return command_Action_IncreasePower;

                
                if(!Sinistre.Spawned && SinistreAttached && comp_SinistreNeeds != null)
                {
                    Command_Action command_Action_Feed = new Command_Action();
                    command_Action_Feed.defaultLabel = "DEV: Feed till full";
                    command_Action_Feed.defaultDesc = comp_SinistreNeeds.BioferriteNeed.ToString();
                    command_Action_Feed.action = delegate
                    {
                        comp_SinistreNeeds.BioferriteNeed = comp_SinistreNeeds.Props.maximumNeed;
                    };
                    yield return command_Action_Feed;

                    Command_Action command_Action_MakeHungry = new Command_Action();
                    command_Action_MakeHungry.defaultLabel = "DEV: make hungry";
                    command_Action_MakeHungry.defaultDesc = comp_SinistreNeeds.BioferriteNeed.ToString();
                    command_Action_MakeHungry.action = delegate
                    {
                        comp_SinistreNeeds.BioferriteNeed = SinistreUtility.BioferriteHungryThreshold;
                    };
                    yield return command_Action_MakeHungry;

                    Command_Action command_Action_MakeExtremlyHungry = new Command_Action();
                    command_Action_MakeExtremlyHungry.defaultLabel = "DEV: make extremly hungry";
                    command_Action_MakeExtremlyHungry.defaultDesc = comp_SinistreNeeds.BioferriteNeed.ToString();
                    command_Action_MakeExtremlyHungry.action = delegate
                    {
                        comp_SinistreNeeds.BioferriteNeed = SinistreUtility.BioferriteExtremlyHungryThreshold;
                    };
                    yield return command_Action_MakeExtremlyHungry;

                    Command_Action command_Action_Reset = new Command_Action();
                    command_Action_Reset.defaultLabel = "DEV: Reset sinistre";
                    command_Action_Reset.action = delegate
                    {
                        Cachedsinistre = null;
                    };
                    yield return command_Action_Reset;

                }

                
               



            }

        }



    }
}
