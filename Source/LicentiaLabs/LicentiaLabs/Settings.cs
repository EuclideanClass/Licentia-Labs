using UnityEngine;
using Verse;

namespace LicentiaLabs
{
	public class Settings : ModSettings
	{
		public static bool IsAllowMultiplePenetratorsPerOrifice = true;
		public static bool IsStretchingEnabled = true;
		public static bool IsPermanentStretchEnabled = true;
		public static bool IsStretchTextPlayerFactionVisibleOnly = true;

		public static bool IsCumflationEnabled = true;
		public static bool IsCumflationNutritionEnabled = true;
		public static bool IsAllowCumflationNutritionToViolateThermodynamics = false;
		public static float CumflationMultiplier = 1f;

		public static bool IsDamageEnabled = false;
		public static float DamageMultiplier = 1f;
		public static bool IsProlapseEnabled = false;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref IsAllowMultiplePenetratorsPerOrifice, "IsAllowMultiplePenetratorsPerOrifice", true);
			Scribe_Values.Look(ref IsStretchingEnabled, "IsStretchingEnabled", true);
			Scribe_Values.Look(ref IsPermanentStretchEnabled, "IsPermanentStretchEnabled", true);
			Scribe_Values.Look(ref IsStretchTextPlayerFactionVisibleOnly, "IsStretchTextPlayerFactionVisibleOnly", true);

			Scribe_Values.Look(ref IsCumflationEnabled, "IsCumflationEnabled", true);
			Scribe_Values.Look(ref IsCumflationNutritionEnabled, "IsCumflationNutritionEnabled", true);
			Scribe_Values.Look(ref IsAllowCumflationNutritionToViolateThermodynamics, "IsAllowCumflationNutritionToViolateThermodynamics", false);
			Scribe_Values.Look(ref CumflationMultiplier, "CumflationMultiplier", 1f);

			Scribe_Values.Look(ref IsDamageEnabled, "IsDamageEnabled", false);
			Scribe_Values.Look(ref DamageMultiplier, "DamageMultiplier", 1f);
			Scribe_Values.Look(ref IsProlapseEnabled, "IsProlapseEnabled", false);
			base.ExposeData();
		}

		private static readonly float height_modifier = 100f;

		public static void DoWindowContents(Rect inRect)
		{
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

            Listing_Standard listingStandard = new Listing_Standard
            {
                maxOneColumn = true,
                ColumnWidth = viewRect.width / 2.05f
            };
            listingStandard.Begin(inRect);
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("Enable multiple penetrators per orifice", ref IsAllowMultiplePenetratorsPerOrifice, "Allow orifices to have a chance of being targeted by more than one penetrator at once, if more are available.");
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("Enable stretching effects", ref IsStretchingEnabled, "Allow orifices to be stretched, which affects colonists' moods.");
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("Enable permanent stretching", ref IsPermanentStretchEnabled, "Allow orifices to be permanently stretched by large penetrators (at least 1.5x size of orifice).");
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("Stretch messages for player faction only:", ref IsStretchTextPlayerFactionVisibleOnly, "Only show stretch messages for player faction");

			listingStandard.Gap(40f);
			listingStandard.CheckboxLabeled("Enable cumflation", ref IsCumflationEnabled, "Allow colonists to be inflated by excessive cum production.");
			if (IsCumflationEnabled)
			{
				listingStandard.Gap(4f);
				listingStandard.CheckboxLabeled("Enable nutrition gain through cum", ref IsCumflationNutritionEnabled,
					"Allow pawns to gain nutrition by having other pawns cum inside them orally. The giving pawn loses nutrition while the receiving pawn gains nutrition.");
				if (IsCumflationNutritionEnabled)
				{
					listingStandard.Gap(4f);
					listingStandard.CheckboxLabeled("Enable violation of thermodynamics", ref IsAllowCumflationNutritionToViolateThermodynamics,
						"Allows pawns to receive more nutrition than the giving pawn is able to give.");
				}
				listingStandard.Gap(4f);
				listingStandard.Label("Cumflation multiplier (ranges from 0.1x to 20x, affects only cumflation effects): " + CumflationMultiplier);
				CumflationMultiplier = listingStandard.Slider(CumflationMultiplier, 0.1f, 20f);
			}

			listingStandard.Gap(40f);
			listingStandard.CheckboxLabeled("Enable orifice damage", ref IsDamageEnabled, "Allow orifices to be damaged by overly large penetrations.");
			listingStandard.Gap(4f);
			listingStandard.Label("Damage multiplier (ranges from 0.1x to 2x): " + DamageMultiplier);
			DamageMultiplier = listingStandard.Slider(DamageMultiplier, 0.1f, 2f);
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("Enable orifice prolapsing", ref IsProlapseEnabled, "Allow orifices to be prolapsed, which, in extreme cases, may require surgery.");
			listingStandard.End();
		}
	}

	public class LicentiaLabs : Mod
	{
		private readonly Settings settings;

		public LicentiaLabs(ModContentPack content) : base(content)
		{
			settings = GetSettings<Settings>();
		}

		public override string SettingsCategory()
		{
			return "RJW Licentia Labs";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Settings.DoWindowContents(inRect);
		}
	}
}
