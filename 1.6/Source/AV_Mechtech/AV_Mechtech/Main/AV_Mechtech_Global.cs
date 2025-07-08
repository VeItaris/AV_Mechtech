using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;
using static AV_Framework.SettingsHelper;
//using static AV_Framework.FrameworkSettings;
using AV_Framework;

namespace AV_Mechtech
{
    public class MechtechSettings : ModSettings
    {
        //default values for settings

        public static int DefaultHackingLanceBandwidth          = 3;

        public static float DefaultNeuroferiumInstantDeathChance = 0.02f;   //not death chance anymore, its a sideeffect coma now

        public static bool DefaultDebugLogging                  = false;
        public static bool DefaultDebugOnLoad                   = false;

        //apply default values for settings on first start with this mod

        public static int HackingLanceBandwidth         = DefaultHackingLanceBandwidth;

        public static float NeuroferiumInstantDeathChance = DefaultNeuroferiumInstantDeathChance;

        public static bool DebugLogging                 = DefaultDebugLogging;
        public static bool DebugOnLoad                  = DefaultDebugOnLoad;

        public static bool CarbonPanelsForIdeo          = false;

        public static bool AllowSinistreArrivalWithoutMonolith = false;

        public static bool AllowTarantulaPlayerAIMissiles = false;


        // The part that writes our settings to file. Note that saving is by ref.
        public override void ExposeData()
        {
            Scribe_Values.Look(ref HackingLanceBandwidth, "HackingLanceBandwidth");
            Scribe_Values.Look(ref CarbonPanelsForIdeo, "CarbonPanelsForIdeo");
            Scribe_Values.Look(ref AllowSinistreArrivalWithoutMonolith, "AllowSinistreArrivalWithoutMonolith");
            Scribe_Values.Look(ref AllowTarantulaPlayerAIMissiles, "AllowTarantulaPlayerAIMissiles");

            

            Scribe_Values.Look(ref DebugLogging, "DebugLogging");
            Scribe_Values.Look(ref DebugOnLoad, "DefaultDebugOnLoad");

            base.ExposeData();
        }
    }

    public class MechtechMod : Mod
    {
#pragma warning disable IDE0052 // Remove unread private members
        readonly MechtechSettings settings;
#pragma warning restore IDE0052 // Remove unread private members

        // A mandatory constructor which resolves the reference to our settings.
        public MechtechMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MechtechSettings>();
        }

        private int CurrentPage = (int)AV_SettingsFolder.Left;

        private Texture2D Steve => ContentFinder<Texture2D>.Get("UI/Widgets/AV_centipede");
        private Texture2D Steve_bugs => ContentFinder<Texture2D>.Get("UI/Widgets/AV_bouble_bugs");
        private Texture2D Steve_nodev => ContentFinder<Texture2D>.Get("UI/Widgets/AV_bouble_nodev");

        private string label = "";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            int Folderchanged = DrawFolderStructure(inRect, CurrentPage, "Content-Settings", "DEV-Settings");

