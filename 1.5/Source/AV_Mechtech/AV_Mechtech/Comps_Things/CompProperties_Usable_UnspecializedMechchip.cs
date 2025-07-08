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
    public class CompProperties_Usable_UnspecializedMechchip : CompProperties
    {
        public JobDef useJob;

        [MustTranslate]
        public string useLabel;

        [MustTranslate]
        public string useMessage;

        public List<ThingDef> thingDefs;

        public int useDuration = 100;


        public MenuOptionPriority floatMenuOptionPriority = MenuOptionPriority.Default;
        /*
        public FactionDef floatMenuFactionIcon;

        public ThingDef warmupMote;

        public ThingDef finalizeMote;
        */
        public bool ignoreOtherReservations;


        public List<MutantDef> allowedMutants = new List<MutantDef>();

        public CompProperties_Usable_UnspecializedMechchip()
        {
            compClass = typeof(CompUsableUnspecializedMechchip);
        }
    }
}
