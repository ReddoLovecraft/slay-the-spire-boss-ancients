using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using System.Collections.Generic;
using System.Linq;

namespace StsBossAncients.Scripts.Utils;

public static class CombatUtils
{
	public static IReadOnlyList<Player> GetAliveTeammatePlayers(Player owner)
	{
		CombatState? cs = owner.Creature.CombatState;
		if (cs == null)
		{
			return new List<Player>();
		}

		return (from c in cs.GetTeammatesOf(owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer && c.Player != null
			select c.Player).ToList();
	}
}
