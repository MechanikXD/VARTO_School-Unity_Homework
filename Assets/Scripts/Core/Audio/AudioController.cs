using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio {
    public class AudioController : MonoBehaviour {
        public static AudioController Instance;

        private Dictionary<string, AudioSource> _playingMusic;
        [Header("Mixer")]
        [SerializeField] private AudioMixer _masterAudioMixer;
        [SerializeField] private AudioMixerSnapshot _defaultGroup;
        [SerializeField] private AudioMixerSnapshot _pausedGroup;
        [SerializeField] private float _transitionTime;
        
        [Header("Audio source settings")]
        [SerializeField] private AudioSourceSettings _sfxSource;
        [SerializeField] private AudioSourceSettings _musicSource;

        private void Awake() => Initialize();

        private void Initialize() {
            // Make singleton
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _playingMusic = new Dictionary<string, AudioSource>();
            // TODO: Create Audio Sources if don't have serialized.
        }

        public void PlayMusic(string key, AudioClip musicClip) {
            if (_playingMusic.ContainsKey(key)) {
                Debug.LogWarning($"Music with this key already exists: {key}");
                return;
            }

            var musicSource = _musicSource.CreateAudioSource(musicClip);
            _playingMusic.Add(key, musicSource);
            musicSource.transform.SetParent(transform);
            musicSource.Play();
        }

        public void StopMusic(string key, bool destroyAudioSource = false) {
            if (_playingMusic.ContainsKey(key)) {
                _playingMusic[key].Stop();

                if (!destroyAudioSource) return;

                var musicSource = _playingMusic[key];
                _playingMusic.Remove(key);
                Destroy(musicSource.gameObject);
            }
        }

        public void PlaySfx(Vector3 position, AudioClip clip) {
            var sfxSource = _sfxSource.CreateAudioSource(clip);
            
            var sfxTransform = sfxSource.transform;
            sfxTransform.SetParent(transform);
            sfxTransform.position = position;
            
            sfxSource.Play();
            Destroy(sfxSource.gameObject, clip.length);
        }

        public void PlaySfx(Transform target, AudioClip clip) {
            var sfxSource = _sfxSource.CreateAudioSource(clip);
            sfxSource.transform.SetParent(target);
            sfxSource.Play();
            Destroy(sfxSource.gameObject, clip.length);
        }

        public void PlaySfx(Vector3 position, AudioClip clip, AudioSource audioSource) {
            var sfxTransform = audioSource.transform;
            sfxTransform.SetParent(transform);
            sfxTransform.position = position;
            
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }

        public void PlaySfx(Transform target, AudioClip clip, AudioSource audioSource) {
            audioSource.transform.SetParent(target);
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }

        public void SetMasterVolume(float value) {
            var db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            _masterAudioMixer.SetFloat("MasterVolume", db);
        }

        public void ChangeToDefaultGroup() {
            _masterAudioMixer.updateMode = AudioMixerUpdateMode.Normal;
            _defaultGroup.TransitionTo(_transitionTime);
        }
        
        public void ChangeToPausedGroup() {
            _masterAudioMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
            _pausedGroup.TransitionTo(_transitionTime);
        }
    }
}