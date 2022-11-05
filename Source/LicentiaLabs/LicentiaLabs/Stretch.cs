using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace LicentiaLabs
{
    public static class StretchHelper
	{
		public static void Stretch(SexProps props)
		{
			if (!LicentiaHelper.GetPenetrator(props, out Pawn penetrator, out Pawn recipient))
			{
				// If there is not penetrator involved, then there will be no inflation.
				return;
			}

			if (props.pawn != recipient)
			{
				// RJW calls SatisfyPersonal 2 times with props.pawn pointing to different participant.
				return;
			}

			if (IsElasticised(recipient))
			{
				// Elasticised recipients cannot be stretched.
				return;
			}

			// Collect some information about the act beforehand, namely the dimensions of the hediffs being
			// used for penetrating and for being penetrated.
			// Collect penetrator dimensions.
			var adjustedPenetratorDimensions = GetAdjustedHediffDimensions(penetrator, LicentiaHelper.TryGetPawnPenisHediff(penetrator));
			if (adjustedPenetratorDimensions.EnumerableNullOrEmpty())
            {
				return;
            }

			// Collect orifice dimensions.
			var pawnOrifices = new List<Hediff>();
			if (props.sexType == xxx.rjwSextype.DoublePenetration || props.sexType == xxx.rjwSextype.Vaginal)
            {
				pawnOrifices.Add(LicentiaHelper.TryGetPawnVaginaHediff(recipient));
			}
			if (props.sexType == xxx.rjwSextype.DoublePenetration || props.sexType == xxx.rjwSextype.Anal)
			{
				pawnOrifices.Add(LicentiaHelper.TryGetPawnAnusHediff(recipient));
			}
			if (pawnOrifices.EnumerableNullOrEmpty())
			{
				return;
            }
			var adjustedOrificeDimensions = GetAdjustedHediffDimensions(recipient, pawnOrifices, float.MaxValue / 2);
			if (adjustedOrificeDimensions.EnumerableNullOrEmpty())
			{
				return;
			}

			// Map penetrators (not their dimensions, just the penetrator Hediffs themselves) to orifices.
			var targetMappings = MapOrificesToPenetrators(adjustedPenetratorDimensions.Keys, pawnOrifices);

			// Determine what the new severity increase should be.
			foreach (var mapping in targetMappings)
            {
				var orifice = mapping.Key;
				if (LicentiaHelper.IsArtificial(orifice))
				{
					// Artificial orifices cannot be stretched.
					continue;
                }

				// Collect the penetrator dimensions.
				var penetrators = mapping.Value;
				var penetratorDimensions = CalculateTotalPenetratorDimensions(penetrators, adjustedPenetratorDimensions);
				var orificeDimensions = adjustedOrificeDimensions.TryGetValue(orifice);

				var isStretched = DoesStretchingOccur(penetratorDimensions, orificeDimensions, props.isRape, out var isPermanentlyStretched);
				if (!isStretched.Item1 && !isStretched.Item2)
				{
					// Orifice is larger than penetrators in all dimensions.
					continue;
				}

				// Find the larger of the permanent severities for the orifice to add to the real
				// orifice severity.
				var lengthPermSeverity = StretchOrifice(recipient, isPermanentlyStretched.Item1, isStretched.Item1, orifice, penetratorDimensions.Item1, orificeDimensions.Item1, out float lengthDamage);
				var girthPermSeverity = StretchOrifice(recipient, isPermanentlyStretched.Item2, isStretched.Item2, orifice, penetratorDimensions.Item2, orificeDimensions.Item2, out float girthDamage);
				var greaterPermSeverity = Math.Max(lengthPermSeverity, girthPermSeverity);
				if (greaterPermSeverity > 0f)
				{
					orifice.Severity += greaterPermSeverity;
				}

				GiveStretchThoughts(recipient);

				if (DamageHelper.IsPawnInvulnerable(recipient) || orifice.Part == null)
				{
					continue;
				}

				if (Settings.IsDamageEnabled)
                {
					if (lengthDamage > 0f)
					{
						if (Settings.IsDamageEnabled)
						{
							if (orifice.def.defName.Contains("Vagina"))
							{
								DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.CervixBruise, lengthDamage);
							}
							else
							{
								DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.StretchTear, lengthDamage);
							}
						}
					}
					if (girthDamage > 0f)
					{
						DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.StretchTear, girthDamage);
					}
				}

				float higherDamage = Math.Max(lengthDamage, girthDamage);
				if (Settings.IsProlapseEnabled && higherDamage > 0f)
				{
					// When length damage is below 1.5f, apply a regular prolapse. If over 1.5f, apply an extreme
					// prolapse. Severity will scale linearly from 0f to 1f for regular prolapse severity, while
					// extreme prolapse severity will be fixed to 1.0f.
					if (higherDamage <= 1.5f)
					{
						DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.Prolapse, higherDamage * 2f / 3f);
					}
					else if (higherDamage <= 2.5f)
                    {
						// To lessen the number of extreme prolapses, the threshold for damage can go up to 2.5f
						// but the receipient Pawn will receive a regular prolapse instead at max severity.
						DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.Prolapse, 1f);
					}
					else
					{
						DamageHelper.ApplyDamage(recipient, orifice.Part, Licentia.HediffDefs.ExtremeProlapse, 1f);
					}
				}
			}
		}

		/// <summary>
		/// Checks to see if the given <c>Pawn</c> is a valid stretch target.
		/// </summary>
		/// <param name="stretchee">the <c>Pawn</c> to be checked</param>
		/// <returns><c>true</c> if the <paramref name="stretchee"/> has used an elasticise serum recently,
		/// <c>false</c> otherwise.</returns>
		public static bool IsElasticised(Pawn stretchee)
		{
			return stretchee?.health.hediffSet.GetFirstHediffOfDef(Licentia.HediffDefs.Elasticised) != null;
		}

		/// <summary>
		/// Calculates the lengths and girths of each genital <c>Hediff</c> specified in the <paramref name="hediffs"/>
		/// list. It is assumed that each <c>Hediff</c> belongs to the provided <paramref name="pawn"/>.
		/// If lengths and girths are not available, then <c>Hediff</c> severity will be used as the
		/// substitute.
		/// </summary>
		/// <param name="pawn">the <c>Pawn</c> that is assumed to own all of the <c>Hediff</c>s</param>
		/// <param name="hediffs">the <c>Hediff</c>s containing the <c>Hediff</c>s to be calculated</param>
		/// <param name="defaultValue">the value to be used if no dimension value could be obtained</param>
		/// <returns>A Dictionary of <c>Hediff</c>s to tuples of the <c>Hediff</c>s' lengths and girths in
		/// that order.</returns>
		public static Dictionary<Hediff, (float, float)> GetAdjustedHediffDimensions(Pawn pawn, IEnumerable<Hediff> hediffs, float defaultValue = float.MinValue)
        {
			Dictionary<Hediff, (float, float)> hediffDimensions = new Dictionary<Hediff, (float, float)>();

			// Theoretically, a Pawn could have both enlargement and diminishment effects active at the same time.
			var adjustmentValue = 0f;
			adjustmentValue += pawn?.health.hediffSet.GetFirstHediffOfDef(Licentia.HediffDefs.Enlarged) != null ? 0.2f : 0f;
			adjustmentValue += pawn?.health.hediffSet.GetFirstHediffOfDef(Licentia.HediffDefs.Diminished) != null ? -0.2f : 0f;

			// Calculate the length and girth of each Hediff.
			foreach (var hediff in hediffs)
			{
				// Some HediffComps have varying stretch amounts.
				var stretchVariance = hediff.def.CompProps<CompProperties_LicentiaPart>()?.stretchVariance;
				if (stretchVariance.HasValue)
				{
					adjustmentValue += Rand.Range(0, (float)stretchVariance) / 2f;
				}

				// Try to use length and girth if at all possible, and use Severity only as a fallback.
				CompHediffBodyPart rjwHediff = hediff.TryGetComp<CompHediffBodyPart>();
				if (rjwHediff != null)
                {
                    if (!PartSizeExtension.TryGetLength(hediff, out float length)) length = defaultValue == float.MinValue ? hediff.Severity : defaultValue;
                    length += adjustmentValue;
					if (!PartSizeExtension.TryGetGirth(hediff, out float girth)) girth = defaultValue == float.MinValue ? hediff.Severity : defaultValue;
					girth += adjustmentValue;
					if (length < 0 || girth < 0)
                    {
						continue;
                    }
					hediffDimensions.Add(hediff, (length, girth));
                }
				else
				{
					if (hediff.Severity < 0)
                    {
						continue;
                    }
					hediffDimensions.Add(hediff, (hediff.Severity, hediff.Severity));
				}
			}

			return hediffDimensions;
		}

		/// <summary>
		/// Tries to pair penetrators with orifices. If there are more penetrators than there are orifices,
		/// then the targets will be doubled up and later may have a chance of multiple penetrators targeting
		/// the same orifice, increasing effective girth when calculated for stretching.
		/// </summary>
		/// <param name="penetrators">the penetrator <c>Hediff</c>s</param>
		/// <param name="orifices">the orifice <c>Hediff</c>s; count must be greater than 0</param>
		/// <returns>A Dictionary mapping orifice <c>Hediff</c>s to one or more penetrator <c>Hediff</c>s</returns>
		public static Dictionary<Hediff, List<Hediff>> MapOrificesToPenetrators(IEnumerable<Hediff> penetrators, IList<Hediff> orifices)
		{
			var penetratorList = penetrators.ToList();
			penetratorList.Shuffle();

			var output = new Dictionary<Hediff, List<Hediff>>();
			var nOrifices = orifices.Count();
			for (int ii = 0; ii < penetratorList.Count(); ++ii)
			{
				var orifice = orifices[ii % nOrifices];
                if (!output.TryGetValue(orifice, out List<Hediff> mappedPenetrators))
                {
                    mappedPenetrators = new List<Hediff>();
                    output.Add(orifice, mappedPenetrators);
				}
				var penetrator = penetratorList[ii];
				mappedPenetrators.Add(penetrator);

				if (ii == nOrifices - 1 && !Settings.IsAllowMultiplePenetratorsPerOrifice)
                {
					break;
                }
			}
			return output;
		}

		/// <summary>
		/// Determines what the final total penetrator dimensions will be. The maximum penetrator length
		/// and some form of summation of girths will be used, meaning that the overall penetrator dimensions
		/// will be that of some aggregate penetrator roughly resembling all of the penetrators used.
		/// </summary>
		/// <param name="penetrators">the <c>Hediff</c>s performing the penetrating</param>
		/// <param name="penetratorDimensions">the Dictionary that will serve as a lookup for dimensions
		/// of each of the <paramref name="penetrators"/></param>
		/// <returns>A tuple of the aggregate penetrator's length and girth in that order.</returns>
		public static (float, float) CalculateTotalPenetratorDimensions(IEnumerable<Hediff> penetrators, Dictionary<Hediff, (float, float)> penetratorDimensions)
		{
			var length = 0f;
			var totalGirth = 0f;
			foreach (var penetrator in penetrators)
			{
				var dimensions = penetratorDimensions.TryGetValue(penetrator);
				if (length == 0f || totalGirth == 0f || true/*Rand.Range(0f, 1f) > 0.5f*/)
				{
					length = Math.Max(length, dimensions.Item1);
					totalGirth += dimensions.Item2;
				}
			}
			return (length, totalGirth);
		}

		/// <summary>
		/// Determines based on preset thresholds whether the orifice being penetrated should be stretched
		/// or not.
		/// </summary>
		/// <param name="penetrator">The aggregate length and girth of the penetrator</param>
		/// <param name="orifice">The length and girth of the receiving orifice</param>
		/// <param name="isRape">lowers the threshold for stretching effects from simple comparison to
		/// modifying the penetrating dimensions to 125% their actual dimensions to simulate the damaging
		/// effects of non-consensual intercourse.</param>
		/// <param name="isPermanentlyStretched">Contains whether the length or girth are valid stretching
		/// candidates</param>
		/// <returns>Whether there was any stretching at all in either length or girth directions.</returns>
		public static (bool, bool) DoesStretchingOccur((float, float) penetrator, (float, float) orifice, bool isRape, out (bool, bool) isPermanentlyStretched)
		{
			// Check for stretching length-wise.
			bool isLengthStretched = penetrator.Item1 * (isRape ? 1.25f : 1f) > orifice.Item1;
			bool isLengthStretchedPerm = Settings.IsPermanentStretchEnabled && penetrator.Item1 * (isRape ? 1.25f : 1f) > orifice.Item1 * 1.5;

			// Check for stretching girth-wise.
			bool isGirthStretched = penetrator.Item2 * (isRape ? 1.25f : 1f) > orifice.Item2;
			bool isGirthStretchedPerm = Settings.IsPermanentStretchEnabled && penetrator.Item2 * (isRape ? 1.25f : 1f) > orifice.Item2 * 1.5;

			isPermanentlyStretched = (isLengthStretchedPerm, isGirthStretchedPerm);
			return (isLengthStretched, isGirthStretched);
		}

		/// <summary>
		/// Adds a "Stretched" <c>Hediff</c> to the orifice currently being stretched. Also returns the
		/// severity increase for the orifice if permanent stretching has taken place and is enabled. One
		/// of the two parameters <paramref name="isPermanentlyStretched"/> or <paramref name="isStretched"/>
		/// must be true for this function to have any effect.
		/// </summary>
		/// <param name="penetratedPawn">the <c>Pawn</c> receiving the penetration</param>
		/// <param name="isPermanentlyStretched">if true, then permanent stretching is to be performed</param>
		/// <param name="isStretched">if true, then temporary stretching is to be performed</param>
		/// <param name="orifice">the orifice <c>Hediff</c> being stretched</param>
		/// <param name="penetratorDimension">the current dimension being checked (length or girth) for
		/// the penetrator</param>
		/// <param name="orificeDimension">the current dimension being checked (length or girth) for
		/// the receiving orifice</param>
		/// <param name="uniqueDamage">the damage to be uniquely received by the penetrated orifice; scales
		/// from 0f to 2f when the adjusted dimension ratio between penetrator and orifice was >150%.</param>
		/// <returns></returns>
		public static float StretchOrifice(Pawn penetratedPawn, bool isPermanentlyStretched, bool isStretched, Hediff orifice, float penetratorDimension, float orificeDimension, out float uniqueDamage)
		{
			float severityIncrease = 0f;
			Hediff hediff = null;
			if (isPermanentlyStretched)
			{
				// Attempt to increase the severity of the stretched orifice, as that is the only way that the
				// orifice can increase in size (and prevent stretching again).
				// TODO: Could use an algorithm for directly converting dimension differences into severity.
				var ratio = Math.Min(penetratorDimension / orificeDimension / 1.5f, 0.12f) * Rand.Range(0.85f, 1.15f); // The maximum an orifice can stretch by is 10%-14% its current severity.
				GenderHelper.ChangeSex(penetratedPawn, () =>
				{
					var stretchAmount = orifice.Severity * ratio;
					severityIncrease = LicentiaHelper.Clamp(stretchAmount, 0.05f, 0.25f); // Clamp the actual severity increase to prevent rapid changes.
				});

				// Stretching severity will scale from 25% (lasting one day) to 100% (lasting 4 days) depending
				// on how badly the orifice was stretched.
				hediff = HediffMaker.MakeHediff(Licentia.HediffDefs.Stretched, penetratedPawn);
				hediff.Severity = 0.25f + LicentiaHelper.Clamp(0.75f * (ratio / 0.12f), 0.25f, 0.75f);
				penetratedPawn.health.AddHediff(hediff, orifice.Part);

				if (!Settings.IsStretchTextPlayerFactionVisibleOnly || (penetratedPawn.Faction?.IsPlayer ?? false))
				{
					Messages.Message("LL_OrificeStretched".Translate(penetratedPawn, orifice.def.defName.ToLower()), penetratedPawn, MessageTypeDefOf.SilentInput);
				}
			}
			else if (isStretched)
			{
				// If any stretching occurred at all, there will be a minimum of 25% severity for stretching. Mild
				// stretching will last one day.
				hediff = HediffMaker.MakeHediff(Licentia.HediffDefs.Stretched, penetratedPawn);
				hediff.Severity = 0.25f;
				penetratedPawn.health.AddHediff(hediff, orifice.Part);
			}
            else
            {
				// For some reason, this function was called without any stretching taking place.
				uniqueDamage = 0f;
				return 0f;
            }

			// Stretch severity ranges between 0.25f and 1.0f, which means the severity of the damage can
			// range between 0f (regular stretching does not cause tears) and 2.0f, which is 25% of the orifice
			// HP for a >150% stretch.
			// Formula here is a linear relationship: y = 2.6666...x + 0.6666...
			uniqueDamage = Settings.DamageMultiplier * ((2f + 2f / 3f) * hediff?.Severity ?? 0f) - (2f / 3f);
			return severityIncrease;
		}

		/// <summary>
		/// Gives the <paramref name="stretchee"/> a thought about having been stretched. For <c>Pawn</c>s
		/// that enjoy pain or the act of stretching itself, a positive thought will be gained. Otherwise,
		/// the <c>Pawn</c> will gain a negative thought.
		/// </summary>
		/// <param name="stretchee">the <c>Pawn</c> that was stretched and will be receiving a thought</param>
		public static void GiveStretchThoughts(Pawn stretchee)
		{
			if (xxx.is_animal(stretchee))
			{
				return;
			}

			string pawn_quirks = CompRJW.Comp(stretchee).quirks.ToString();
			if (pawn_quirks.Contains("Impregnation fetish") ||
				pawn_quirks.Contains("Teratophile") ||
				pawn_quirks.Contains("Incubator") ||
				pawn_quirks.Contains("Breeder") ||
				xxx.is_zoophile(stretchee))
			{
				stretchee.needs.mood.thoughts.memories.TryGainMemory(Licentia.ThoughtDefs.GotStretchedKinky);
			}
			else if (xxx.is_masochist(stretchee))
			{
				stretchee.needs.mood.thoughts.memories.TryGainMemory(Licentia.ThoughtDefs.GotStretchedMasochist);
			}
			else
			{
				stretchee.needs.mood.thoughts.memories.TryGainMemory(Licentia.ThoughtDefs.GotStretched);
			}
		}
	}

	public static class DamageHelper
	{
		public static bool IsPawnInvulnerable(Pawn pawn)
		{
			return pawn?.health.hediffSet.GetFirstHediffOfDef(Licentia.HediffDefs.Untearable) != null;
		}

		public static void ApplyDamage(Pawn pawn, BodyPartRecord part, HediffDef damageType, float damage)
		{
			if (part == null)
			{
				return;
			}

			var hediff = HediffMaker.MakeHediff(damageType, pawn);
			hediff.Severity = damage;
			pawn.health.AddHediff(hediff, part);
		}
	}

	[StaticConstructorOnStartup]
	internal static class First
	{
		static First()
		{
			var har = new Harmony("LL");
			har.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[HarmonyPatch(typeof(SexUtility))]
	[HarmonyPatch("SatisfyPersonal")]
	[HarmonyPatch(new Type[] { typeof(SexProps), typeof(float) })]
	public static class StretchPatch
	{
		[HarmonyPostfix]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter is provided by RJW through Harmony.")]
		public static void SatisfyPersonal(SexProps props, float satisfaction)
		{
			if (props.isCoreLovin || !Settings.IsStretchingEnabled || props.pawn?.Dead == true || props.partner == null || props.partner.Dead == true)
            {
				return;
            }
			
			StretchHelper.Stretch(props);
		}
	}
}
