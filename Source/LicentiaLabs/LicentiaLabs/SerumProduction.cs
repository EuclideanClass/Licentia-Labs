using Verse;

namespace LicentiaLabs
{
	internal class Hediff_ProducingHormonalSerum : HediffWithComps
	{
		public float SerumProgress
		{
			get => Severity;
			private set => Severity = value;
		}

		public override void Tick()
		{

			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (SerumProgress == 1f)
				{
					GenSpawn.Spawn(Licentia.ThingDefs.HormonalSerum, this.pawn.Position, this.pawn.Map);
					this.Severity = 0f;
				}

			}
		}
	}
}
