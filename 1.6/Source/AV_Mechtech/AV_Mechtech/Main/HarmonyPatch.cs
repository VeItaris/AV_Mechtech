using AV_Framework;
using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using UnityEngine;
using Verse.AI;
using RimWorld.Planet;


namespace AV_Mechtech
{
    public class AV_MechtechMod : Mod
    {
        private static AV_MechtechMod _instance;

        public static AV_MechtechMod Instance => _instance;

        public AV_MechtechMod(ModContentPack content)
            : base(content)
        {
            Harmony harmony = new Harmony("AV_MechtechMod");
            harmony.PatchAll();
            _instance = this;
        }
    }




    [HarmonyPatch(typeof(JobGiver_GetHemogen))]
    [HarmonyPatch("TryGiveJob")]
    public static class JobGiver_GetHemogen_TryGiveJob_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Job __result, Pawn pawn)
        {
            if (__result == null && HemogenPaste != null)
            {
                if (!ModsConfig.BiotechActive)
                {
                    return;
                }
                Gene_Hemogen gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
                if (gene_Hemogen == null)
                {
                    return;
                }
                if (!gene_Hemogen.ShouldConsumeHemogenNow())
                {
                    return;
                }
                if (gene_Hemogen.hemogenPacksAllowed)
                {
                    int num = Mathf.FloorToInt((gene_Hemogen.Max - gene_Hemogen.Value) / HemogenPasteHemogenGain);
                    if (num > 0)
                    {
                        Thing hemogenPack = GetHemogenPaste(pawn);
                        if (hemogenPack != null)
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.Ingest, hemogenPack);
                            job.count = Mathf.Min(hemogenPack.stackCount, num);
                            job.ingestTotalCount = true;
                            __result = job;
                        }
                    }
                }
                return;
            }
        }

        public static ThingDef HemogenPaste = MechtechDefOfs.AV_HemogenPaste;

        public static float HemogenPasteHemogenGain
        {
            get
            {
                if (HemogenPaste != null)
                {
                    if (!(HemogenPaste.ingestible?.outcomeDoers?.FirstOrDefault((IngestionOutcomeDoer x) => x is IngestionOutcomeDoer_OffsetHemogen) is IngestionOutcomeDoer_OffsetHemogen ingestionOutcomeDoer_OffsetHemogen))
                    {
                        return 0f;
                    }
                    else
                    {
                        return ingestionOutcomeDoer_OffsetHemogen.offset;
                    }
                }

                return 0.15f;   //hardcoded for now
            }
        }

        private static Thing GetHemogenPaste(Pawn pawn)
        {
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing != null && carriedThing.def == HemogenPaste)
            {
                return carriedThing;
            }
            for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
            {
                if (pawn.inventory.innerContainer[i].def == HemogenPaste)
                {
                    return pawn.inventory.innerContainer[i];
                }
            }
            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(HemogenPaste), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
        }
    }



    [HarmonyPatch(typeof(PreceptWorker_Apparel))]
    [HarmonyPatch("IsValidApparel")]
    public static class PreceptWorker_Apparel_IsValidApparel_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ThingDef td)
        {
            if(MechtechSettings.CarbonPanelsForIdeo)
            {
                if (__result == false && CarbonPanels != null && td == CarbonPanels)
                {
                    __result = true;
                }
            }
        }
        public static ThingDef CarbonPanels = MechtechDefOfs.AV_Carbonpanels;
    }



    //Explode missile in silo when destroyed   | PostRemoved patch not needed, will update graphics by default
    [HarmonyPatch(typeof(Hediff_MissingPart))]
    [HarmonyPatch("PostAdd")]
    public static class Hediff_MissingPart_PostAdd_MissileSiloExplodes
    {
        [HarmonyPostfix]
        public static void PostAdd(ref Hediff_MissingPart __instance, DamageInfo? dinfo)
        {
            if(__instance.Part.def == MechtechDefOfs.AV_MissileSilo)
            {
                CompMechReloadableResourceHolder comp = __instance.pawn.TryGetComp<CompMechReloadableResourceHolder>();

                if(comp != null)
                {
                    TarantulaUtility.ExplodeAllRemainingMissiles(__instance.pawn, comp);
                }
            }
        }
    }


    /*
    [HarmonyPatch(typeof(Pawn_MechanitorTracker))]
    [HarmonyPatch("CanCommandTo")]
    // above is same as [HarmonyPatch(typeof(Pawn_MechanitorTracker), nameof(Pawn_MechanitorTracker.CanCommandTo))]
    public static class AllowUnlimitedCommandRageForSinistreOvertake
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_MechanitorTracker __instance, LocalTargetInfo target, ref bool __result)    //__instance.Pawn = mechanitor  //target is a place
        {
            if (__result == false && MechtechDefOfs.AV_Mech_Reshaper != null && MechtechDefOfs.AV_SinistreOvertake != null)
            {
                __result = OnlyValidReshaperSelected();
            }
            return;
        }

        public static bool OnlyValidReshaperSelected()
        {
            //single
            if(Find.Selector?.SingleSelectedThing?.def == MechtechDefOfs.AV_Mech_Reshaper)
            {
                if (Find.Selector.SingleSelectedThing is Pawn reshaper)
                {
                    if (reshaper.health.hediffSet.HasHediff(MechtechDefOfs.AV_SinistreOvertake))
                    {
                        return true;
                    }
                }
            }

            //multiple
            int counter = 0;
            foreach (Pawn p in Find.Selector.SelectedPawns)
            {
                if(p.def == MechtechDefOfs.AV_Mech_Reshaper && p.health.hediffSet.HasHediff(MechtechDefOfs.AV_SinistreOvertake))
                {
                    counter++;
                }
            }

            if(counter != 0 && counter == Find.Selector.SelectedPawns.Count)
            {
                return true;
            }

            return false;
        }


    }
    */


    [HarmonyPatch(typeof(MechanitorUtility))]
    [HarmonyPatch("InMechanitorCommandRange")]
    public static class AllowUnlimitedCommandRageForSinistreOvertake
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)    //__instance.Pawn = mechanitor  //target is a place
        {
            if (__result == false && MechtechDefOfs.AV_Mech_Reshaper != null && MechtechDefOfs.AV_SinistreOvertake != null)
            {
                if (mech.def == MechtechDefOfs.AV_Mech_Reshaper)
                {
                    if (mech.health.hediffSet.HasHediff(MechtechDefOfs.AV_SinistreOvertake))
                    {
                        __result = true;
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch("PawnCanOpen")]
    public static class SinistreCanOpenDoors
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn p, ref bool __result)    //__instance.Pawn = mechanitor  //target is a place
        {

            if (__result == false && p.def == MechtechDefOfs.AV_Sinistre)
            {
                __result = true;
            }
        }
    }


    [HarmonyPatch(typeof(MechanitorUtility))]
    [HarmonyPatch("CanDraftMech")]
    public static class AllowReshaperDraftingForSinistreOvertake
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn mech, ref AcceptanceReport __result)    //__instance.Pawn = mechanitor  //target is a place
        {

            if (MechtechDefOfs.AV_Mech_Reshaper != null && mech.def == MechtechDefOfs.AV_Sinistre && MechtechDefOfs.AV_SinistreOvertake != null)
            {
                if (mech.health.hediffSet.HasHediff(MechtechDefOfs.AV_SinistrePower))
                {
                    __result = true;
                }
            }
        }
    }




    //how the fuck is this blocking quest mechs from getting removed from world pawns
    /*
    [HarmonyPatch(typeof(WorldPawns))]
    [HarmonyPatch("RemovePawn")]
    public static class SinistreDontGoToHeaven
    {
        public static GameComponent_Sinistre gameComp => Current.Game.GetComponent<GameComponent_Sinistre>();

        [HarmonyPrefix]
        public static bool Prefix(Pawn p)    
        {
            if (p.def == MechtechDefOfs.AV_Sinistre && p.Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(p);
                Log.Warning("WorldPawns: Skip removing sinistre from WorldPawns");
                return true; // skip
            }
            return false; // no skip
        }
    }
    */

    [HarmonyPatch(typeof(WorldPawns))]
    [HarmonyPatch("Notify_PawnDestroyed")]
    public static class SinistreDontGoToHell
    {
        public static GameComponent_Sinistre gameComp => Current.Game.GetComponent<GameComponent_Sinistre>();

        [HarmonyPrefix]
        public static bool Prefix(Pawn p)
        {
            if (p.def == MechtechDefOfs.AV_Sinistre && p.Faction == Faction.OfPlayer)
            {
                gameComp.SavePawn(p);
                //Log.Warning("Not adding sinistre to WorldPawns");
                return true; // skip
            }
            return false; // no skip
        }
    }
    



    //Hediff_MechDeathRefusal
    public static class Corpse_TickRare_Patch
    {
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare", MethodType.Normal)]
        public static class Corpse_TickRare
        {
            [HarmonyPostfix]
            public static void Postfix(ref Corpse __instance)
            {
                if (ModsConfig.AnomalyActive && __instance.InnerPawn?.RaceProps?.IsMechanoid == true)   // null = false
                {
                    Hediff_MechDeathRefusal firstHediff = __instance.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_MechDeathRefusal>();

                    if (firstHediff != null)
                    {
                        firstHediff.TickRare();
                        if (__instance.Destroyed)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }






}
