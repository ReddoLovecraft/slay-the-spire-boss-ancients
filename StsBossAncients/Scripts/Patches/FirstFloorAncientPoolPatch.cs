using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Unlocks;
using StsBossAncicents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StsBossAncients.Scripts.Patches
{
	[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
	public static class FirstFloorAncientPoolPatch
	{
		private static readonly FieldInfo? RoomsField = AccessTools.Field(typeof(ActModel), "_rooms");

		public static void Postfix(ActModel __instance, Rng rng, UnlockState unlockState, bool isMultiplayer)
		{
			if (__instance.ActNumber() != 1)
			{
				return;
			}
			if (RoomsField == null)
			{
				return;
			}
			if (RoomsField.GetValue(__instance) is not RoomSet rooms)
			{
				return;
			}

			List<AncientEventModel> candidates = new List<AncientEventModel>();
			try
			{
				candidates.Add(ModelDb.AncientEvent<Neow>());
			}
			catch
			{
			}

			foreach (Type t in GetFirstFloorAncientTypes())
			{
				if (!ModelDb.Contains(t))
				{
					continue;
				}
				ModelId id = ModelDb.GetId(t);
				AncientEventModel? ancient = ModelDb.GetByIdOrNull<AncientEventModel>(id);
				if (ancient != null)
				{
					candidates.Add(ancient);
				}
			}

			candidates = candidates.Distinct().ToList();
			if (candidates.Count == 0)
			{
				return;
			}

			AncientEventModel? chosen = rng.NextItem(candidates);
			if (chosen == null)
			{
				return;
			}
			rooms.Ancient = chosen;
		}

		private static IEnumerable<Type> GetFirstFloorAncientTypes()
		{
			Assembly a = typeof(FirstFloorAncientModel).Assembly;
			foreach (Type t in a.GetTypes())
			{
				if (t.IsAbstract)
				{
					continue;
				}
				if (!typeof(FirstFloorAncientModel).IsAssignableFrom(t))
				{
					continue;
				}
				yield return t;
			}
		}
	}
}
