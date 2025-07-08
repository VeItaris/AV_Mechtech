using AV_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AV_Mechtech
{
    public class CompProperties_GiveHediffsWhenAlive_Reshaper : CompProperties
    {

        public HediffDef hediff;

        public bool UseCooldown = false;

        public int CooldownTicks = 60000; // 1 day 

        public bool NeedsGraphicUpdate = false;

        public CompProperties_GiveHediffsWhenAlive_Reshaper()
        {
            compClass = typeof(Comp_GiveHediffsWhenAlive_Reshaper);
        }

    }
}
