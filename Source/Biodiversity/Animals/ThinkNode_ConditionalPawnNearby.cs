using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Animals
{
	public class ThinkNode_ConditionalPawnNearby : ThinkNode_Priority
	{
		private int CheckForEnemyTime;

		public bool AttackOtherAnimals = false;

		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			Pawn pawn2 = pawn.mindState.enemyTarget as Pawn;
			ThinkResult result;
			if (pawn2 != null)
			{
				result = base.TryIssueJobPackage(pawn);
			}
			else if (Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick < this.CheckForEnemyTime)
			{
				result = ThinkResult.NoJob;
			}
			else
			{
				pawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
				this.CheckForEnemyTime = Rand.RangeInclusive(180, 600);
				pawn2 = this.FindTarget(pawn, 3f, this.AttackOtherAnimals);
				if (pawn2 == null)
				{
					result = ThinkResult.NoJob;
				}
				else
				{
					pawn.mindState.enemyTarget = pawn2;
					result = base.TryIssueJobPackage(pawn);
				}
			}
			return result;
		}

		public virtual Pawn FindTarget(Pawn pawn, float MaxAttackDistance, bool attackAnimals = false)
		{
			Predicate<Thing> predicate = delegate(Thing t)
			{
				bool result;
				if (t == pawn)
				{
					result = false;
				}
				else
				{
					Pawn pawn5 = t as Pawn;
					result = (!pawn5.Downed && t.SpawnedInWorld && !t.Destroyed && !(t.Position == IntVec3.Invalid) && (attackAnimals || !pawn5.RaceProps.Animal) && pawn5.def != pawn.def && GenSight.LineOfSight(pawn.Position, t.Position, false));
				}
				return result;
			};
			Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false), MaxAttackDistance, predicate, null, 2, false);
			Pawn pawn3 = new Pawn();
			Pawn pawn4;
			if (pawn2 == null)
			{
				pawn4 = null;
			}
			else
			{
				Log.Warning("Pre Faction Check");
				if (pawn.Faction != null && pawn2.Faction != null)
				{
					Log.Warning("Faction Check");
					Log.Warning(string.Concat(new object[]
					{
						"Ani Fac = ",
						pawn.Faction.def,
						". \nTarPwn = ",
						pawn2.Faction.def,
						"."
					}));
					if (pawn.Faction.def == pawn2.Faction.def)
					{
						Log.Warning("Eqv");
						pawn4 = null;
						goto IL_347;
					}
				}
				else
				{
					Log.Warning("Sorry mate... somthing was null.");
				}
				Log.Warning("Pre Faction ally Check");
				if (pawn.Faction != null && pawn2.Faction != null)
				{
					Log.Warning("Faction ally Check");
					Log.Warning(string.Concat(new object[]
					{
						"Ani Fac = ",
						pawn.Faction.def,
						". \nTarPwn = ",
						pawn2.Faction.def,
						"."
					}));
					if (FactionUtility.HostileTo(pawn.Faction, pawn2.Faction))
					{
						Log.Warning("Eqv");
						pawn4 = pawn2;
					}
					else
					{
						pawn4 = null;
					}
				}
				else
				{
					Log.Warning("Sorry mate... somthing was null. ally");
					pawn4 = pawn2;
					Log.Warning("Did we Faction(ally) Check?");
					if (pawn2.CurJob == null)
					{
						pawn4 = pawn2;
					}
					else if (pawn2.CurJob.def == JobDefOf.Tame)
					{
						if (pawn2.CurJob.targetA != null)
						{
							Log.Warning("TarA");
							if (pawn2.CurJob.targetA.Thing != null)
							{
								Log.Warning("TarA.Thing... ID: " + pawn2.CurJob.targetA.Thing.ThingID);
								Log.Warning("Pwn ID: " + pawn.ThingID);
								if (pawn2.CurJob.targetA.Thing.ThingID == pawn.ThingID)
								{
									Log.Warning("Eqv");
									pawn4 = null;
								}
							}
						}
						else
						{
							pawn4 = pawn2;
						}
					}
				}
			}
			IL_347:
			if (pawn4 == pawn3)
			{
				pawn4 = pawn2;
			}
			if (pawn4 == pawn)
			{
				pawn4 = null;
			}
			return pawn4;
		}
	}
}
