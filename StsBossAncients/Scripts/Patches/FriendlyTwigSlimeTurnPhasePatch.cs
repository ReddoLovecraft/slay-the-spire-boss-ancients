using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Patches
{
	[HarmonyPatch(typeof(CombatManager), "SwitchFromPlayerToEnemySide")]
	public static class FriendlyTwigSlimeTurnPhasePatch
	{
		private const string MoveClumpId = "STSBOSSANCIENTS_SLIME_TOPHAT_CLUMP";
		private const string MoveDebuffId = "STSBOSSANCIENTS_SLIME_TOPHAT_DEBUFF";
		private const string MoveBodySlamId = "STSBOSSANCIENTS_SLIME_TOPHAT_BODYSLAM";

		private const int ClumpShotDamage = 12;
		private const int StickyShotStacks = 2;
		private const int BodySlamBlock = 8;
		private const int BodySlamDamage = 8;

		public static void Prefix(CombatManager __instance, ref Func<Task>? actionDuringEnemyTurn)
		{
			Func<Task>? original = actionDuringEnemyTurn;
			actionDuringEnemyTurn = async () =>
			{
				if (original != null)
				{
					await original();
				}
				var stateField = AccessTools.Field(typeof(CombatManager), "_state");
				CombatState? combatState = stateField?.GetValue(__instance) as CombatState;
				if (combatState != null && combatState.CurrentSide == CombatSide.Enemy)
				{
					await ExecuteAll(combatState);
				}
			};
		}

		private static async Task ExecuteAll(CombatState combatState)
		{
			PlayerChoiceContext ctx = new ThrowingPlayerChoiceContext();
			foreach (Player player in combatState.Players)
			{
				if (!player.Relics.Any(r => r is SlimeTopHat))
				{
					continue;
				}
				Creature owner = player.Creature;
				List<Creature> slimes = owner.Pets.Where(p => p.IsAlive && p.Monster is TwigSlimeM && p.PetOwner == player).ToList();
				foreach (Creature slime in slimes)
				{
					await ExecuteSingle(combatState, slime, ctx);
				}
			}
		}

		private static async Task ExecuteSingle(CombatState combatState, Creature slime, PlayerChoiceContext ctx)
		{
			if (!combatState.ContainsCreature(slime))
			{
				return;
			}
			if (combatState.HittableEnemies.Count == 0)
			{
				return;
			}
			if (slime.Monster == null)
			{
				return;
			}

			Rng rng = slime.PetOwner?.RunState.Rng.Niche ?? combatState.RunState.Rng.Niche;
			string moveId = slime.Monster.NextMove.Id;
			if (!IsOurMove(moveId))
			{
				int roll = rng.NextInt(3);
				SetMoveAndUpdateIntent(combatState, slime, roll);
				moveId = slime.Monster.NextMove.Id;
			}

			NCreature? node = NCombatRoom.Instance?.GetCreatureNode(slime);
			if (node != null)
			{
				await node.PerformIntent();
			}

			switch (moveId)
			{
				case MoveClumpId:
					await ClumpShot(combatState, slime, ctx);
					break;
				case MoveDebuffId:
					await StickyShot(combatState, slime);
					break;
				default:
					await BodySlam(combatState, slime, ctx);
					break;
			}

			int nextRoll = rng.NextInt(3);
			SetMoveAndUpdateIntent(combatState, slime, nextRoll);
		}

		private static bool IsOurMove(string? moveId)
		{
			return string.Equals(moveId, MoveClumpId, StringComparison.OrdinalIgnoreCase)
				|| string.Equals(moveId, MoveDebuffId, StringComparison.OrdinalIgnoreCase)
				|| string.Equals(moveId, MoveBodySlamId, StringComparison.OrdinalIgnoreCase);
		}

		internal static void SetMoveAndUpdateIntent(CombatState combatState, Creature slime, int roll)
		{
			if (slime.Monster == null)
			{
				return;
			}

			MoveState move = roll switch
			{
				0 => CreateMove(MoveClumpId, new SingleAttackIntent(ClumpShotDamage)),
				1 => CreateMove(MoveDebuffId, new DebuffIntent(strong: false)),
				_ => CreateMove(MoveBodySlamId, new DefendIntent(), new SingleAttackIntent(BodySlamDamage))
			};

			slime.Monster.SetMoveImmediate(move, forceTransition: true);

			NCreature? node = NCombatRoom.Instance?.GetCreatureNode(slime);
			if (node != null)
			{
				node.UpdateIntent(combatState.HittableEnemies);
				node.IntentContainer.Modulate = Colors.White;
			}
		}

		private static MoveState CreateMove(string id, params AbstractIntent[] intents)
		{
			return new MoveState(id, NoOpMove, intents)
			{
				FollowUpStateId = "RAND"
			};
		}

		private static Task NoOpMove(IReadOnlyList<Creature> _)
		{
			return Task.CompletedTask;
		}

		private static Creature GetDealer(Creature slime)
		{
			return slime.PetOwner?.Creature ?? slime;
		}

		private static async Task ClumpShot(CombatState combatState, Creature slime, PlayerChoiceContext ctx)
		{
			Creature? target = combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
			if (target != null && target.IsAlive)
			{
				await CreatureCmd.Damage(ctx, target, ClumpShotDamage, ValueProp.Move, GetDealer(slime), null);
			}
		}

		private static async Task StickyShot(CombatState combatState, Creature slime)
		{
			Creature dealer = GetDealer(slime);
			foreach (Creature enemy in combatState.HittableEnemies.ToList())
			{
				await PowerCmd.Apply<WeakPower>(enemy, StickyShotStacks, dealer, null);
				await PowerCmd.Apply<VulnerablePower>(enemy, StickyShotStacks, dealer, null);
			}
		}

		private static async Task BodySlam(CombatState combatState, Creature slime, PlayerChoiceContext ctx)
		{
			Creature dealer = GetDealer(slime);
			await CreatureCmd.GainBlock(slime, BodySlamBlock, ValueProp.Move, null);
			await CreatureCmd.Damage(ctx, combatState.HittableEnemies.ToList(), BodySlamDamage, ValueProp.Move, dealer, null);
		}
	}

	[HarmonyPatch(typeof(NCombatRoom), "AddCreature")]
	public static class FriendlyTwigSlimePlacementPatch
	{
		private static readonly AccessTools.FieldRef<NCreature, object> _stateDisplayRef =
			AccessTools.FieldRefAccess<NCreature, object>("_stateDisplay");

		public static void Postfix(NCombatRoom __instance, Creature creature)
		{
			Player? owner = creature.PetOwner;
			if (owner == null)
			{
				return;
			}
			if (creature.Monster is not TwigSlimeM)
			{
				return;
			}
			if (!owner.Relics.Any(r => r is SlimeTopHat))
			{
				return;
			}

			NCreature? ownerNode = __instance.GetCreatureNode(owner.Creature);
			if (ownerNode == null)
			{
				return;
			}

			List<NCreature> slimeNodes = __instance.CreatureNodes
				.Where(n => n.Entity.PetOwner == owner && n.Entity.Monster is TwigSlimeM)
				.OrderBy(n => n.Position.X)
				.ToList();

			float gap = 30f;
			float x = ownerNode.Position.X + ownerNode.Hitbox.Size.X * 0.6f;
			float y = ownerNode.Position.Y + 10f;
			foreach (NCreature n in slimeNodes)
			{
				float width = Math.Max(80f, Math.Max(n.Hitbox.Size.X, n.Visuals.Bounds.Size.X));
				n.Position = new Vector2(x, y);
				object stateDisplayObj2 = _stateDisplayRef(n);
				if (stateDisplayObj2 is CanvasItem stateDisplay2)
				{
					stateDisplay2.Visible = true;
				}
				n.Hitbox.FocusMode = Control.FocusModeEnum.None;
				n.Hitbox.MouseFilter = Control.MouseFilterEnum.Ignore;
				x += width + gap;
			}

			CombatState? combatState = creature.CombatState;
			if (combatState == null)
			{
				return;
			}

			Rng rng = owner.RunState.Rng.Niche ?? combatState.RunState.Rng.Niche;
			int roll = rng.NextInt(3);
			FriendlyTwigSlimeTurnPhasePatch.SetMoveAndUpdateIntent(combatState, creature, roll);
		}
	}

	[HarmonyPatch(typeof(Creature), nameof(Creature.PrepareForNextTurn))]
	public static class FriendlyTwigSlimePreventAutoRollPatch
	{
		public static void Prefix(Creature __instance, ref bool rollNewMove)
		{
			if (!rollNewMove)
			{
				return;
			}
			Player? owner = __instance.PetOwner;
			if (owner == null)
			{
				return;
			}
			if (__instance.Monster is not TwigSlimeM)
			{
				return;
			}
			if (!owner.Relics.Any(r => r is SlimeTopHat))
			{
				return;
			}
			rollNewMove = false;
		}
	}
}
