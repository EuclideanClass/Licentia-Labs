using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using rjw;

namespace LicentiaLabs.Hediffs
{
	/* public class HediffGiver_Genderbend : HediffGiver
	{
		public float severity;

		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			if (!(cause.Severity < severity))
				{
				var newSize = 0;
				var genitals = Genital_Helper.get_genitals(pawn);

				var chest = Genital_Helper.get_breasts(pawn);
				var newBreastSize = 0;

				if (BreastSize_Helper.TryGetBreastSize(pawn, out var oldBreastSize, out var oldBreast))
				{
					if (oldBreastSize > 0)
					{
						newBreastSize = 0;
					}
					else
					{
						newBreastSize = 2;
					}

					var newBreast = BreastSize_Helper.GetHediffDef(newBreastSize);

					GenderHelper.ChangeSex(pawn, () =>
					{
						if (oldBreast != null)
						{
							pawn.health.RemoveHediff(oldBreast);
						}
						pawn.health.AddHediff(newBreast, chest);
					});

					var message = "LL_BreastsSwapped".Translate(pawn);
					Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);

				}

				if (LicentiaHelper.TryGetGenitalSpectrumSize(pawn, out var oldSize, out var oldGenitals))
				{
					if (oldSize >= 5)
					{
						newSize = oldSize - 5;
					}
					else
					{
						newSize = oldSize + 5;
					}

					var newGenitals = LicentiaHelper.GetGenitalHediffDef(newSize);

					GenderHelper.ChangeSex(pawn, () =>
					{
						if (oldGenitals != null)
						{
							pawn.health.RemoveHediff(oldGenitals);
						}
						pawn.health.AddHediff(newGenitals, genitals);
					});

					var message = "LL_GenitalsSwapped".Translate(pawn);
					Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);

				}
				else if (LicentiaHelper.TryGetOtherGenitaliaIndex(pawn, out oldSize, out oldGenitals))
				{
					if (Genital_Helper.has_penis(pawn))
					{
						newSize = oldSize + 1;
					}
					else
					{
						newSize = oldSize - 1;
					}

					var newGenitals = LicentiaHelper.GetGenitalHediffDef(newSize);

					GenderHelper.ChangeSex(pawn, () =>
					{
						if (oldGenitals != null)
						{
							pawn.health.RemoveHediff(oldGenitals);
						}
						pawn.health.AddHediff(newGenitals, genitals);
					});

					var message = "LL_GenitalsSwapped".Translate(pawn);
					Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
				}
				else
				{
					Messages.Message("LL_GenitalsNotAlterable".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);

					return;
				}
			}

			return;

		}
	}
	public class HediffGiver_RemoveThis : HediffGiver
	{
		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			pawn.health.RemoveHediff(cause);
			return;
		}
	} */
}
