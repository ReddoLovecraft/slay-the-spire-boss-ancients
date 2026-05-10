using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class MalignantTumor : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MinionPower>(),MegaCrit.Sts2.Core.MonsterMoves.Intents.StunIntent.GetStaticHoverTip()];
	private readonly HashSet<uint> _alreadySplit = new();
	private static readonly HashSet<string> _excludedExactEntries = new(StringComparer.OrdinalIgnoreCase)
	{
		"BIG_DUMMY",
		"THE_OBSCURA",
		"CRUSHER",
		"ROCKET",
		"TEST_SUBJECT"
	};

	public override async Task BeforeCombatStart()
	{
		_alreadySplit.Clear();
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

		Flash();
		foreach (Creature enemy in cs.HittableEnemies.ToList())
		{
			int lose = (int)Math.Floor(enemy.MaxHp * 0.5m);
			if (lose <= 0)
			{
				continue;
			}
			await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), enemy, lose, false);
		}
	}

	public override bool ShouldDieLate(Creature creature)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			return true;
		}
		if (creature.Side != CombatSide.Enemy)
		{
			return true;
		}
		if (creature.IsSecondaryEnemy)
		{
			return true;
		}
		if (creature.Monster == null)
		{
			return true;
		}

		if (IsExcludedForSplit(creature))
		{
			return true;
		}

		if (creature.CombatId is uint id && _alreadySplit.Contains(id))
		{
			return true;
		}

		return false;
	}

		public override async Task AfterPreventingDeath(Creature creature)
		{
		if (!CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (creature.Side != CombatSide.Enemy)
		{
			return;
		}
		if (creature.IsSecondaryEnemy)
		{
			return;
		}
		if (creature.Monster == null)
		{
			return;
		}
		CombatState? cs = creature.CombatState;
		if (cs == null)
		{
			return;
		}

		uint? combatId = creature.CombatId;
		if (combatId == null)
		{
			return;
		}
		if (_alreadySplit.Contains(combatId.Value))
		{
			return;
		}

		if (IsExcludedForSplit(creature))
		{
			return;
		}

		int childMaxHp = Math.Max(1, (int)Math.Floor(creature.MaxHp * 0.5m));

		Flash();
		_alreadySplit.Add(combatId.Value);

		IReadOnlyList<string> encounterSlots = cs.Encounter?.Slots ?? Array.Empty<string>();
		bool hasEncounterSlots = encounterSlots.Count > 0;
		string? slotA = null;
		string? slotB = null;
		if (hasEncounterSlots)
		{
			string? originalSlot = creature.SlotName;
			creature.SlotName = null;
			slotA = string.IsNullOrWhiteSpace(originalSlot) ? encounterSlots[0] : originalSlot;
			slotB = encounterSlots.FirstOrDefault(s =>
				!string.IsNullOrWhiteSpace(s)
				&& !string.Equals(s, slotA, StringComparison.OrdinalIgnoreCase)
				&& cs.Enemies.All(e => !string.Equals(e.SlotName, s, StringComparison.OrdinalIgnoreCase)));
			if (string.IsNullOrWhiteSpace(slotB))
			{
				slotB = slotA;
			}
		}

		NCombatRoom? room = NCombatRoom.Instance;
		Vector2? basePos = room?.GetCreatureNode(creature)?.GlobalPosition;
		float offsetX = 110f;
		if (basePos.HasValue)
		{
			float width = room?.GetCreatureNode(creature)?.Hitbox.Size.X ?? 0f;
			if (width > 0f)
			{
				offsetX = MathF.Max(90f, width * 0.45f);
			}
		}

		for (int i = 0; i < 2; i++)
		{
			MonsterModel monster = creature.Monster.CanonicalInstance.ToMutable();
			string? childSlot = hasEncounterSlots ? ((i == 0) ? slotA : slotB) : null;
			Creature child = await CreatureCmd.Add(monster, cs, CombatSide.Enemy, childSlot);
			await CreatureCmd.SetMaxAndCurrentHp(child, childMaxHp);
			if (child.Monster != null)
			{
				string nextMoveId = child.Monster.NextMove.Id;
				if (string.IsNullOrWhiteSpace(nextMoveId) || string.Equals(nextMoveId, "UNSET_MOVE", StringComparison.OrdinalIgnoreCase))
				{
					child.Monster.RollMove(cs.PlayerCreatures);
					nextMoveId = child.Monster.NextMove.Id;
				}
				await CreatureCmd.Stun(child, nextMoveId);
			}
			else
			{
				await CreatureCmd.Stun(child);
			}
			if (basePos.HasValue && room != null)
			{
				var childNode = room.GetCreatureNode(child);
				if (childNode != null)
				{
					float yOffset = (i == 0) ? -20f : 20f;
					float xOffset = (i == 0) ? -offsetX : offsetX;
					childNode.GlobalPosition = basePos.Value + new Vector2(xOffset, yOffset);
				}
			}
			if (child.CombatId is uint childId)
			{
				_alreadySplit.Add(childId);
			}
		}

		await CreatureCmd.SetMaxAndCurrentHp(creature, 1);
			await CreatureCmd.Kill(creature, force: true);
		}

		private static bool IsExcludedForSplit(Creature creature)
		{
			MonsterModel? monster = creature.Monster;
			if (monster == null)
			{
				return true;
			}
			string entry = monster.Id.Entry ?? string.Empty;
			if (entry.Length == 0)
			{
				return true;
			}
			if (_excludedExactEntries.Contains(entry))
			{
				return true;
			}
			if (entry.StartsWith("DECIMILLIPEDE_SEGMENT", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (entry.StartsWith("BATTLE_FRIEND_V", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}
	}
}
