
using RimWorld;
using System;
using Verse;

namespace LicentiaLabs.Hediffs
{
	public class HediffComp_VomitCumMtb : HediffComp
	{
		public HediffCompProperties_VomitCumMtb Props
		{
			get
			{
				return (HediffCompProperties_VomitCumMtb)this.props;
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (this.Props.mtbDaysPerStage[this.parent.CurStageIndex] > 0f && base.Pawn.IsHashIntervalTick(60) && Rand.MTBEventOccurs(this.Props.mtbDaysPerStage[this.parent.CurStageIndex], 60000f, 60f))
			{
				// If the Pawn likes cumflation, they're going to try to keep it in. Still, a little will leak out, so generate filth but do not vomit.
				if (this.Pawn.story.traits.HasTrait(Licentia.TraitDefs.likesCumflation))
				{
					FilthMaker.TryMakeFilth(this.Pawn.PositionHeld, this.Pawn.MapHeld, Licentia.ThingDefs.FilthCum, 1);
					return;
                }

				this.Pawn.jobs.StartJob(JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VomitCum")), lastJobEndCondition: Verse.AI.JobCondition.InterruptForced, resumeCurJobAfterwards: true);
			}
		}
	}
}
