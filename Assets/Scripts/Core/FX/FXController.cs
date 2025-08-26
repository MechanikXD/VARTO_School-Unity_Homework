using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Core.FX {
    public class FXController : MonoBehaviour {
        [SerializeField] private Volume _globalVolume;
        private VolumeProfile _profile;
        private Dictionary<Type, VolumeComponent> _cachedComponents;
        
        [SerializeField] private Button _playEffectButton;
        [SerializeField] private Color _damagedColor;
        [SerializeField] private float _vignetteIntensity;
        [SerializeField] private float _effectDuration;
        private bool _isPlaying;

        private void Awake() => Initialize();

        private void OnDisable() => RemoveAllListeners();
        
        private void Initialize() {
            _cachedComponents = new Dictionary<Type, VolumeComponent>();
            _profile = _globalVolume.profile;
            _playEffectButton.onClick.AddListener(PlayDamagedEffect);
        }

        private void RemoveAllListeners() {
            _playEffectButton.onClick.RemoveAllListeners();
        }

        public T GetOrAdd<T>() where T : VolumeComponent {
            if (_cachedComponents.ContainsKey(typeof(T))) return (T)_cachedComponents[typeof(T)];

            if (!_profile.TryGet<T>(out var component)) {
                component = _profile.Add<T>();
            }
            
            _cachedComponents.Add(typeof(T), component);
            return component;
        }

        public void SetVignette(Color color, float intensity) {
            var vignette = GetOrAdd<Vignette>();
            vignette.color.Override(color);
            vignette.intensity.Override(intensity);
            vignette.active = true;
        }

        private void PlayDamagedEffect() {
            if (_isPlaying) return;
            StartCoroutine(DamagedEffect());
        }
        
        private IEnumerator DamagedEffect() {
            var vignette = GetOrAdd<Vignette>();
            var originalColor = vignette.color.value;
            var originalIntensity = vignette.intensity.value;

            var dist = _vignetteIntensity - originalIntensity;
            
            SetVignette(_damagedColor, _vignetteIntensity);
            var elapsed = 0f;
            while (elapsed < _effectDuration) {
                elapsed += Time.deltaTime;
                var invElapsed = 1f - Mathf.Clamp01(elapsed / _effectDuration);
                
                var currentIntensity = originalIntensity + dist * invElapsed;
                var currentColor = Color.Lerp(originalColor, _damagedColor, invElapsed);

                SetVignette(currentColor, currentIntensity);
                yield return null;
            }

            // вимкнемо побічні ефекти
            SetVignette(originalColor, originalIntensity);

            _isPlaying = false;
        }
    }
}