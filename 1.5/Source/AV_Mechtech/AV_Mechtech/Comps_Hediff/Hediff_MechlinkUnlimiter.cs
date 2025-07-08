using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static HarmonyLib.Code;

namespace AV_Mechtech
{
    public class Hediff_MechlinkUnlimiter : HediffWithComps
    {
        /*
        private HediffStage curStage;

        public float BandwidthGain => Severity * 2;

        public float WorkSpeedGlobalOffsetMechGain => Severity * 0.10f;

        public float PsychicSensitivityGain => Severity * 0.05f;

        public float MechRepairSpeedGain => Severity * 0.10f;

        public float MechFormingSpeedGain => Severity * 0.15f;
        

        public float MechBandwidthFactorGain => 1 + (Severity * 0.10f);

        //public float ConsciousnessFactorGain => 1 - (Severity * 0.10f);


        public bool UpdateStats = false;

        */
        //overriding this somehow confuses rimworld to not properly update stat factors...
        /*
        public override HediffStage CurStage    //this is called EVERY TICK!!! Cached in curStage...
        {
            get
            {
                if(UpdateStats || curStage == null)
                {
                    //curStage.label = Label + " (stage" + Severity + ")";  //crashes the game

                    //order is also the order shown when hovering over the hediff description

                    curStage = def.stages[CurStageIndex];

                    //MechBandwidth
                    StatModifier statModifier_bandwidth = new StatModifier();
                    statModifier_bandwidth.stat = StatDefOf.MechBandwidth;
                    statModifier_bandwidth.value = BandwidthGain;
                    curStage.statOffsets = new List<StatModifier> { statModifier_bandwidth };

                    //MechBandwidthFactor
                    StatModifier statModifier_bandwidthfactor = new StatModifier();
                    statModifier_bandwidthfactor.stat = StatDefOf.MechBandwidth;
                    statModifier_bandwidthfactor.value = MechBandwidthFactorGain;
                    curStage.statFactors = new List<StatModifier> { statModifier_bandwidthfactor };


                    //WorkSpeedGlobalOffsetMech
                    StatModifier statModifier_workspeed = new StatModifier();
                    statModifier_workspeed.stat = MechtechDefOfs.WorkSpeedGlobalOffsetMech;
                    statModifier_workspeed.value = WorkSpeedGlobalOffsetMechGain;
                    curStage.statOffsets.Add(statModifier_workspeed);


                    //MechRepairSpeed
                    StatModifier statModifier_mechrepairspeed = new StatModifier();
                    statModifier_mechrepairspeed.stat = StatDefOf.MechRepairSpeed;
                    statModifier_mechrepairspeed.value = MechRepairSpeedGain;
                    curStage.statOffsets.Add(statModifier_mechrepairspeed);

                    // MechFormingSpeed
                    StatModifier statModifier_mechformingspeed = new StatModifier();
                    statModifier_mechformingspeed.stat = MechtechDefOfs.MechFormingSpeed;
                    statModifier_mechformingspeed.value = MechFormingSpeedGain;
                    curStage.statOffsets.Add(statModifier_mechformingspeed);


                    //PsychicSensitivity
                    StatModifier statModifier_psychicsensitivity = new StatModifier();
                    statModifier_psychicsensitivity.stat = StatDefOf.PsychicSensitivity;
                    statModifier_psychicsensitivity.value = PsychicSensitivityGain;
                    curStage.statOffsets.Add(statModifier_psychicsensitivity);


                    

                    //Hediff myhediff = pawn.health.hediffSet.GetFirstHediffOfDef(MechtechDefOfs.AV_Mechlink_UnlimiterImplant);
                    //pawn.health.Notify_HediffChanged(myhediff);   //crashes the game






                    //sometimes the game still thinks the pawn has 10% more consciousness, sso we update it here anyway
                    pawn.health.capacities.Clear();   //updates other capacities -> but pawn still thinks it can't walk
                    pawn.health.capacities.Notify_CapacityLevelsDirty();  //updates other capacities -> but pawn still thinks it can't walk

                    UpdateStats = false;
                }

                return curStage;
            }
        }

        */
        /*
                    //ConsciousnessLoss //forget this shit it is now in xml
                    PawnCapacityModifier statModifier_ConsciousnessFactor = new PawnCapacityModifier();
                    statModifier_ConsciousnessFactor.capacity = PawnCapacityDefOf.Consciousness;
                    statModifier_ConsciousnessFactor.postFactor = ConsciousnessFactorGain;
                    curStage.capMods = new List<PawnCapacityModifier> { statModifier_ConsciousnessFactor };
                    */

