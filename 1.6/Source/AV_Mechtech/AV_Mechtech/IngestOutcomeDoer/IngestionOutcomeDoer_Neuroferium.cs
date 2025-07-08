using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class IngestionOutcomeDoer_Neuroferium : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public float severity = -1f;

        public ChemicalDef toleranceChemical;

        public bool multiplyByGeneToleranceFactors;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (Rand.Chance(MechtechSettings.NeuroferiumInstantDeathChance))
            {
                pawn.health.AddHediff(hediffDef);
            }
        }
    }
}