using RimWorld;
using Verse;

namespace AV_Mechtech
{

    [DefOf]
    public static class MechtechDefOfs
    {
#pragma warning disable CS0649 

        public static HediffDef AV_BasicMechlinkImplant;

        public static ThingDef AV_HemogenPaste;

        public static ThingDef AV_Carbonpanels;

        public static HediffDef AV_BandNode;

        public static PawnKindDef Mech_WarUrchin;

        public static ShaderTypeDef CutoutWithOverlay;

        public static BodyPartDef AV_MissileSilo;

        public static SoundDef Explosion_Rocket;

        public static StatDef WorkSpeedGlobalOffsetMech;

        public static StatDef MechFormingSpeed;

        public static HediffDef AV_Mechlink_UnlimiterImplant;


        [MayRequire("Ludeon.RimWorld.Royalty")]
        public static ThingDef Apparel_Gunlink;


        [MayRequire("Ludeon.RimWorld.Ideology")]
        public static SoundDef CombatCommand_Warmup;




        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef AV_SinistreMechDeathRefusal;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef AV_SinistrePower;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static ThingDef AV_Mech_Reshaper;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef AV_SinistreOvertake;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static ThingDef AV_Sinistre;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static JobDef AV_SinistreIngest;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static ThingDef AV_SinistreEssence;


        [MayRequire("Veltaris.Mechcrate")]
        public static HediffDef AV_Databreach;

        [MayRequire("Veltaris.Mechcrate")]
        public static HediffDef AV_MechMightConnectToHive;


        [MayRequire("sarg.alphamechs")]
        public static HediffDef AM_BeamncasterBandNode;

        [MayRequire("sarg.alphamechs")]
        public static HediffDef AM_VoidlinkBandNode;

        [MayRequire("Aoba.DeadManSwitch.Core")]
        public static HediffDef DMS_BandNode;

        public static AV_Framework.SpawnerDef AV_HemogenPasteSpawn;
        public static AV_Framework.SpawnerDef AV_NeutroamineSpawn;
        public static AV_Framework.SpawnerDef AV_NeurofoamSpawn;


        static MechtechDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MechtechDefOfs));
        }


#pragma warning restore CS0649
    }


    [DefOf]
    public static class MoreMechtechDefOfs
    {
#pragma warning disable CS0649 

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static PawnKindDef AV_Sinistre;

        static MoreMechtechDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoreMechtechDefOfs));
        }


#pragma warning restore CS0649
    }

}



