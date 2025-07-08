using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using AV_Framework;

namespace AV_Mechtech
{
    public class CultivationPod : Building
    {
        Comp_SelectableSpawner comp => GetComp<Comp_SelectableSpawner>();

        //public static readonly Color TankColorForNeurofoam = new Color(0.7f, 0.86f, 0.94f, 1f);

        public override Color DrawColor
        {
            get
            {
                return Color.white;
            }
        }

        public override Color DrawColorTwo
        {
            get
            {
                if (comp != null)
                {
                    if (comp.currentSpawnerDef.color != null)
                    {
                        return comp.currentSpawnerDef.color;
                    }
                }
                return Color.white;
            }
        }
    }
}
