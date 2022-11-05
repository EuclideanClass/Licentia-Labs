using RimWorld;
using Verse;
using rjw;
using System.Linq;

namespace LicentiaLabs
{
	public class Comp_MutagenicSerum : ThingComp
	{
		public CompProperties_MutagenicSerum Props => (CompProperties_MutagenicSerum)props;

		public override void PostIngested(Pawn pawn)
		{
			var partBPR = Genital_Helper.get_genitalsBPR(pawn);
			var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			var newVagina = Props.vaginaDef;
			var newPenis = Props.penisDef;

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
						hd = HediffMaker.MakeHediff(newPenis, pawn, partBPR);
					}

					if (hed.def.defName.ToLower().Contains("vagina") ||
						hed.def.defName.ToLower().Contains("ovipositorf")
						)
					{
						hd = HediffMaker.MakeHediff(newVagina, pawn, partBPR);
					}

					if (hd != null)
					{
						CompHediff = hd.TryGetComp<rjw.CompHediffBodyPart>();
						if (CompHediff != null)
						{
							//Log.Message("SexPartAdder::PartMaker init comps");
							CompHediff.initComp(pawn);
							CompHediff.updatesize();
						}
						GenderHelper.ChangeSex(pawn, () =>
						{
							pawn.health.RemoveHediff(hed);
							pawn.health.AddHediff(hd, partBPR);
						});
					}
				}

