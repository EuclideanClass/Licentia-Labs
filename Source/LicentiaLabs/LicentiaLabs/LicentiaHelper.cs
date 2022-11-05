using System.Collections.Generic;
using RimWorld;
using Verse;
using rjw;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Helpers;
using rjw.Modules.Interactions.Enums;
using System.Linq;

namespace LicentiaLabs
{
	public static class Licentia
	{
		public static class TaleDefs
        {
			public static readonly TaleDef VomitedCum = DefDatabase<TaleDef>.GetNamed("VomitedCum");
        }

		public static class ThoughtDefs
		{
			public static readonly ThoughtDef GotInflatedKinky = DefDatabase<ThoughtDef>.GetNamed("KinkyGotInflated");
			public static readonly ThoughtDef GotOverCumflated = DefDatabase<ThoughtDef>.GetNamed("GotOverCumflated");
			public static readonly ThoughtDef GotOverCumflatedEnjoyed = DefDatabase<ThoughtDef>.GetNamed("GotOverCumflatedEnjoyed");
			public static readonly ThoughtDef GotOverCumstuffed = DefDatabase<ThoughtDef>.GetNamed("GotOverCumstuffed");
			public static readonly ThoughtDef GotOverCumstuffedEnjoyed = DefDatabase<ThoughtDef>.GetNamed("GotOverCumstuffedEnjoyed");
			public static readonly ThoughtDef GotStretched = DefDatabase<ThoughtDef>.GetNamed("GotStretched");
			public static readonly ThoughtDef GotStretchedKinky = DefDatabase<ThoughtDef>.GetNamed("KinkyGotStretched");
			public static readonly ThoughtDef GotStretchedMasochist = DefDatabase<ThoughtDef>.GetNamed("MasochistGotStretched");
		}
		public static class HediffDefs
		{
			public static readonly HediffDef CervixBruise = HediffDef.Named("CervixBruise");
			public static readonly HediffDef Cumflation = HediffDef.Named("Cumflation");
			public static readonly HediffDef Cumstuffed = HediffDef.Named("Cumstuffed");
			public static readonly HediffDef Diminished = HediffDef.Named("Diminished");
			public static readonly HediffDef Elasticised = HediffDef.Named("Elasticised");
			public static readonly HediffDef Enlarged = HediffDef.Named("Enlarged");
			public static readonly HediffDef ExtremeProlapse = HediffDef.Named("ExtremeProlapse");
			public static readonly HediffDef Stretched = HediffDef.Named("Stretched");
			public static readonly HediffDef StretchTear = HediffDef.Named("StretchTear");
			public static readonly HediffDef Prolapse = HediffDef.Named("Prolapse");
			public static readonly HediffDef Untearable = HediffDef.Named("Untearable");
		}

		public static class RecordDefs
        {
			public static readonly RecordDef AmountOfCumstuffed = DefDatabase<RecordDef>.GetNamed("AmountOfCumstuffed");
			public static readonly RecordDef AmountOfCumflation = DefDatabase<RecordDef>.GetNamed("AmountOfCumflation");
		}

		public static class ThingDefs
		{
			public static readonly ThingDef FilthCum = ThingDef.Named("FilthCum");
			public static readonly ThingDef HormonalSerum = ThingDef.Named("HormonalSerum");
		}
		public static class TraitDefs
		{
			public static readonly TraitDef likesCumflation = TraitDef.Named("LikesCumflation");
		}
	}

	public static class LicentiaHelper
	{
		public static readonly IDictionary<SexPartType, BodyPartDef> BodyPartDefBySexPartType = new Dictionary<SexPartType, BodyPartDef>();

		static LicentiaHelper() {
			BodyPartDefBySexPartType.Add(SexPartType.Anus, xxx.anusDef);
			BodyPartDefBySexPartType.Add(SexPartType.FemaleBreast, xxx.breastsDef);
			BodyPartDefBySexPartType.Add(SexPartType.FemaleGenital, xxx.genitalsDef);
			BodyPartDefBySexPartType.Add(SexPartType.MaleBreast, xxx.breastsDef);
			BodyPartDefBySexPartType.Add(SexPartType.MaleGenital, xxx.genitalsDef);
		}

		public static bool IsArtificial(Hediff hediff) {
			return !(hediff is Hediff_PartBaseNatural);
		}

		public static Hediff TryGetRJWPartHediff(Pawn pawn, SexPartType partType) {
			switch (partType) {
				case SexPartType.Anus:
					return TryGetPawnAnusHediff(pawn);
				case SexPartType.FemaleGenital:
					return TryGetPawnVaginaHediff(pawn);
				case SexPartType.MaleGenital:
					{
						var hediffs = TryGetPawnPenisHediff(pawn);
						if (hediffs.Count > 0)
                        {
							return hediffs[0];
                        }
						return null;
					}
			}
			
			// default value
			return TryGetPawnBreastHediff(pawn);
		}

		public static List<Hediff> TryGetPawnPenisHediff(Pawn pawn) // used for stretch and futa calculation
		{
			var hediffs = new HashSet<Hediff>();

			var genitalsHediffs = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_genitalsBPR(pawn));
			if (Genital_Helper.has_penis_fertile(pawn, genitalsHediffs) || Genital_Helper.has_penis_infertile(pawn, genitalsHediffs) || Genital_Helper.has_multipenis(pawn, genitalsHediffs))
			{
				hediffs.AddRange(genitalsHediffs.FindAll((Hediff hediff) => hediff.def.defName.ToLower().Contains("penis")));
				hediffs.AddRange(genitalsHediffs.FindAll((Hediff hediff) => hediff.def.defName.ToLower().Contains("ovipositorf")));
				hediffs.AddRange(genitalsHediffs.FindAll((Hediff hediff) => hediff.def.defName.ToLower().Contains("ovipositorm")));
				hediffs.AddRange(genitalsHediffs.FindAll((Hediff hediff) => hediff.def.defName.ToLower().Contains("tentacle")));
			}
			else if (Genital_Helper.has_ovipositorF(pawn, genitalsHediffs))
			{
				hediffs.AddRange(genitalsHediffs.FindAll((Hediff hediff) => hediff.def.defName.ToLower().Contains("ovipositorf")));
			}

