using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenithX;

public class CoroutineRunner : MonoBehaviour
{
	public static CoroutineRunner Instance;

	public Dictionary<string, AudioSource> ClipSources = new Dictionary<string, AudioSource>();

	public string ClipPath;

	public string ClipExt;

	private void Awake()
	{
		if ((Object)(object)Instance == (Object)null)
		{
			Instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	public void AddClip(string name, AudioClip clip)
	{
		if (!ClipSources.ContainsKey(name))
		{
			AudioSource val = ((Component)this).gameObject.AddComponent<AudioSource>();
			val.clip = clip;
			ClipSources.Add(name, val);
		}
	}

	public void PlayClip(string name, float volume)
	{
		if (ClipSources.ContainsKey(name))
		{
			ClipSources[name].volume = volume;
			ClipSources[name].Play();
		}
		else
		{
			ZenithX.Warning("[AudioClipContainer] Missing clip: " + name);
		}
	}

	[HideFromIl2Cpp]
	public IEnumerator LoadClipCoroutine()
	{
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + ClipPath, (AudioType)0);
		yield return www.SendWebRequest();
		if ((int)www.result != 1)
		{
			ZenithX.Error($"[ZenithXSoundManager] Failed to load {ClipExt} {ClipPath}: {www.error}");
		}
		else
		{
			AudioClip content = DownloadHandlerAudioClip.GetContent(www);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ClipPath);
			if (!ZenithXSoundManager.LoadedSounds.ContainsKey(fileNameWithoutExtension))
			{
				ZenithXSoundManager.LoadedSounds.Add(fileNameWithoutExtension, content);
			}
			AddClip(fileNameWithoutExtension, content);
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("[ZenithXSoundManager] " + ClipExt.ToUpper() + " loaded: " + ClipPath);
			}
		}
		www.Dispose();
	}
}
