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
    [Obsolete]
    public class HediffCompProperties_MechlinkUnlimiter : HediffCompProperties
    {
        public HediffDef hediff;
        public ThingDef mote;

        public HediffCompProperties_MechlinkUnlimiter()
        {
            compClass = typeof(HediffComp_MechlinkUnlimiter);
        }
    }
}
