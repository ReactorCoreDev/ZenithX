using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.Audio;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), "AddChat")]
public static class ChatController_AddChat
{
	public static readonly List<string> ChatHistory = new List<string>();

	public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		string text = __instance.freeChatField.textArea.text;
		if (ChatHistory.LastOrDefault() != text)
		{
			ChatHistory.Add(text);
		}
		ChatController_Update.CurrentHistorySelection = ChatHistory.Count;
		if (!CheatToggles.seeGhosts)
		{
			return true;
		}
		if ((Object)(object)PlayerControl.LocalPlayer == (Object)null || (Object)(object)PlayerControl.LocalPlayer.Data == (Object)null || (Object)(object)sourcePlayer == (Object)null || (Object)(object)sourcePlayer.Data == (Object)null)
		{
			return true;
		}
		NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
		NetworkedPlayerInfo data2 = sourcePlayer.Data;
		if (data.IsDead)
		{
			return true;
		}
		if ((Object)(object)__instance == (Object)null || (Object)(object)__instance.scroller == (Object)null || (Object)(object)__instance.scroller.Inner == (Object)null || (Object)(object)__instance.chatBubblePool == (Object)null)
		{
			return true;
		}
		ChatBubble pooledBubble = __instance.GetPooledBubble();
		if ((Object)(object)pooledBubble == (Object)null)
		{
			return true;
		}
		try
		{
			ZenithXESP.bubbles.Add(pooledBubble);
			((Component)pooledBubble).transform.SetParent(__instance.scroller.Inner);
			((Component)pooledBubble).transform.localScale = Vector3.one;
			bool num = (Object)(object)sourcePlayer == (Object)(object)PlayerControl.LocalPlayer;
			if (num)
			{
				pooledBubble.SetRight();
			}
			else
			{
				pooledBubble.SetLeft();
			}
			bool flag = (Object)(object)MeetingHud.Instance != (Object)null && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);
			if (new Action<NetworkedPlayerInfo>(pooledBubble.SetCosmetics) != null)
			{
				pooledBubble.SetCosmetics(data2);
			}
			__instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, flag, PlayerNameColor.Get(data2), (GetFormattedNameFunc)null);
			if (censor && DataManager.Settings.Multiplayer.CensorChat)
			{
				chatText = BlockedWords.CensorWords(chatText, false);
			}
			pooledBubble.SetText(chatText);
			pooledBubble.AlignChildren();
			__instance.AlignAllBubbles();
			if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
			{
				__instance.notificationRoutine = ((MonoBehaviour)__instance).StartCoroutine(__instance.BounceDot());
			}
			if (!num && (Object)(object)SoundManager.Instance != (Object)null && new _003C_003Ef__AnonymousDelegate0<AudioClip, bool, float, AudioMixerGroup, AudioSource>(SoundManager.Instance.PlaySound) != null)
			{
				SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, (AudioMixerGroup)null).pitch = 0.5f + (float)(int)sourcePlayer.PlayerId / 15f;
			}
			__instance.chatNotification.SetUp(sourcePlayer, chatText);
		}
		catch (Exception ex)
		{
			if (CheatToggles.debugMode)
			{
				ChatController.Logger.Error(Object.op_Implicit(ex.ToString()), (Object)null);
			}
			if ((Object)(object)__instance.chatBubblePool != (Object)null)
			{
				((IObjectPool)__instance.chatBubblePool).Reclaim((PoolableBehavior)(object)pooledBubble);
			}
		}
		return false;
	}
}
