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
    public class CompProperties_SinistreNeeds : CompProperties
    {
        public float maximumNeed = 1f;

        public CompProperties_SinistreNeeds()
        {
            compClass = typeof(Comp_SinistreNeeds);
        }
    }
}
