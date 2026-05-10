using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using System.Reflection;

namespace StsBossAncients.Scripts.Main;

public static class StsBossAncientsReflection
{
	public static bool IsLikelyOsty(Creature creature, Player owner)
	{
		if (creature.PetOwner == owner)
		{
			return true;
		}
		if (creature.IsPet && creature.PetOwner == owner)
		{
			return true;
		}
		return false;
	}

	

	public static MonsterModel? TryGetMonsterModelByName(string keyword)
	{
		try
		{
			Assembly a = typeof(CreatureCmd).Assembly;
			MethodInfo? monsterGeneric = typeof(ModelDb).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.FirstOrDefault(m => m.Name == "Monster" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 0);
			if (monsterGeneric == null)
			{
				return null;
			}

			Type? monsterModelType = typeof(MonsterModel);
			foreach (Type t in a.GetTypes())
			{
				if (!monsterModelType.IsAssignableFrom(t))
				{
					continue;
				}
				if (!t.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				MethodInfo mi = monsterGeneric.MakeGenericMethod(t);
				if (mi.Invoke(null, null) is MonsterModel monster)
				{
					return monster;
				}
			}
			return null;
		}
		catch
		{
			return null;
		}
	}
}
