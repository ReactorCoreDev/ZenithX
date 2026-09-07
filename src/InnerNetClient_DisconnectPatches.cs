using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch]
public static class InnerNetClient_DisconnectPatches
{
	private static IEnumerable<MethodBase> TargetMethods()
	{
		yield return AccessTools.Method(typeof(InnerNetClient), "EnqueueDisconnect", (Type[])null, (Type[])null);
		yield return AccessTools.Method(typeof(InnerNetClient), "DisconnectInternal", (Type[])null, (Type[])null);
	}

	public static void Prefix()
	{
		Utils.GameLoaded = false;
	}
}