				var message = "LL_GenitalsAltered".Translate(pawn);
				Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
			}
			else
			{
				Messages.Message("LL_GenitalsNotAlterable".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);

				return;
			}
		}
	}

	public class Comp_MutagenicSerumSpecial : ThingComp
	{

		public CompProperties_MutagenicSerumSpecial Props => (CompProperties_MutagenicSerumSpecial)props;

		public override void PostIngested(Pawn pawn)
		{
			if (Props.breast && GenderHelper.GetSex(pawn) != GenderHelper.Sex.male)
			{
				var partBPR = Genital_Helper.get_breastsBPR(pawn);
				var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);
				var newParts = Props.breastDef;

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
						hd = HediffMaker.MakeHediff(newParts, pawn, partBPR);

						if (hd != null)
						{
							CompHediff = hd.TryGetComp<rjw.CompHediffBodyPart>();
							if (CompHediff != null)
							{
								//Log.Message("SexPartAdder::PartMaker init comps");
								CompHediff.initComp(pawn);
								CompHediff.updatesize();
							}
							GenderHelper.ChangeSex(pawn, () =>
							{
								pawn.health.RemoveHediff(hed);
								pawn.health.AddHediff(hd, partBPR);
							});
						}
					}

					var message = "LL_BreastsAltered".Translate(pawn);
					Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
				}
			}

			if (Props.anus)
			{
				var partBPR = Genital_Helper.get_anusBPR(pawn);
				var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);
				var newParts = Props.anusDef;

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
						hd = HediffMaker.MakeHediff(newParts, pawn, partBPR);

						if (hd != null)
						{
							CompHediff = hd.TryGetComp<rjw.CompHediffBodyPart>();
							if (CompHediff != null)
							{
								//Log.Message("SexPartAdder::PartMaker init comps");
								CompHediff.initComp(pawn);
								CompHediff.updatesize();
							}
							GenderHelper.ChangeSex(pawn, () =>
							{
								pawn.health.RemoveHediff(hed);
								pawn.health.AddHediff(hd, partBPR);
							});
						}
					}

					var message = "LL_AnusAltered".Translate(pawn);
					Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
				}
			}
		}
	}

	public class Comp_ReverterSerum : ThingComp
	{
		//bool futa = false;
		public override void PostIngested(Pawn pawn)
		{
			if (GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.none) {
				// what is happenning??
				return;
			}

			GenderHelper.ChangeSex(pawn, () => 
			{
				// trap, futa, female & male could all revert their anus
				var partBPR = Genital_Helper.get_anusBPR(pawn);
				var oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

				if (!oldParts.NullOrEmpty())
				{
					foreach (Hediff hed in oldParts)
					{
						if (LicentiaHelper.IsArtificial(hed))
							continue;

						pawn.health.RemoveHediff(hed);
						LicentiaHelper.AddSexPart(pawn, SexPartType.Anus);
					}
				}

				partBPR = Genital_Helper.get_genitalsBPR(pawn);
				oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

				// male, trap & futas gets a dic
				if (GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.male
				|| GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.trap
				|| GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.futa)
				{
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
								pawn.health.RemoveHediff(hed);
								LicentiaHelper.AddSexPart(pawn, SexPartType.MaleGenital);
							}
						}
					}
				}

				// female & futa gets a vagoo
				if(GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.female
				|| GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.futa)
				{
					if (!oldParts.NullOrEmpty())
					{
						foreach (Hediff hed in oldParts)
						{
							if (LicentiaHelper.IsArtificial(hed))
								continue;

							if (hed.def.defName.ToLower().Contains("vagina") ||
								hed.def.defName.ToLower().Contains("ovipositorf")
								)
							{
								pawn.health.RemoveHediff(hed);
								LicentiaHelper.AddSexPart(pawn, SexPartType.FemaleGenital);
							}
						}
					}
				}

				partBPR = Genital_Helper.get_breastsBPR(pawn);
				oldParts = Genital_Helper.get_PartsHediffList(pawn, partBPR);

				// males have flat chest
				if (GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.male)
				{
					foreach (Hediff hed in oldParts)
					{
						if (LicentiaHelper.IsArtificial(hed))
							continue;

						pawn.health.RemoveHediff(hed);
						LicentiaHelper.AddSexPart(pawn, SexPartType.MaleBreast, 0.01f);
					}
				}

				// females have classic chest
				if(GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.female
				|| GenderHelper.GetOriginalSex(pawn) == GenderHelper.Sex.futa)
				{
					foreach (Hediff hed in oldParts)
					{
						if (LicentiaHelper.IsArtificial(hed))
							continue;

						pawn.health.RemoveHediff(hed);
						LicentiaHelper.AddSexPart(pawn, SexPartType.FemaleBreast);
					}
				}
			});

			Messages.Message("LL_Reverted".Translate(pawn), pawn, MessageTypeDefOf.SilentInput);
		}
	}

	public class Comp_TraitSerum : ThingComp
	{
		public CompProperties_TraitSerum Props => (CompProperties_TraitSerum)props;
		public override void PostIngested(Pawn pawn)
		{
			if (Props.addTrait != null)
			{
				if (!Props.replaceTrait)
				{
					pawn.story.traits.GainTrait(new Trait(Props.addTrait));
				}
				else
				{
					pawn.story.traits.allTraits.Remove(pawn.story.traits.GetTrait(Props.removeTrait));
					pawn.story.traits.GainTrait(new Trait(Props.addTrait));
				}
			}
			else
			{
				pawn.story.traits.allTraits.Remove(pawn.story.traits.GetTrait(Props.removeTrait));
			}
			var message = "LL_PsycheAltered".Translate(pawn);
			Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
		}
	}

	public class CompProperties_MutagenicSerum : CompProperties
	{
		public HediffDef penisDef;
		public HediffDef vaginaDef;

		public CompProperties_MutagenicSerum()
		{
			compClass = typeof(Comp_MutagenicSerum);
		}
	}
	public class CompProperties_MutagenicSerumSpecial : CompProperties
	{
		public bool anus = false;
		public bool breast = false;
		public HediffDef anusDef;
		public HediffDef breastDef;

		public CompProperties_MutagenicSerumSpecial()
		{
			compClass = typeof(Comp_MutagenicSerumSpecial);
		}
	}
	public class CompProperties_ReverterSerum : CompProperties
	{
		public CompProperties_ReverterSerum()
		{
			compClass = typeof(Comp_ReverterSerum);
		}
	}
	public class CompProperties_TraitSerum : CompProperties
	{
		public bool replaceTrait = false;
		public TraitDef addTrait;
		public TraitDef removeTrait;
		public CompProperties_TraitSerum()
		{
			compClass = typeof(Comp_TraitSerum);
		}
	}
}
