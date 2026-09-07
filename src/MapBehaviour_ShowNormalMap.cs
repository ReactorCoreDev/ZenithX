using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(MapBehaviour), "ShowNormalMap")]
public static class MapBehaviour_ShowNormalMap
{
	public static void Postfix(MapBehaviour __instance)
	{
		MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();
		if (!MinimapHandler.minimapActive)
		{
			return;
		}
		__instance.DisableTrackerOverlays();
		foreach (HerePoint herePoint in MinimapHandler.herePoints)
		{
			object obj;
			if (herePoint == null)
			{
				obj = null;
			}
			else
			{
				SpriteRenderer sprite = herePoint.sprite;
				obj = ((sprite != null) ? ((Component)sprite).gameObject : null);
			}
			if ((Object)obj != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)herePoint.sprite).gameObject);
			}
		}
		MinimapHandler.herePoints.Clear();
		List<HerePoint> list = new List<HerePoint>();
		Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			PlayerControl current2 = enumerator2.Current;
			if (!((InnerNetObject)current2).AmOwner)
			{
				SpriteRenderer sprite2 = Object.Instantiate<SpriteRenderer>(__instance.HerePoint, ((Component)__instance.HerePoint).transform.parent);
				list.Add(new HerePoint(current2, sprite2));
			}
		}
		MinimapHandler.herePoints = list;
	}
}
