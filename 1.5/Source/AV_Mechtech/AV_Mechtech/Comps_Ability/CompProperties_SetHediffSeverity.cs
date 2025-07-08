using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static HarmonyLib.Code;

namespace AV_Mechtech
{
   public class CompProperties_SetHediffSeverity : CompProperties_AbilityEffectWithDuration
   {
       public HediffDef hediffDef;

       public bool onlyBrain;

       public bool applyToSelf;

       public bool onlyApplyToSelf;

       public bool applyToTarget = true;

       public bool loop = false;

       public bool allowRemoving = false;

       public float SeverityChange = 1f;

       public float MaxSeverity = 1f;

       public float MinSeverity = 0.01f;
   }
}
