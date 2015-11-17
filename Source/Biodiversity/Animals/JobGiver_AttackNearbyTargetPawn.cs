using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Animals
{
	internal class JobGiver_AttackNearbyTargetPawn : ThinkNode_JobGiver
	{
		public const float MaxSearchDistance = 3f;

		private const int EnemyForgetTime = 200;

		private const int MaxMeleeChaseTicksMax = 600;

		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Pawn pawn2 = pawn.mindState.enemyTarget as Pawn;
			Job result;
			if (pawn2 == null)
			{
				result = null;
			}
			else if (pawn2.Destroyed || pawn2.Downed || Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick > 200 || (pawn.Position - pawn2.Position).LengthHorizontalSquared > 9f || !GenSight.LineOfSight(pawn.Position, pawn2.Position, false))
			{
				pawn.mindState.enemyTarget = null;
				result = null;
			}
			else if (pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
			{
				result = null;
			}
			else
			{
				Job job = new Job(JobDefOf.AttackMelee, pawn2)
				{
					maxNumMeleeAttacks = 1,
					expiryInterval = Rand.Range(200, 600)
				};
				result = job;
			}
			return result;
		}
	}
}
