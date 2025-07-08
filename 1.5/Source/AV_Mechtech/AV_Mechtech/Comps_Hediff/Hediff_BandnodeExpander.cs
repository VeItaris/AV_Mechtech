using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class Hediff_BandnodeExpander : Hediff_Level
    {
        private HediffStage curStage;

        public float BandwidthGain = 0f;

        public bool UpdateStats = false;

        public override HediffStage CurStage
        {
            get
            {
                if (curStage == null)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    statModifier.value = BandwidthGain;
                    curStage = new HediffStage();
                    curStage.statOffsets = new List<StatModifier> { statModifier };
                }
                else if (UpdateStats)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    statModifier.value = BandwidthGain;
                    curStage = new HediffStage();
                    curStage.statOffsets = new List<StatModifier> { statModifier };
                    UpdateStats = false;
                }
                return curStage;
            }
        }


        public override void PostTick()
        {
            base.PostTick();

            if (pawn.IsHashIntervalTick(60))    //same as bandnodes
            {
                BandwidthGain = BandwidthFromBandnodes();

                BandwidthGain = (float)Math.Floor(BandwidthGain * 0.5f * level);

                UpdateStats = true;
            }
        }

        public float BandwidthFromBandnodes()
        {
            float counter = 0;

            pawn.health.hediffSet.TryGetHediff(HediffDefOf.BandNode, out Hediff Hediff_BandNode);

            if (Hediff_BandNode != null && Hediff_BandNode?.CurStage?.statOffsets != null)
            {
                foreach (StatModifier statModifier in Hediff_BandNode.CurStage.statOffsets)
                {
                    if (statModifier != null && statModifier.stat == StatDefOf.MechBandwidth)
                    {
                        counter += statModifier.value;
                        break;
                    }
                }
            }

            HediffDef Hediff_BandNodefueledDef = MechtechDefOfs.AV_BandNode;

            pawn.health.hediffSet.TryGetHediff(Hediff_BandNodefueledDef, out Hediff Hediff_BandNodeFueled);

            if (Hediff_BandNodeFueled != null && Hediff_BandNodeFueled?.CurStage?.statOffsets != null)
            {
                foreach (StatModifier statModifier in Hediff_BandNodeFueled.CurStage.statOffsets)
                {
                    if (statModifier != null && statModifier.stat == StatDefOf.MechBandwidth)
                    {
                        counter += statModifier.value;
                        break;
                    }
                }
            }
            #region alpha mechs
            HediffDef Hediff_BandNodeBeamncasterDef = MechtechDefOfs.AM_BeamncasterBandNode;

            if (Hediff_BandNodeBeamncasterDef != null)
            {
                pawn.health.hediffSet.TryGetHediff(Hediff_BandNodeBeamncasterDef, out Hediff Hediff_BandNodeBeamncaster);

                if (Hediff_BandNodeBeamncaster != null && Hediff_BandNodeBeamncaster?.CurStage?.statOffsets != null)
                {
                    foreach (StatModifier statModifier in Hediff_BandNodeBeamncaster.CurStage.statOffsets)
                    {
                        if (statModifier != null && statModifier.stat == StatDefOf.MechBandwidth)
                        {
                            counter += statModifier.value;
                            break;
                        }
                    }
                }
            }

            HediffDef Hediff_BandNodeVoidlinkDef = MechtechDefOfs.AM_VoidlinkBandNode;
            if (Hediff_BandNodeVoidlinkDef != null)
            {
                pawn.health.hediffSet.TryGetHediff(Hediff_BandNodeVoidlinkDef, out Hediff Hediff_BandNodeVoidlink);

                if (Hediff_BandNodeVoidlink != null && Hediff_BandNodeVoidlink?.CurStage?.statOffsets != null)
                {
                    foreach (StatModifier statModifier in Hediff_BandNodeVoidlink.CurStage.statOffsets)
                    {
                        if (statModifier != null && statModifier.stat == StatDefOf.MechBandwidth)
                        {
                            counter += statModifier.value;
                            break;
                        }
                    }
                }
             }

            #endregion

            #region dead mans switch
            HediffDef Hediff_ProcessArrayDef = MechtechDefOfs.DMS_BandNode;

            if (Hediff_ProcessArrayDef != null)
            {
                pawn.health.hediffSet.TryGetHediff(Hediff_ProcessArrayDef, out Hediff Hediff_ProcessArray);

                if (Hediff_ProcessArray != null && Hediff_ProcessArray?.CurStage?.statOffsets != null)
                {
                    foreach (StatModifier statModifier in Hediff_ProcessArray.CurStage.statOffsets)
                    {
                        if (statModifier != null && statModifier.stat == StatDefOf.MechBandwidth)
                        {
                            counter += statModifier.value;
                            break;
                        }
                    }
                }
            }

            #endregion
            

            return counter;
        }


        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref BandwidthGain, "Amount", 0);
        }
    }




}

