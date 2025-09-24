using Verse;

namespace AV_Mechtech.Main
{
    [StaticConstructorOnStartup]
    public static class OnStartUp
    {
        static OnStartUp()
        {
            ProductionManager.UpdateProductionCount();
        }
    }

    public static class ProductionManager
    {
        public static void UpdateProductionCount()
        {

            if(MechtechSettings.NeurofoamProductionCount >= 1)
            MechtechDefOfs.AV_NeurofoamSpawn.countToSpawn = MechtechSettings.NeurofoamProductionCount;

            if (MechtechSettings.NeutroamineProductionCount >= 1)
                MechtechDefOfs.AV_NeutroamineSpawn.countToSpawn = MechtechSettings.NeutroamineProductionCount;

            if (MechtechSettings.HemogenePasteProductionCount >= 1)
                MechtechDefOfs.AV_HemogenPasteSpawn.countToSpawn = MechtechSettings.HemogenePasteProductionCount;

        }
    }



}
