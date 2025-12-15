using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using TMPro;
using UnityEngine;

namespace ZenithX
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShower_Start
    {
        public static void Postfix(VersionShower __instance)
        {
            try
            {
                if (__instance == null || __instance.text == null) return;

                var tmp = __instance.text;
                tmp.richText = true;
                tmp.color = new Color(0.4f, 0.6f, 1f, 1f);
                tmp.outlineColor = new Color(0f, 1f, 0.55f, 1f);
                tmp.outlineWidth = 0.2f;

                try { ClassInjector.RegisterTypeInIl2Cpp<VersionShowerFx>(); } catch { }

                string originalText = tmp.text;
                string oblivionText = ZenithX.supportedAU.Contains(Application.version)
                    ? $"ZenithX v{ZenithX.ZenithXVersion} (v{Application.version})"
                    : $"ZenithX v{ZenithX.ZenithXVersion} (<color=red>v{Application.version}</color>)";

                string modText = $"{originalText} | {oblivionText}";

                var fx = __instance.gameObject.GetComponent<VersionShowerFx>();
                if (fx == null) fx = __instance.gameObject.AddComponent<VersionShowerFx>();
                fx.Initialize(tmp, modText);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ZenithX] VersionShower_Start error: {ex}");
            }
        }
    }

    public class VersionShowerFx : MonoBehaviour
    {
        private TextMeshPro _text;
        private string _modText;
        private bool _isEffectRunning;
        private Vector2 _baseAnchoredPosition;
        private float _baseScale;
        private Color _baseColor;
        private Color _baseOutlineColor;
        private float _baseOutlineWidth;
        private static readonly WaitForSeconds sWaitFrame = new WaitForSeconds(0.016f);

        public void Initialize(TextMeshPro text, string modText)
        {
            _text = text;
            _modText = modText;

            _baseAnchoredPosition = _text.rectTransform.anchoredPosition;
            _baseScale = _text.rectTransform.localScale.x;
            _baseColor = _text.color;
            _baseOutlineColor = _text.outlineColor;
            _baseOutlineWidth = _text.outlineWidth;

            if (!_isEffectRunning)
            {
                _isEffectRunning = true;
                StartCoroutine(IdleBreathing().WrapToIl2Cpp());
            }

            _text.text = _modText;
        }

        private IEnumerator IdleBreathing()
        {
            while (_isEffectRunning)
            {
                if (_text == null) yield break;

                var rt = _text.rectTransform;
                float t = Time.time;
                float angle = Mathf.Sin(t * 3f) * 0.4f;
                rt.localRotation = Quaternion.Euler(0, 0, angle);
                rt.localScale = new Vector3(_baseScale, _baseScale, 1f);
                rt.anchoredPosition = _baseAnchoredPosition;

                yield return sWaitFrame;
            }
        }

        private void ResetVisualsToStable()
        {
            if (_text == null) return;

            var rt = _text.rectTransform;
            rt.anchoredPosition = _baseAnchoredPosition;
            rt.localRotation = Quaternion.identity;
            rt.localScale = new Vector3(_baseScale, _baseScale, 1f);
            _text.color = _baseColor;
            _text.outlineColor = _baseOutlineColor;
            _text.outlineWidth = _baseOutlineWidth;

            _text.text = _modText;
        }

        private void OnDisable()
        {
            _isEffectRunning = false;
            StopAllCoroutines();
            if (_text != null && gameObject.activeInHierarchy) ResetVisualsToStable();
        }

        private void OnDestroy() => OnDisable();
    }
}