			return hediffs.ToList();
		}

		public static Hediff TryGetPawnVaginaHediff(Pawn pawn) // used for stretch and futa calculation
		{
			Hediff hediff = null;
			var pawnparts = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_genitalsBPR(pawn));

			if (Genital_Helper.has_vagina(pawn, pawnparts))
			{
				hediff = pawnparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("vagina")).InRandomOrder().FirstOrDefault();

				if (hediff == null)
					hediff = pawnparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("ovipositorf")).InRandomOrder().FirstOrDefault();
			}
			else
			{
				hediff = null;
			}

			return hediff;
		}

		public static Hediff TryGetPawnAnusHediff(Pawn pawn)
		{
			BodyPartRecord Part = Genital_Helper.get_anusBPR(pawn);
			if (Part is null)
				return null;

			return pawn.health.hediffSet.hediffs.FindAll((Hediff hed) =>
					hed.Part == Part &&
					(hed is Hediff_PartBaseNatural || hed is Hediff_PartBaseArtifical) &&
					(hed.def.defName.ToLower().Contains("anus"))).InRandomOrder().FirstOrDefault();
		}

		public static Hediff TryGetPawnBreastHediff(Pawn pawn)
		{
			BodyPartRecord Part = Genital_Helper.get_breastsBPR(pawn);
			if (Part is null)
				return null;

			return pawn.health.hediffSet.hediffs.FindAll((Hediff hed) =>
					hed.Part == Part &&
					(hed is Hediff_PartBaseNatural || hed is Hediff_PartBaseArtifical) &&
					(hed.def.defName.ToLower().Contains("breasts") || hed.def.defName.ToLower().Contains("chest"))
					).InRandomOrder().FirstOrDefault();
		}

		public static bool GetPenetrator(SexProps props, out Pawn penetrator, out Pawn penetrated)
		{
			penetrator = null;
			penetrated = null;

			bool initiatorHasPenis = true;
			bool partnerHasPenis = true;
			if (TryGetPawnPenisHediff(props.pawn).EnumerableNullOrEmpty())
			{
				initiatorHasPenis = false;
			}
			if (TryGetPawnPenisHediff(props.partner).EnumerableNullOrEmpty())
			{
				partnerHasPenis = false;
			}

			if (!initiatorHasPenis && !partnerHasPenis){
				return false;
			}

			InteractionWithExtension interaction = InteractionHelper.GetWithExtension(props.dictionaryKey);
			bool initiatorUsesPenis;
			bool partnerUsesPenis;
			if (!props.isReceiver)
			{
				initiatorUsesPenis = interaction?.DominantHasTag(GenitalTag.CanPenetrate) ?? false;
				partnerUsesPenis = interaction?.SubmissiveHasTag(GenitalTag.CanPenetrate) ?? false;
			}
			else
			{
				initiatorUsesPenis = interaction?.SubmissiveHasTag(GenitalTag.CanPenetrate) ?? false;
				partnerUsesPenis = interaction?.DominantHasTag(GenitalTag.CanPenetrate) ?? false;
			}

			if (initiatorHasPenis && initiatorUsesPenis)
			{
				penetrator = props.pawn;
				penetrated = props.partner;
				return true;
			}
			else if (partnerHasPenis && partnerUsesPenis)
			{
				penetrator = props.partner;
				penetrated = props.pawn;
				return true;
			}

			return false;
		}

		public static bool AddSexPart(Pawn pawn, rjw.SexPartType partType, float severity = 0) {
			if(pawn == null) {
				return false;
			}

			if(PawnExtensions.TryAddRacePart(pawn, partType)) {

				Hediff hdf = TryGetRJWPartHediff(pawn, partType);
				CompHediffBodyPart chdf = hdf.TryGetComp<rjw.CompHediffBodyPart>();
				if (chdf != null) {
					chdf.updatesize(severity);
				}

				return true;
			}

			string defaultHediffName = "GenericBreasts";
			switch(partType) {
				case SexPartType.Anus:
					defaultHediffName = "GenericAnus";
					break;
				case SexPartType.FemaleGenital:
					defaultHediffName = "GenericVagina";
					break;
				case SexPartType.MaleGenital:
					defaultHediffName = "GenericPenis";
					break;
			}

			BodyPartRecord bpr = pawn.RaceProps.body.AllParts.Find(item => item.def == BodyPartDefBySexPartType[partType]);
			HediffDef hdd = DefDatabase<HediffDef>.GetNamed(defaultHediffName);
			Hediff hd = HediffMaker.MakeHediff(hdd, pawn, bpr);
			CompHediffBodyPart chd = hd.TryGetComp<CompHediffBodyPart>();

			if (chd == null) {
				return false;
			}
			
			chd.initComp(pawn);
			chd.updatesize(severity);
			pawn.health.AddHediff(hd);

			return true;
		}

		public static T Clamp<T>(T value, T min, T max) where T : System.IComparable<T> {
			if (value.CompareTo(min) < 0) return min;
			else if (value.CompareTo(max) > 0) return max;
			return value;
		}
	}
}
