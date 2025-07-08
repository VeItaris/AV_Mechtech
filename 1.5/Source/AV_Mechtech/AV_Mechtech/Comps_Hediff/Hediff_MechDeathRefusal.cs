using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.Sound;
using Verse;
using AV_Framework;

namespace AV_Mechtech
{
    // Verse.Hediff_DeathRefusal adjusted for mechs
    public class Hediff_MechDeathRefusal : HediffWithComps
    {
        protected int usesLeft;

        private TickTimer resurrectTimer = new TickTimer();

        private TickTimer warmupTimer = new TickTimer();

        private Sustainer resurrectSustainer;

        private bool resurrecting;

        private bool aiEnabled = true;

        private Effecter effecter;

        private Effecter resurrectAvailableEffecter;

        private Effecter resurrectUsedEffecter;

        //private static readonly CachedTexture Icon = new CachedTexture("UI/Abilities/SelfResurrect");

        private const float ScarredChance = 0f;

        private static readonly float ResurrectDurationSeconds = 3f;

        private static readonly FloatRange AIWarmupSeconds = new FloatRange(1f, 2f);

        public bool PlayerControlled
        {
            get
            {
                return pawn.IsColonyMech;
            }
        }

        public bool InProgress => !resurrectTimer.Finished;

        public int UsesLeft => usesLeft;

        public bool AIEnabled
        {
            get
            {
                return aiEnabled;
            }
            set
            {
                aiEnabled = value;
            }
        }

        public virtual int MaxUses => 2;

