using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class CompUseEffect_UpgradeMechlink : CompUseEffect
    {
        public CompProperties_UseEffectUpgradeMechlink Props => (CompProperties_UseEffectUpgradeMechlink)props;

        public override void DoEffect(Pawn user)
        {
            BodyPartRecord bodyPartRecord = user.RaceProps.body.GetPartsWithDef(Props.bodyPart).FirstOrFallback();
            if (bodyPartRecord != null)
            {
                //add mechlink
                user.health.AddHediff(Props.hediffDef, bodyPartRecord);

                //remove basic mechlink
                Hediff firstHediffOfDef = user.health.hediffSet.GetFirstHediffOfDef(Props.hediffDefToRemove);
                if (firstHediffOfDef != null)
                {
                    user.health.RemoveHediff(firstHediffOfDef);
                }
            }
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (p.health.hediffSet.HasHediff(Props.hediffDef))
            {
                return "InstallImplantAlreadyInstalled".Translate(Props.hediffDef.label);
            }
            return true;
        }
    }
}
