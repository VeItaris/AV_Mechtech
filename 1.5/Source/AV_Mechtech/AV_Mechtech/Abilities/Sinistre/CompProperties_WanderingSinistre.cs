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
    public class CompProperties_WanderingSinistre : CompProperties
    {
        public PawnKindDef pawnKindDef;

        public PawnKindDef pawnKindDefShadow;

        public IntRange TimeToSpawn;

        public CompProperties_WanderingSinistre()
        {
            compClass = typeof(Comp_WanderingSinistre);
        }
    }
}