        public override string LabelInBrackets => UsesLeft + " " + ((UsesLeft > 1) ? "DeathRefusalUsePlural".Translate() : "DeathRefusalUseSingular".Translate()).ToString();

        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!ModLister.CheckAnomaly("Death refusal") || !pawn.RaceProps.IsMechanoid)
            {
                pawn.health.RemoveHediff(this);
                return;
            }
            base.PostAdd(dinfo);
            usesLeft = 1;
            overseer = MechanitorUtility.GetOverseer(pawn);
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            /*
            if (!pawn.Dead )//|| !PlayerControlled)
            {
                yield break;
            }

            Command_ActionWithLimitedUseCount _cmdSelfResurrect = new Command_ActionWithLimitedUseCount();
            _cmdSelfResurrect.defaultLabel = "DEV: Mech ressurect self";//"CommandSelfResurrect".Translate();
            _cmdSelfResurrect.defaultDesc = "CommandSelfResurrectDesc".Translate();
            _cmdSelfResurrect.usesLeftGetter = () => usesLeft;
            _cmdSelfResurrect.maxUsesGetter = () => MaxUses;
            _cmdSelfResurrect.UpdateUsesLeft();
            _cmdSelfResurrect.icon = Icon.Texture;
            _cmdSelfResurrect.action = delegate
            {
                if (resurrectTimer.Finished)
                {
                    Use();
                    _cmdSelfResurrect.UpdateUsesLeft();
                }
            };
            yield return _cmdSelfResurrect;
            */
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref usesLeft, "usesLeft", 0);
            Scribe_Values.Look(ref resurrecting, "resurrecting", defaultValue: false);
            Scribe_Values.Look(ref aiEnabled, "aiEnabled", defaultValue: false);
            Scribe_Deep.Look(ref resurrectTimer, "resurrectTimer");
            Scribe_Deep.Look(ref warmupTimer, "warmupTimer");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                resurrectTimer.OnFinish = Resurrect;
                warmupTimer.OnFinish = Use;
                if (!resurrecting && pawn.Dead)
                {
                    TryTriggerAIWarmupResurrection();
                }
            }

            Scribe_References.Look(ref overseer, "overseer");
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            //makes a double message as it resurrects instantly, so its a bit annoying
            /*
            if (PlayerControlled && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("AV_SelfResurrectText".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
            }
            */
            TryTriggerAIWarmupResurrection();
            TryTriggerReadyEffect();
        }

        private void TryTriggerAIWarmupResurrection()
        {
            //if (!PlayerControlled && !resurrecting && AIEnabled && usesLeft > 0)
            if (!resurrecting && AIEnabled && usesLeft > 0) //player controlled also uses it on its own - this iss an anomaly - player should have no control
            {
                resurrecting = true;
                warmupTimer.Start(GenTicks.TicksGame, AIWarmupSeconds.RandomInRange.SecondsToTicks(), Use);
            }
        }

        private void TryTriggerReadyEffect()
        {
            if (!resurrecting && usesLeft > 0 && resurrectAvailableEffecter == null && pawn.ParentHolder is Corpse corpse)
            {
                resurrectAvailableEffecter = EffecterDefOf.DeathRefusalAvailable.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(resurrectAvailableEffecter, corpse, 250);
            }
        }

        public override void Tick()
        {
            if (!resurrectTimer.Finished && pawn.ParentHolder is Corpse corpse)
            {
                if (effecter == null)
                {
                    effecter = EffecterDefOf.CellPollution.Spawn(corpse, pawn.MapHeld, Vector3.zero);
                    pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(effecter, corpse, 250);
                }
                if (resurrectSustainer == null)
                {
                    SoundInfo info = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    resurrectSustainer = SoundDefOf.Pawn_SelfResurrection.TrySpawnSustainer(info);
                }
            }

            if (pawn.IsHashIntervalTick(250))   //basicly tick rare
            {
                if (overseer == null && pawn.Faction == Faction.OfPlayer && !pawn.Dead)
                {
                    overseer = MechanitorUtility.GetOverseer(pawn);

                }
            }
        }

        public Pawn overseer = null;

        public void TickRare()  //only runs when called by Corpse.TickRare
        {
            if (!warmupTimer.Finished)
            {
                warmupTimer.TickInterval();
            }
            TryTriggerReadyEffect();
            if (!resurrectTimer.Finished)
            {
                resurrectTimer.TickInterval();
                if (resurrectSustainer != null && !resurrectSustainer.Ended)
                {
                    resurrectSustainer?.Maintain();
                }
            }
            if (effecter != null)
            {
                effecter.ticksLeft = ((!resurrectTimer.Finished) ? (effecter.ticksLeft + 250) : 0);
            }
            if (resurrectAvailableEffecter != null)
            {
                resurrectAvailableEffecter.ticksLeft += 250;
            }
        }

        private void Resurrect()
        {
            resurrectSustainer?.End();
            resurrecting = false;
            pawn.Drawer.renderer.SetAnimation(null);
            ResurrectionUtility.TryResurrect(pawn, new ResurrectionParams
            {
                gettingScarsChance = ScarredChance,
                canKidnap = false,
                canTimeoutOrFlee = false,
                useAvoidGridSmart = true,
                canSteal = false,
                invisibleStun = true
            });
            if (usesLeft == 0)
            {
                Severity = 0f;
            }
            /*
            if (overseer != null)
            {
                Log.Message("overseer = " + overseer);
            }*/

            if (pawn.Faction == Faction.OfPlayer && CanReconnect())
            {
                overseer.relations.AddDirectRelation(PawnRelationDefOf.Overseer, pawn);
            }

            if (pawn.HasComp<CompMechReloadableResourceHolder>())
            {
                CompMechReloadableResourceHolder comp = pawn.TryGetComp<CompMechReloadableResourceHolder>();

                comp.UseAllResources();
            }

            //pawn.health.AddHediff(HediffDefOf.DeathRefusalSickness);
        }

        public bool CanReconnect()
        {
            if (overseer == null || overseer.Dead)
            {
                return false;
            }

            if (pawn.GetStatValue(StatDefOf.BandwidthCost) <= overseer.mechanitor.TotalBandwidth - overseer.mechanitor.UsedBandwidth)
            {
                return true;    // has the bandwith to take control
            }

            return false;
        }


        public void SetUseAmountDirect(int amount, bool ignoreLimit = false)
        {
            if (ignoreLimit)
            {
                usesLeft = amount;
            }
            else
            {
                usesLeft = Mathf.Clamp(amount, 0, MaxUses);
            }
        }

        private void Use()
        {
            Messages.Message("AV_MessageUsingMechSelfResurrection".Translate(pawn), pawn, MessageTypeDefOf.NeutralEvent);
            resurrecting = true;
            usesLeft = Mathf.Max(usesLeft - 1, 0);
            resurrectTimer.Start(GenTicks.TicksGame, ResurrectDurationSeconds.SecondsToTicks(), Resurrect);
            resurrectAvailableEffecter?.ForceEnd();
            resurrectAvailableEffecter = null;
            if (pawn.ParentHolder is Corpse corpse)
            {
                resurrectUsedEffecter = EffecterDefOf.DeathRefusalUse.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(resurrectUsedEffecter, corpse, 1000);
                pawn.Drawer.renderer.SetAnimation(AnimationDefOf.DeathRefusalTwitches);
            }
        }

        public override void Notify_PawnCorpseDestroyed()
        {
            resurrectSustainer?.End();
        }

        public override string TipStringExtra
        {
            get
            {
                if (Prefs.DevMode)
                {
                    string x = "\n debug -- ";

                    if (overseer != null)
                    {
                        x += overseer.LabelShort.ToString();
                    }
                    else
                    {
                        x += "No overseer";
                    }

                    return base.TipStringExtra + x;
                }

                return base.TipStringExtra;

            }
        }




    }

}
