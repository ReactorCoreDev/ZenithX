using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenithX;

public class CoroutineRunner : MonoBehaviour
    {
        public static CoroutineRunner Instance;
        public Dictionary<string, AudioSource> ClipSources = new Dictionary<string, AudioSource>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddClip(string name, AudioClip clip)
        {
            if (!ClipSources.ContainsKey(name))
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.clip = clip;
                ClipSources.Add(name, src);
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
                Debug.LogWarning($"[AudioClipContainer] Missing clip: {name}");
            }
        }

        public string ClipPath;
        public string ClipExt;

        public IEnumerator LoadClipCoroutine()
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + ClipPath, AudioType.UNKNOWN);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[ZenithXSoundManager] Failed to load {ClipExt} {ClipPath}: {www.error}");
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                string key = Path.GetFileNameWithoutExtension(ClipPath);
                if (!ZenithXSoundManager.LoadedSounds.ContainsKey(key))
                    ZenithXSoundManager.LoadedSounds.Add(key, clip);

                AddClip(key, clip);

                Debug.Log($"[ZenithXSoundManager] {ClipExt.ToUpper()} loaded: {ClipPath}");
            }

            www.Dispose();
        }
    }