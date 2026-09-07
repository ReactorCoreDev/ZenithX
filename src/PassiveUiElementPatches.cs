using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch]
public static class PassiveUiElementPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(PassiveButton), "ReceiveClickDown")]
	[HarmonyPatch(typeof(PassiveButton), "ReceiveClickUp")]
	[HarmonyPatch(typeof(PassiveButton), "ReceiveMouseOver")]
	[HarmonyPatch(typeof(GameOptionButton), "ReceiveClickDown")]
	[HarmonyPatch(typeof(GameOptionButton), "ReceiveClickUp")]
	[HarmonyPatch(typeof(GameOptionButton), "ReceiveMouseOver")]
	[HarmonyPatch(typeof(SlideBar), "ReceiveClickDrag")]
	[HarmonyPatch(typeof(Scrollbar), "ReceiveClickDrag")]
	[HarmonyPatch(typeof(Scroller), "UpdateScrollBars")]
	public static bool Prefix()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mouseGuiPosition = MenuUI.GetMouseGuiPosition(new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y));
		if ((!MenuUI.isGUIActive || !((Rect)(ref MenuUI.WindowRect)).Contains(mouseGuiPosition)) && (!CheatToggles.showConsoleMenu || !((Rect)(ref ConsoleUI.WindowRect)).Contains(mouseGuiPosition)) && (!CheatToggles.showDoorsMenu || !((Rect)(ref DoorsUI.WindowRect)).Contains(mouseGuiPosition)) && (!CheatToggles.showProtectMenu || !((Rect)(ref OverloadUI.WindowRect)).Contains(mouseGuiPosition)) && (!CheatToggles.showProtectMenu || !((Rect)(ref ProtectUI.WindowRect)).Contains(mouseGuiPosition)) && (!CheatToggles.showRolesMenu || !((Rect)(ref RolesUI.WindowRect)).Contains(mouseGuiPosition)))
		{
			if (CheatToggles.showTasksMenu)
			{
				return !((Rect)(ref TasksUI.WindowRect)).Contains(mouseGuiPosition);
			}
			return true;
		}
		return false;
	}
}
