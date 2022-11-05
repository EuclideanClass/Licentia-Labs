using RimWorld;
using Verse;
using rjw;

namespace LicentiaLabs
{
	public class Comp_BreastIncrease : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_breastsBPR(pawn);
			var Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!Parts.NullOrEmpty())
			{
				foreach (Hediff hed in Parts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					GenderHelper.ChangeSex(pawn, () =>
					{
						hed.Severity += 0.1f;
					});
				}
				Messages.Message("LL_BreastsSizeIncrease".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_BreastDecrease : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_breastsBPR(pawn);
			var Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!Parts.NullOrEmpty())
			{
				foreach (Hediff hed in Parts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					GenderHelper.ChangeSex(pawn, () =>
					{
						if (hed.Severity <= 0.11f)
						{
							hed.Severity = 0.01f;
						}
						else
						{
							hed.Severity -= 0.1f;
						}
					});
				}
				Messages.Message("LL_BreastsSizeDecrease".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_GenitalIncrease : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_genitalsBPR(pawn);
			var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!oldParts.NullOrEmpty())
			{
				foreach (Hediff hed in oldParts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					if (hed.def.defName.ToLower().Contains("penis") ||
						hed.def.defName.ToLower().Contains("ovipositorm") ||
						hed.def.defName.ToLower().Contains("tentacle")
						)
					{
						hed.Severity += 0.1f;
					}

					if (hed.def.defName.ToLower().Contains("vagina") ||
						hed.def.defName.ToLower().Contains("ovipositorf")
						)
					{
						if (hed.Severity <= 0.11f)
						{
							hed.Severity = 0.01f;
						}
						else
						{
							hed.Severity -= 0.1f;
						}
					}
				}
				Messages.Message("LL_GenitalsAltered".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_GenitalDecrease : ThingComp
	{
		public override void PostIngested(Pawn pawn) {

			var partBPR = Genital_Helper.get_genitalsBPR(pawn);
			var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!oldParts.NullOrEmpty())
			{
				foreach (Hediff hed in oldParts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					if (hed.def.defName.ToLower().Contains("penis") ||
						hed.def.defName.ToLower().Contains("ovipositorm") ||
						hed.def.defName.ToLower().Contains("tentacle")
						)
					{
						if (hed.Severity <= 0.11f)
						{
							hed.Severity = 0.01f;
						}
						else
						{
							hed.Severity -= 0.1f;
						}
					}

					if (hed.def.defName.ToLower().Contains("vagina") ||
						hed.def.defName.ToLower().Contains("ovipositorf")
						)
					{
						hed.Severity += 0.1f;
					}
				}
				Messages.Message("LL_GenitalsAltered".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_CumIncrease : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_genitalsBPR(pawn);
			var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!oldParts.NullOrEmpty())
			{
				Hediff hd;
				CompHediffBodyPart CompHediff;

				foreach (Hediff hed in oldParts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					hd = null;
					CompHediff = null;
					if (hed.def.defName.ToLower().Contains("penis") ||
						hed.def.defName.ToLower().Contains("ovipositorm") ||
						hed.def.defName.ToLower().Contains("tentacle")
						)
					{
						hd = hed;
					}

					if (hed.def.defName.ToLower().Contains("vagina") ||
						hed.def.defName.ToLower().Contains("ovipositorf")
						)
					{
						hd = hed;
					}

					if (hd != null)
					{
						CompHediff = hed.TryGetComp<rjw.CompHediffBodyPart>();
						if (CompHediff != null)
						{
							CompHediff.FluidAmmount *= 1.1f;
						}
					}
				}
				Messages.Message("LL_CumAltered".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_CumDecrease : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_genitalsBPR(pawn);
			var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!oldParts.NullOrEmpty())
			{
				Hediff hd;
				CompHediffBodyPart CompHediff;

				foreach (Hediff hed in oldParts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					hd = null;
					CompHediff = null;
					if (hed.def.defName.ToLower().Contains("penis") ||
						hed.def.defName.ToLower().Contains("ovipositorm") ||
						hed.def.defName.ToLower().Contains("tentacle")
						)
					{
						hd = hed;
					}

					if (hed.def.defName.ToLower().Contains("vagina") ||
						hed.def.defName.ToLower().Contains("ovipositorf")
						)
					{
						hd = hed;
					}

					if (hd != null)
					{
						CompHediff = hed.TryGetComp<rjw.CompHediffBodyPart>();
						if (CompHediff != null)
						{
							CompHediff.FluidAmmount *= 0.9f;
						}
					}
				}
				Messages.Message("LL_CumAltered".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_AnalTighteningSerum : ThingComp
	{

		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_anusBPR(pawn);
			var Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

			if (!Parts.NullOrEmpty())
			{
				foreach (Hediff hed in Parts)
				{
					if (LicentiaHelper.IsArtificial(hed))
						continue;

					if (hed.Severity <= 0.11f)
					{
						hed.Severity = 0.01f;
					}
					else
					{
						hed.Severity -= 0.1f;
					}
				}
				var message = "LL_AnusAltered".Translate(pawn);
				Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
			}
		}
	}

	public class Comp_Genderbend : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			if (GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.futa) //if futa
			{
				Messages.Message("LL_NoEffectOnFuta".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
				return;
			}

			GenderHelper.ChangeSex(pawn, () =>
			{
				var partBPR = Genital_Helper.get_genitalsBPR(pawn);
				var Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

				if (!Genital_Helper.has_penis_fertile(pawn, Parts) && !Genital_Helper.has_penis_infertile(pawn, Parts))
				{
					LicentiaHelper.AddSexPart(pawn, SexPartType.MaleGenital);
				}
				else
				{
					if (!Parts.NullOrEmpty())
					{
						foreach (Hediff hed in Parts)
						{
							if (LicentiaHelper.IsArtificial(hed))
								continue;

							if (hed.def.defName.ToLower().Contains("penis") ||
								hed.def.defName.ToLower().Contains("ovipositorm") ||
								hed.def.defName.ToLower().Contains("tentacle")
								)
							{
								pawn.health.RemoveHediff(hed);
							}
						}
					}
				}

				if (!Genital_Helper.has_vagina(pawn, Parts))
				{
					LicentiaHelper.AddSexPart(pawn, SexPartType.FemaleGenital);
				} 
				else
				{
					if (!Parts.NullOrEmpty())
					{
						foreach (Hediff hed in Parts)
						{
							if (LicentiaHelper.IsArtificial(hed))
								continue;

								if (hed.def.defName.ToLower().Contains("vagina") ||
									hed.def.defName.ToLower().Contains("ovipositorf")
									)
								{
								pawn.health.RemoveHediff(hed);
							}
						}
					}
				}
			});

			Messages.Message("LL_GenitalsSwapped".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
		}
	}

	public class Comp_FutaSerum : ThingComp
	{
		public override void PostIngested(Pawn pawn)
		{
			if (GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.futa) //if futa
			{
				Messages.Message("LL_NoEffectOnFuta".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
				return;
			}

			GenderHelper.ChangeSex(pawn, () =>
			{
				var partBPR = Genital_Helper.get_genitalsBPR(pawn);
				var Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

				if (!Genital_Helper.has_penis_fertile(pawn, Parts) && !Genital_Helper.has_penis_infertile(pawn, Parts)) {
					LicentiaHelper.AddSexPart(pawn, SexPartType.MaleGenital);
				}

				if (!Genital_Helper.has_vagina(pawn, Parts))
				{
					LicentiaHelper.AddSexPart(pawn, SexPartType.FemaleGenital);
				}

				partBPR = Genital_Helper.get_breastsBPR(pawn);
				Parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);
				if (Parts.NullOrEmpty() || (!Genital_Helper.has_breasts(pawn, Parts) && !Parts.Any((Hediff hed) => hed.def.defName.ToLower().Contains("featureless"))))
				{
					LicentiaHelper.AddSexPart(pawn, SexPartType.FemaleBreast);
				}
			});

			Messages.Message("LL_MadeFuta".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
		}
	}

	public class CompProperties_BreastIncrease : CompProperties
	{
		public CompProperties_BreastIncrease()
		{
			compClass = typeof(Comp_BreastIncrease);
		}
	}
	public class CompProperties_BreastDecrease : CompProperties
	{
		public CompProperties_BreastDecrease()
		{
			compClass = typeof(Comp_BreastDecrease);
		}
	}
	public class CompProperties_GenitalIncrease : CompProperties
	{
		public CompProperties_GenitalIncrease()
		{
			compClass = typeof(Comp_GenitalIncrease);
		}
	}
	public class CompProperties_GenitalDecrease : CompProperties
	{
		public CompProperties_GenitalDecrease()
		{
			compClass = typeof(Comp_GenitalDecrease);
		}
	}
	public class CompProperties_CumIncrease : CompProperties
	{
		public CompProperties_CumIncrease()
		{
			compClass = typeof(Comp_CumIncrease);
		}
	}
	public class CompProperties_CumDecrease : CompProperties
	{
		public CompProperties_CumDecrease()
		{
			compClass = typeof(Comp_CumDecrease);
		}
	}
	public class CompProperties_Genderbend: CompProperties
	{
		public CompProperties_Genderbend()
		{
			compClass = typeof(Comp_Genderbend);
		}
	}
	public class CompProperties_FutaSerum: CompProperties
	{
		public CompProperties_FutaSerum()
		{
			compClass = typeof(Comp_FutaSerum);
		}
	}

	public class CompProperties_AnalTighteningSerum : CompProperties
	{
		public CompProperties_AnalTighteningSerum()
		{
			compClass = typeof(Comp_AnalTighteningSerum);
		}
	}
}
