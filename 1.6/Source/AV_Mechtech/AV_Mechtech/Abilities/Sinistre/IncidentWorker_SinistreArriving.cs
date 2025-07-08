using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class IncidentWorker_SinistreArriving : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 result = parms.spawnCenter;
            if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Hostile))
            {
                return false;
            }
            Rot4 rot = Rot4.FromAngleFlat((map.Center - result).AngleFlat);
            GenSpawn.Spawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(MoreMechtechDefOfs.AV_Sinistre, Faction.OfEntities, PawnGenerationContext.NonPlayer, map.Tile)), result, map, rot);
            return true;
        }

        public static GameComponent_Anomaly gameComp => Current.Game.GetComponent<GameComponent_Anomaly>();

        public override float ChanceFactorNow(IIncidentTarget target)
        {
            if(gameComp  == null)
            {
                return 0f;
            }


            if(!MechtechSettings.AllowSinistreArrivalWithoutMonolith && gameComp.Level < 1)
            {
                return 0f;
            }


            Map map = (Map)target;

            if(MapIsComfy(map, out float multiplier))
            {
                float chance = base.ChanceFactorNow(target);

                if(chance > 0)
                {
                    chance *= multiplier;
                }

                return chance;
            }
            return 0f;

        }

        public bool MapIsComfy(Map map, out float multiplier)
        {
           multiplier = 1f;
            if (map == null)
            {
                List<Thing> shardsOnMap = map.listerThings.ThingsOfDef(ThingDefOf.Shard);

                List<Thing> BioferriteOnMap = map.listerThings.ThingsOfDef(ThingDefOf.Bioferrite);

                List<Thing> ReshaperOnMap = map.listerThings.ThingsOfDef(MechtechDefOfs.AV_Mech_Reshaper);
                
                if (ReshaperOnMap.Count > 0)
                {
                    multiplier = 0.2f;  
                }

                if (shardsOnMap.Count >= SinistreUtility.ComfyShardCount)
                {
                    return true;
                }
                else if (BioferriteOnMap.Count >= SinistreUtility.ComfyBioferriteCount)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
