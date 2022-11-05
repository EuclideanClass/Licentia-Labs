using HarmonyLib;
using RimWorld;
using rjw;
using System;
using Verse;

namespace LicentiaLabs
{
    public class CumflationHelper
	{
		public static void Cumflation(SexProps props)
		{
			if (!LicentiaHelper.GetPenetrator(props, out Pawn inflator, out Pawn inflated))
			{
				// If there is not penetrator involved, then there will be no inflation.
				return;
			}

			if (props.pawn != inflator)
			{
				// RJW calls SatisfyPersonal 2 times with props.pawn pointing to a different participant.
				return;
			}

			if (xxx.is_animal(inflated))
			{
				// Animals cannot get inflated.
				return;
			}

			Hediff cumflationHediff;
			switch (props.sexType)
			{
				case xxx.rjwSextype.Oral:
				case xxx.rjwSextype.Fellatio:
					cumflationHediff = GetCumflationHediff(inflated, Licentia.HediffDefs.Cumstuffed, "Stomach");
					break;
				case xxx.rjwSextype.Vaginal:
				case xxx.rjwSextype.Anal:
				case xxx.rjwSextype.DoublePenetration:
					cumflationHediff = GetCumflationHediff(inflated, Licentia.HediffDefs.Cumflation);
					break;
				default:
					return;
			}

			var holeSize = inflated.BodySize;
			float cumAmount = CalculateCumAmount(inflator);
			var severity = AdjustCumAmount(cumAmount, holeSize);

			float consumed = AdjustCumAmount(Math.Max(1f - cumflationHediff.Severity, severity), holeSize, true);
			float overflow = cumflationHediff.Severity + severity - 1f;
			cumflationHediff.Severity += severity;
			switch (props.sexType)
			{
				case xxx.rjwSextype.Oral:
				case xxx.rjwSextype.Fellatio:
					TransferNutrition(inflator, inflated, consumed);
					inflated.records.AddTo(Licentia.RecordDefs.AmountOfCumstuffed, consumed);
					break;
				case xxx.rjwSextype.Vaginal:
				case xxx.rjwSextype.Anal:
				case xxx.rjwSextype.DoublePenetration:
					cumflationHediff = GetCumflationHediff(inflated, Licentia.HediffDefs.Cumflation);
					inflated.records.AddTo(Licentia.RecordDefs.AmountOfCumflation, consumed);
					break;
				default:
					return;
			}

			if (cumflationHediff.Severity > 0.6)
			{
				GiveCumflationThoughts(inflated);
			}

			if (overflow > 0f)
			{
				GiveOverinflationThoughts(inflated, props.sexType);
				var secondaryOverflow = 0f;
				var tertiaryOverflow = 0f;
				switch (props.sexType)
				{
					case xxx.rjwSextype.Oral:
					case xxx.rjwSextype.Fellatio:
						secondaryOverflow = overflow;
						break;
					case xxx.rjwSextype.DoublePenetration:
						overflow /= 2f;
						tertiaryOverflow = overflow;
						goto case xxx.rjwSextype.Anal;
					case xxx.rjwSextype.Anal:
						var overflowHediff = GetCumflationHediff(inflated, Licentia.HediffDefs.Cumstuffed, "Stomach");
						float secondaryConsumed = AdjustCumAmount(Math.Max(1f - overflowHediff.Severity, severity), holeSize, true);
						TransferNutrition(inflator, inflated, secondaryConsumed);
						inflated.records.AddTo(Licentia.RecordDefs.AmountOfCumstuffed, secondaryConsumed);
						secondaryOverflow = overflowHediff.Severity + overflow - 1f;
						overflowHediff.Severity += overflow;
						break;
					case xxx.rjwSextype.Vaginal:
						tertiaryOverflow = overflow;
						break;
					default:
						break;
				}

				if (secondaryOverflow > 0f)
                {
					var vomitJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VomitCum"), inflated);
					((JobDriver_VomitCum)vomitJob.GetCachedDriver(inflated)).sourceName = inflator.LabelIndefinite();
					inflated.jobs.jobQueue.EnqueueFirst(vomitJob);
					FilthMaker.TryMakeFilth(inflated.PositionHeld, inflated.MapHeld, Licentia.ThingDefs.FilthCum, inflator.LabelIndefinite(), (int)Math.Max(secondaryOverflow / 5, 10.0f));
				}
				if (tertiaryOverflow > 0f)
				{
					FilthMaker.TryMakeFilth(inflated.PositionHeld, inflated.MapHeld, Licentia.ThingDefs.FilthCum, inflator.LabelIndefinite(), (int)Math.Max(tertiaryOverflow / 5, 10.0f));
				}
			}
		}

		public static Hediff GetCumflationHediff(Pawn inflated, HediffDef hediffDef, string bodyPartRecordName = null)
		{
			BodyPartRecord bodyPartRecord = null;
			if (bodyPartRecordName != null)
            {
				bodyPartRecord = inflated.RaceProps.body.AllParts.Find(bpr => bpr.def.defName.Contains(bodyPartRecordName) || bpr.def.defName.Contains(bodyPartRecordName.ToLower()));
				if (bodyPartRecord == null)
                {
					return null;
				}
			}

			Hediff cumflationHediff = inflated.health.hediffSet.GetFirstHediffOfDef(hediffDef);
			if (cumflationHediff == null)
			{
				cumflationHediff = HediffMaker.MakeHediff(hediffDef, inflated, bodyPartRecord);
				cumflationHediff.Severity = 0;
				inflated.health.AddHediff(cumflationHediff, bodyPartRecord);
			}
			return cumflationHediff;
		}