            if (Folderchanged != 0)
            {
                CurrentPage = Folderchanged;
            }
            CorrectCurrentFolder(CurrentPage, 2);

            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);

            Foldergap(listingStandard);

            if (CurrentPage == (int)AV_SettingsFolder.Center)
            {
                DrawDEVSettings(listingStandard, inRect);
            }
            else if (CurrentPage == (int)AV_SettingsFolder.Left)
            {
                DrawContentSettings(listingStandard, inRect);
            }

            listingStandard.End();


            Rect nameRect = DrawLogo(Steve, inRect);
            nameRect.y -= 5f;
            if (CurrentPage == (int)AV_SettingsFolder.Center)
            {
                if (Prefs.DevMode)
                {
                    DrawCenteredLabelAdjusted(nameRect, "Steve", x_offset: 2f);
                    DrawLogo(Steve_bugs, inRect, 160, 150);
                }
                else
                {
                    DrawCenteredLabelAdjusted(nameRect, "Steve", true, "_____", x_offset: 2f, x_offset_drafted: 1f);
                    DrawLogo(Steve_nodev, inRect, 140, 150);
                }
            }
            else
            {
                DrawCenteredLabelAdjusted(nameRect, "Steve", x_offset: 2f);
            }


            base.DoSettingsWindowContents(inRect);
        }


        public void DrawContentSettings(Listing_Standard listingStandard, Rect inRect)
        {
            Rect labelRect = inRect;
            labelRect.y = listingStandard.CurHeight;
            DrawCenteredLabel(listingStandard, labelRect, "AV_SettingsLabel_general".Translate().CapitalizeFirst());
            listingStandard.GapLine();

            listingStandard.CheckboxLabeled("Frameworksetting: " + "AV_SettingsBoxLabel_MessageSpawner".Translate().CapitalizeFirst(), ref FrameworkSettings.ShowMessageOnItemSpawn, "AV_SettingsBoxDesc_MessageSpawner".Translate().CapitalizeFirst());

            listingStandard.CheckboxLabeled("Frameworksetting: Allow fluoids to spawn items in caravans", ref FrameworkSettings.AllowCaravanItemSpawn, "Allow Fluoid-mechs to spawn items while in a caravan");

            listingStandard.CheckboxLabeled("Frameworksetting: " + "AV_SettingsBoxLabel_MessageBiocodeable".Translate().CapitalizeFirst(), ref FrameworkSettings.MessageOnBiocodableCreation, "AV_SettingsBoxDesc_MessageBiocodeable".Translate().CapitalizeFirst());

            listingStandard.GapLine();
            labelRect.y = listingStandard.CurHeight;
            DrawCenteredLabel(listingStandard, labelRect, "AV_SettingsLabel_hacking".Translate().CapitalizeFirst());
            listingStandard.GapLine();

            label = "AV_SettingsSliderLabel_HackingMaxBandwidth".Translate().CapitalizeFirst() + ": " + MechtechSettings.HackingLanceBandwidth.ToString();
            MechtechSettings.HackingLanceBandwidth = (int)listingStandard.SliderLabeled(label, MechtechSettings.HackingLanceBandwidth, 1f, 5f, 0.5f, "AV_SettingsSliderDesc_HackingMaxBandwidth".Translate().CapitalizeFirst());

            listingStandard.GapLine();
            labelRect.y = listingStandard.CurHeight;
            DrawCenteredLabel(listingStandard, labelRect, "AV_SettingsLabel_difficulty".Translate().CapitalizeFirst());
            listingStandard.GapLine();

            label = "AV_SettingsSliderLabel_NeuroferiumDeath".Translate().CapitalizeFirst() + ": " + (MechtechSettings.NeuroferiumInstantDeathChance * 100).ToString() + "%";
            float neuroferiumInstantDeathChance = listingStandard.SliderLabeled(label, MechtechSettings.NeuroferiumInstantDeathChance, 0f, 0.1f, 0.5f, "AV_SettingsSliderDesc_MechDrop_NeuroferiumDeath".Translate().CapitalizeFirst());
            MechtechSettings.NeuroferiumInstantDeathChance = (float)Math.Round(neuroferiumInstantDeathChance, 2);
            
            listingStandard.CheckboxLabeled("Anomaly DLC: " + "AV_SettingsBoxLabel_SinistreAllowedWithoutMonolith".Translate().CapitalizeFirst(), ref MechtechSettings.AllowSinistreArrivalWithoutMonolith, "AV_SettingsBoxDesc_SinistreAllowedWithoutMonolith".Translate().CapitalizeFirst());

            listingStandard.CheckboxLabeled("Allow tarantula mech to fire missiles when not drafted", ref MechtechSettings.AllowTarantulaPlayerAIMissiles, "Turn this off if you have mods which might interfere with abilities. Hopefully this helps.");


            
        }

        public void DrawDEVSettings(Listing_Standard listingStandard, Rect inRect)
        {
            listingStandard.CheckboxLabeled("Devmode: Show devtools and debug logging", ref MechtechSettings.DebugLogging, "Activate devtools and debug logging");
            listingStandard.CheckboxLabeled("Devmode: Debug on safegame loading", ref MechtechSettings.DebugOnLoad, "Lets time based thingys trigger directly when they spawn");
            listingStandard.CheckboxLabeled("EXPERIMENTAL: Allow carbonpanels as desired clothing for ideologies", ref MechtechSettings.CarbonPanelsForIdeo, "Experimental, only for my own testing, use on your own risk. Base game usually does not allow non-stuffed apparel to be used for ideo.");
        }

        public void ListingResetButton(Rect reset_rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(reset_rect);

            if (listingStandard.ButtonText("AV_SettingsButtonLabel_reset".Translate().CapitalizeFirst()))
            {
                ResetSettings();
            }
            listingStandard.End();
        }


        public void ResetSettings()
        {
            MechtechSettings.HackingLanceBandwidth = MechtechSettings.DefaultHackingLanceBandwidth;

            MechtechSettings.NeuroferiumInstantDeathChance = MechtechSettings.DefaultNeuroferiumInstantDeathChance;

            MechtechSettings.DebugLogging = MechtechSettings.DefaultDebugLogging;
            MechtechSettings.DebugOnLoad = MechtechSettings.DefaultDebugOnLoad;
        }

        // Gets called on closing the settings and actually applies + saves the settings
        public override void WriteSettings()
        {
            FrameworkSettingsUtility.ForceSaveSettings();

            base.WriteSettings();
        }

        // Override SettingsCategory to show up in the list of settings.
        public override string SettingsCategory()
        {
            return "[AV] Mechtech";
        }
    }

}
