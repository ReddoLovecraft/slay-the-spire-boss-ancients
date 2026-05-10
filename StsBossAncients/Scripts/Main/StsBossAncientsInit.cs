using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Saves;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace StsBossAncients.Scripts.Main
{
	[ModInitializer("Init")]
	public class StsBossAncientsInit
	{
	private static Harmony? _harmony;
	public static void Init()
	{
		 TryRegisterGodotScriptAssembly();
		_harmony = new Harmony("StsBossAncients");
		_harmony.PatchAll();
		Log.Debug("StsBossAncients mod has been loaded successfully");
	}
	
	private static void TryRegisterGodotScriptAssembly()
	{
		try
		{
			Assembly modAssembly = typeof(StsBossAncientsInit).Assembly;
			Type? scriptManagerBridgeType = Type.GetType("Godot.Bridge.ScriptManagerBridge, GodotSharp");

			if (scriptManagerBridgeType == null)
			{
				return;
			}

			MethodInfo? lookupMethod = scriptManagerBridgeType.GetMethod(
				"LookupScriptsInAssembly",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
				binder: null,
				types: [typeof(Assembly)],
				modifiers: null
			);

			lookupMethod ??= scriptManagerBridgeType
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.FirstOrDefault(m =>
				{
					ParameterInfo[] ps = m.GetParameters();
					return ps.Length == 1
						&& ps[0].ParameterType == typeof(Assembly)
						&& (m.Name.Contains("Lookup", StringComparison.OrdinalIgnoreCase)
							|| m.Name.Contains("Load", StringComparison.OrdinalIgnoreCase)
							|| m.Name.Contains("Register", StringComparison.OrdinalIgnoreCase));
				});

			lookupMethod?.Invoke(null, [modAssembly]);
		}
		catch (Exception e)
		{
			Log.Error($"Failed to register Godot scripts for StsBossAncients mod: {e}");
		}
	}
	}

	

	

	
}
