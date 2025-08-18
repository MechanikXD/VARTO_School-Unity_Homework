using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio {
    [CreateAssetMenu(fileName = "AudioSourceSettings", menuName = "Scriptable Objects/AudioSourceSettings")]
    public class AudioSourceSettings : ScriptableObject {
        [Header("General")]
        [SerializeField] private string _name;
        [SerializeField] private AudioMixerGroup _group;
        [SerializeField] private bool _loop;
        [SerializeField] private bool _is3D;
        [Header("3D Audio Settings")]
        [SerializeField] private float _minHearDistance;
        [SerializeField] private float _maxHearDistance;
        [SerializeField] private AudioRolloffMode _rolloff;

        public string Name => _name;
        public AudioMixerGroup MixerGroup => _group;
        public bool IsLoop => _loop;
        public bool Is3D => _is3D;

        public AudioSource CreateAudioSource() {
            var audioSource = new GameObject().AddComponent<AudioSource>();
            
            audioSource.name = _name;
            audioSource.outputAudioMixerGroup = _group;
            audioSource.playOnAwake = false;
            audioSource.loop = _loop;
            audioSource.spatialBlend = _is3D ? 1 : 0;

            if (_is3D) {
                audioSource.minDistance = _minHearDistance;
                audioSource.maxDistance = _maxHearDistance;
                audioSource.rolloffMode = _rolloff;
            }

            return audioSource;
        }
        
        public AudioSource CreateAudioSource(AudioClip clip) {
            var audioSource = CreateAudioSource();
            audioSource.clip = clip;
            return audioSource;
        }
    }
}