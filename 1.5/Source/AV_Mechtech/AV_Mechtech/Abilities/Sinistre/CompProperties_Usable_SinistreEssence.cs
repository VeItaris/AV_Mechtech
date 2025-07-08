using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class CompProperties_Usable_SinistreEssence : CompProperties
    {
        [MustTranslate]
        public string useLabel;

        [MustTranslate]
        public string useMessage;


        public JobDef useJob;

        public int useDuration = 600;


        public MenuOptionPriority floatMenuOptionPriority = MenuOptionPriority.Default;


        public bool ignoreOtherReservations;


        public List<MutantDef> allowedMutants = new List<MutantDef>();

        public CompProperties_Usable_SinistreEssence()
        {
            compClass = typeof(CompUsableSinistreEssence);
        }
    }
}
