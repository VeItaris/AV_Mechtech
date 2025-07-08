using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class CompProperties_RadioMech : CompProperties
    {
        public HediffDef radiohediff;

        public ThingDef mote;

        public EffecterDef radioswitchEffect;
        public CompProperties_RadioMech()
        {
            compClass = typeof(Comp_RadioMech);
        }
    }
}