        /*
        public List<PawnCapacityModifier> capMods = new List<PawnCapacityModifier>();

        public List<HediffGiver> hediffGivers;

        public List<MentalStateGiver> mentalStateGivers;

        public List<StatModifier> statOffsets;

        public List<StatModifier> statFactors;
        
        */
        /*
        public override void PostTick()
        {
            base.PostTick();
            
            if (pawn.IsHashIntervalTick(60))    //same as bandnodes
            {
                if (Severity != DedicatedSeverity)
                {
                    UpdateServ();
                    Log.Message("Im ticking 1");
                }
            }    
        }
        */
        /*public float DedicatedSeverity = 1;

        public void UpdateServ()
        {
            Severity = DedicatedSeverity;
            UpdateStats = true;
        }
        */

        public override IEnumerable<Gizmo> GetGizmos()
        {
            base.GetGizmos();

            if(pawn.Drafted)
            {
                yield break;
            }
            
            if (Severity < 9)
            {
                Command_Action choosemodule_Action_add = new Command_Action();
                choosemodule_Action_add.defaultLabel = "AV_LabelGizmoUnlimited".Translate().CapitalizeFirst();
                choosemodule_Action_add.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_Unlimiter");
                choosemodule_Action_add.defaultDesc = "AV_DescGizmoUnlimited".Translate().CapitalizeFirst();
                choosemodule_Action_add.Disabled = GizmoDisabled();
                choosemodule_Action_add.action = delegate
                {

                    if (DebugSettings.godMode)
                    {
                        IncreaseSeverity();
                    }
                    else
                    {
                        Action action = delegate
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera();
                            IncreaseSeverity();
                        };
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AV_MechlinkUnlimiterWarning".Translate(), action));
                    }

                        

                };
               
                yield return choosemodule_Action_add;
            }
            if (Prefs.DevMode && DebugSettings.godMode)
            {
                if (Severity > 1)
                {
                    Command_Action choosemodule_Action_reduce = new Command_Action();
                    choosemodule_Action_reduce.defaultLabel = "DEV: reduce severity";   //"AV_LabelGizmoUnlimited".Translate().CapitalizeFirst();
                    choosemodule_Action_reduce.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_Unlimiter_decrease");
                    choosemodule_Action_reduce.defaultDesc = "If the pawn is still downed when it shouldn't be: put speed at 1 and increase + decrease again.";
                    choosemodule_Action_reduce.action = delegate
                    {
                        Severity--;
                        //UpdateStats = true;
                    };
                    yield return choosemodule_Action_reduce;
                }
            }


            /*
            if (DedicatedSeverity > 4)
            {
                #region Dis-/connect
                
                Command_Action remoteControll_Action = new Command_Action();
                remoteControll_Action.defaultLabel = "AV_GizmoLabel_Cryopod_Connect".Translate();
                remoteControll_Action.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_remote_connect");
                remoteControll_Action.defaultDesc = "AV_GizmoDesc_Cryopod_Connect".Translate().CapitalizeFirst();
                remoteControll_Action.action = delegate
                {
                    Find.Targeter.BeginTargeting(RemoteConnectTargetingParameters(), StartToConnect, Highlight, CanRemoteConnect);
                };
                yield return remoteControll_Action;
                

                Command_Action disconnect_Action = new Command_Action();
                disconnect_Action.defaultLabel = "AV_GizmoLabel_Cryopod_Disconnect".Translate();
                disconnect_Action.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AV_remote_disconnect");
                disconnect_Action.defaultDesc = "AV_GizmoDesc_Cryopod_Disconnect".Translate().CapitalizeFirst();
                disconnect_Action.action = delegate
                {
                    Find.Targeter.BeginTargeting(RemoteDisconnectTargetingParameters(), StartToDisconnect, Highlight, CanRemoteDisconnect);
                };
                yield return disconnect_Action;

                #endregion
            
            }
            */
        }

        public void IncreaseSeverity()
        {
            Severity++;
            //UpdateStats = true;
        }

        public bool GizmoDisabled()
        {
            /*
            if(Find.TickManager.Paused)
            {
                return true;
            }
            */
            if (DebugSettings.godMode)
            {
                return false;
            }
            if (pawn.InMentalState || pawn.Downed)
            {
                return true;
            }

            return false;
        }

        /*
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
        }

        public override void ExposeData()
        {
            base.ExposeData();

        }

        */

    }




}

