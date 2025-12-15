using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenithX
{
    public static class ZenithXSoundManager
    {
        public static readonly Dictionary<string, AudioClip> LoadedSounds = new Dictionary<string, AudioClip>();
        private static CoroutineRunner _runner;

        public static void Initialize()
        {
            if (_runner == null)
            {
                GameObject runnerObj = new GameObject("CoroutineRunner");
                _runner = runnerObj.AddComponent<CoroutineRunner>();
                Object.DontDestroyOnLoad(runnerObj);
            }

            if (CoroutineRunner.Instance == null)
            {
                GameObject containerObj = new GameObject("ZenithXAudio");
                containerObj.AddComponent<CoroutineRunner>();
            }

            string basePath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "ZenithX",
                "Sounds"
            );

            Debug.Log($"[ZenithXSoundManager] Base path: {basePath}");

            if (!Directory.Exists(basePath))
            {
                Debug.LogWarning("[ZenithXSoundManager] Sounds folder does not exist.");
                return;
            }

            string[] files = Directory.GetFiles(basePath);
            Debug.Log($"[ZenithXSoundManager] Found {files.Length} files.");

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                Debug.Log($"[ZenithXSoundManager] Processing file: {file}, Extension: {ext}");

                if (ext != ".wav" && ext != ".mp3" && ext != ".ogg")
                {
                    Debug.Log($"[ZenithXSoundManager] Skipping unsupported file: {file}");
                    continue;
                }

                if (ext == ".wav")
                    LoadWav(file);
                else
                {
                    _runner.ClipPath = file;
                    _runner.ClipExt = ext;
                    _runner.StartCoroutine("LoadClipCoroutine");
                }
            }
        }

        private static void LoadWav(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                WAV wav = new WAV(data);
                AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path), wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
                clip.SetData(wav.LeftChannel, 0);

                string key = Path.GetFileNameWithoutExtension(path);
                if (!LoadedSounds.ContainsKey(key)) LoadedSounds.Add(key, clip);

                CoroutineRunner.Instance.AddClip(key, clip);

                Debug.Log($"[ZenithXSoundManager] WAV loaded: {path}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ZenithXSoundManager] Failed to load WAV {path}: {ex}");
            }
        }

        public static void PlaySound(string name, float volume)
        {
            if (!LoadedSounds.ContainsKey(name))
            {
                Debug.LogWarning($"[ZenithXSoundManager] Tried to play missing sound: {name}");
                return;
            }

            CoroutineRunner.Instance.AddClip(name, LoadedSounds[name]);
            CoroutineRunner.Instance.PlayClip(name, volume);
        }

        public struct WAV
        {
            public float[] LeftChannel;
            public int ChannelCount;
            public int SampleCount;
            public int Frequency;

            public WAV(byte[] wav)
            {
                ChannelCount = wav[22];
                Frequency = wav[24] | (wav[25] << 8);
                int pos = 12;

                while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
                {
                    int chunkSize = wav[pos + 4] | (wav[pos + 5] << 8);
                    pos += 8 + chunkSize;
                }

                pos += 8;
                SampleCount = (wav.Length - pos) / 2 / ChannelCount;
                LeftChannel = new float[SampleCount * ChannelCount];

                int i = 0;
                while (pos < wav.Length)
                {
                    for (int ch = 0; ch < ChannelCount; ch++)
                    {
                        short sample = (short)(wav[pos] | (wav[pos + 1] << 8));
                        LeftChannel[i++] = sample / 32768f;
                        pos += 2;
                    }
                }
            }
        }
    }
}