		public static float CalculateCumAmount(Pawn giver)
		{
			float cumAmount = 0f;
			var hediffs = LicentiaHelper.TryGetPawnPenisHediff(giver);
			foreach (var hediff in hediffs)
			{
				CompHediffBodyPart rjwHediff = hediff.TryGetComp<CompHediffBodyPart>();
				if (rjwHediff != null)
				{
					cumAmount += rjwHediff.FluidAmmount * rjwHediff.FluidModifier;
				}
				else
				{
					cumAmount += hediff.Severity * giver.BodySize;
				}
			}

			if (cumAmount == 0)
			{
				cumAmount = giver.BodySize; //fallback for mechanoinds and w/e without hediffs
			}
			// Horniness will scale resulting output somewhere from 50% (0 horniness) to 100% (max horniness).
			Need sexNeed = giver?.needs?.AllNeeds?.Find(x => string.Equals(x.def.defName, "Sex"));
			float horniness = sexNeed == null ? 1f : 1f - 0.5f * sexNeed.CurLevel;
			float ageScale = Math.Min(80 / SexUtility.ScaleToHumanAge(giver), 1.0f); //calculation lifted from rjw
			return Settings.CumflationMultiplier * cumAmount * horniness * ageScale;
		}

		public static float AdjustCumAmount(float cumAmount, float holeSize, bool invert = false)
		{
			if (holeSize == 0)
			{
				holeSize = 1f; // we don't want to devide or multiply by zero
			}

			if (!invert)
			{
				float severity = cumAmount / holeSize;
				severity /= 100f; // severity is 0.0-1.0
				return severity;
			}

			float output = cumAmount * 100f; // invert the severity into cumAmount
			output *= holeSize;
			return output;
		}

		public static void TransferNutrition(Pawn giver, Pawn receiver, float cumAmount)
		{
			if (!Settings.IsCumflationNutritionEnabled)
            {
				return;
            }

			float nutrition = CalculateNutritionAmount(giver, cumAmount);
			Need_Food inflatorFood = giver.needs.TryGetNeed<Need_Food>();
			Need_Food inflatedFood = receiver.needs.TryGetNeed<Need_Food>();
			if (inflatorFood != null && inflatedFood != null)
			{
				inflatorFood.CurLevel -= nutrition;
				inflatedFood.CurLevel += nutrition;
			}

			// It is assumed that cum is less filling for thirst than for hunger (though really, the value of either is debatable).
			if (xxx.DubsBadHygieneIsActive)
			{
				NeedDef inflatedThirstDef = receiver.needs.AllNeeds.Find(x => x.def == xxx.DBHThirst)?.def;
				if (inflatedThirstDef != null)
				{
					receiver.needs.TryGetNeed(inflatedThirstDef).CurLevel += nutrition / 2;
				}
            }
		}

		public static float CalculateNutritionAmount(Pawn giver, float cumAmount)
		{
			Need_Food need = giver?.needs?.TryGetNeed<Need_Food>();
			if (need == null)
			{
				return 0f;
			}

			// Every unit of cum can be treated as 0.005 nutrition. That is, 200 cum units would fill an average pawn with a 1.0 nutrition cap.
			var nutritionAmount = cumAmount * 0.005f;
			return Settings.IsAllowCumflationNutritionToViolateThermodynamics ? nutritionAmount : Math.Min(nutritionAmount, need.CurLevel);
		}

		public static bool LikesCumflation(Pawn inflated)
		{
			bool likesCumflation = inflated?.story?.traits?.HasTrait(Licentia.TraitDefs.likesCumflation) ?? false;
			if (likesCumflation)
			{
				return likesCumflation;
			}

			string pawn_quirks = CompRJW.Comp(inflated).quirks.ToString();
			if (pawn_quirks.Contains("Impregnation fetish") ||
				pawn_quirks.Contains("Teratophile") ||
				pawn_quirks.Contains("Incubator") ||
				pawn_quirks.Contains("Breeder") ||
				pawn_quirks.Contains("Messy") ||
				xxx.is_zoophile(inflated))
			{
				return true;
			}
			return false;
		}

		public static void GiveOverinflationThoughts(Pawn inflated, xxx.rjwSextype sextype)
		{
			bool likesCumflation = LikesCumflation(inflated);

			switch (sextype)
			{
				case xxx.rjwSextype.Oral:
				case xxx.rjwSextype.Fellatio:
					if (likesCumflation)
					{
						inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(Licentia.ThoughtDefs.GotOverCumstuffedEnjoyed);
						return;
					}
					inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(Licentia.ThoughtDefs.GotOverCumstuffed);
					return;
				case xxx.rjwSextype.Vaginal:
				case xxx.rjwSextype.Anal:
				case xxx.rjwSextype.DoublePenetration:
					if (likesCumflation)
					{
						inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(Licentia.ThoughtDefs.GotOverCumflatedEnjoyed);
						return;
					}
					inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(Licentia.ThoughtDefs.GotOverCumflated);
					return;
				default:
					return;
			}
		}

		public static void GiveCumflationThoughts(Pawn inflated)
		{
			if (!LikesCumflation(inflated))
			{
				return;
			}

			inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(Licentia.ThoughtDefs.GotInflatedKinky);
		}
	}

	[HarmonyPatch(typeof(SexUtility))]
	[HarmonyPatch("TransferNutrition")]
	[HarmonyPatch(new Type[] { typeof(SexProps) })]
	public static class CumflationPatch
	{
		[HarmonyPrefix]
		public static bool TransferNutrition(SexProps props)
		{
			if (!props.pawn.Dead == true && !props.partner.Dead == true && Settings.IsCumflationEnabled)
			{
				CumflationHelper.Cumflation(props);
				SexUtility.TransferNutritionSucc(props);
			}
			else
            {
				return true;
            }

			return false;
		}
	}
}
