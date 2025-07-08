using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AV_Mechtech
{
    public class GameComponent_Sinistre : GameComponent
    {
        private List<Pawn> pawnsToSave = new List<Pawn>();

        public GameComponent_Sinistre(Game game)
        {
        }

        public void SavePawn(Pawn p)
        {
            if(!pawnsToSave.Contains(p))
            {
                pawnsToSave.Add(p);
                //Log.Message("Added " + p.Label + " to GameComponent_Sinistre");
            }
        }

        public void RemovePawn(Pawn p)
        {
            if (pawnsToSave.Contains(p))
            {
                pawnsToSave.Remove(p);
                //Log.Message("Removed " + p.Label + " from GameComponent_Sinistre");
            }
        }

        //public List<Pawn> NotSpawnedPawns => pawnsToSave;
        /*
        public Pawn GetPawnById(string thingID)
        {
            foreach (Pawn p in pawnsToSave)
            {
                if(p.ThingID == thingID)
                {
                    return p;
                }
            }
            return null;
        }
        */


        //List<Pawn> NotSpawnedPawns => (List<Pawn>)pawnsToSave.Where(p => !p.Spawned);   //filter them out when you spawn them

        public override void ExposeData()
        {
            base.ExposeData();

            //below is fucking things up
            /*
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Log.Message("GameComponent_Sinistre -> ExposeData -> 1");
                List<Pawn> temp = pawnsToSave;

                foreach (Pawn p in pawnsToSave)
                {
                    Log.Message("GameComponent_Sinistre -> ExposeData -> 2");
                    if (p.Spawned)
                    {
                        temp.Remove(p);
                        Log.Message("GameComponent_Sinistre -> ExposeData -> 3");
                        //pawnsToSave.Add(p);
                    }
                }
                pawnsToSave = temp;
                Log.Message("GameComponent_Sinistre -> ExposeData -> 4");
            }
           
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                pawnsToSave = NotSpawnedPawns;
            }
             */
            

            //Log.Message("Saved/Loaded GameComponent_Sinistre");

            Scribe_Collections.Look(ref pawnsToSave, "pawnsToSave", LookMode.Deep);
            /*
            if(!pawnsToSave.NullOrEmpty())
            {
                foreach (Pawn p in pawnsToSave)
                {
                    Log.Message("Saved/Loaded " + p.Label + " (" + p.ThingID + ") in GameComponent_Sinistre");
                }
            }
            */
            
        }

 


        public override void GameComponentTick()
        {
           
        }
    }
}
