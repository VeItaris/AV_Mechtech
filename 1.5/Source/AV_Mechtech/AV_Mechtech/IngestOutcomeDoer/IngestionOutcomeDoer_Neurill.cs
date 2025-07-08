using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class IngestionOutcomeDoer_Neurill : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public HediffDef hediffDef_nonMechanitor;

        public float severity = -1f;

        public ChemicalDef toleranceChemical;

#pragma warning disable CS0649 // Field 'IngestionOutcomeDoer_Neurill.divideByBodySize' is never assigned to, and will always have its default value false
        private bool divideByBodySize;
#pragma warning restore CS0649 // Field 'IngestionOutcomeDoer_Neurill.divideByBodySize' is never assigned to, and will always have its default value false

        public bool multiplyByGeneToleranceFactors;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            Hediff hediff;

            if (MechanitorUtility.IsMechanitor(pawn))
            {           
                hediff = HediffMaker.MakeHediff(hediffDef, pawn);
            }
            else
            {
                hediff = HediffMaker.MakeHediff(hediffDef_nonMechanitor, pawn);
            }

            float effect = ((!(severity > 0f)) ? hediffDef.initialSeverity : severity);
            if (divideByBodySize)
            {
                effect /= pawn.BodySize;
            }
            AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize_NewTemp(pawn, toleranceChemical, ref effect, multiplyByGeneToleranceFactors);
            hediff.Severity = effect;
            pawn.health.AddHediff(hediff);
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (!parentDef.IsDrug || !(chance >= 1f))
            {
                yield break;
            }
            foreach (StatDrawEntry item in hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
            {
                yield return item;
            }
        }
    }

}
