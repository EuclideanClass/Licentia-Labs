using Verse;

namespace LicentiaLabs {
	
	public class Comp_LicentiaPart : HediffComp {
	}

	public class CompProperties_LicentiaPart : HediffCompProperties {
		public float stretchVariance = 0f;
		public float genitalSize = 0.5f;
		public float cumAmount = -1f;
		public HediffDef oppositeSexOrgan;
		public HediffDef oppositeBreasts;
		public HediffDef extrudedSexOrgan;
		public HediffDef retractedSexOrgan;

		public CompProperties_LicentiaPart()
		{
			compClass = typeof(Comp_LicentiaPart);
		}
	}
}
