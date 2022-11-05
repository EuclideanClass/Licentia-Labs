using LicentiaLabs;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using UnityEngine;

namespace RimWorld
{
    class JobDriver_VomitCum : JobDriver_Vomit
    {
		public override bool CanBeginNowWhileLyingDown()
        {
			return true;
        }

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				this.ticksLeft = Rand.Range(300, 900);
				int num = 0;
				IntVec3 c;
				for (;;)
				{
					c = this.pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
					num++;
					if (num > 12)
					{
						break;
					}
					if (c.InBounds(this.pawn.Map) && c.Standable(this.pawn.Map))
					{
						goto IL_77;
					}
				}
				c = this.pawn.Position;
			IL_77:
				this.job.targetA = c;
				this.pawn.pather.StopDead();
			};
			toil.tickAction = delegate()
			{
				if (this.ticksLeft % 150 == 149)
				{
					if (!sourceName.NullOrEmpty())
					{
						FilthMaker.TryMakeFilth(this.job.targetA.Cell, base.Map, Licentia.ThingDefs.FilthCum, sourceName);
					}
					else
					{
						FilthMaker.TryMakeFilth(this.job.targetA.Cell, base.Map, Licentia.ThingDefs.FilthCum);
					}
					if (this.pawn.needs.food.CurLevelPercentage > 0.1f)
					{
						this.pawn.needs.food.CurLevel -= this.pawn.needs.food.MaxLevel * 0.005f;
					}
				}
				this.ticksLeft--;
				if (this.ticksLeft <= 0)
				{
					base.ReadyForNextToil();
					TaleRecorder.RecordTale(Licentia.TaleDefs.VomitedCum, new object[]
					{
						this.pawn
					});
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.WithEffect(EffecterDefOf.Vomit, TargetIndex.A, new Color(100f, 100f, 100f, 0.5f));
			toil.PlaySustainerOrSound(() => SoundDefOf.Vomit, 1f);
			yield return toil;
			yield break;
		}

		private int ticksLeft;

		public string sourceName;
	}
}
