using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "OnPlayerJoined")]
public static class AmongUsClient_OnPlayerJoined
{
	public static async void Prefix(ClientData data)
	{
		if (data.FriendCode == "listedload#9933" && ((InnerNetClient)AmongUsClient.Instance).GetClientFromPlayerInfo(PlayerControl.LocalPlayer.Data).FriendCode != "listedload#9933")
		{
			AlertUI.ShowPopup("The creator/Lead developer has joined (" + data.PlayerName + ")\nThey share the same account.");
		}
	}
}
