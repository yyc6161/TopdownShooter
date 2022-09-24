using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.CharacterControl
{
    public class AnimationEvent : MonoBehaviour
    {
        public Action OnHitAnimationFinshed; 
        
        [SerializeField] private AudioClip[] _footstepClips;
        [SerializeField] private AudioSource _audioSource;

        private AudioClip _lastPlayedAudioClip;
        
        public void PlayFootEffect()
        {
            PlayFootstepAudio();
        }

        public void HitAnimationFinshed()
        {
            OnHitAnimationFinshed?.Invoke();
        }

        private void PlayFootstepAudio()
        {
            if (_footstepClips == null || _footstepClips.Length == 0) return;
            
            var audioClip = _footstepClips[Random.Range(0, _footstepClips.Length)];
            
            // 如果有多个Clip可用，那么同一Clip不应连续播放两次。
            while (_footstepClips.Length > 1 && audioClip == _lastPlayedAudioClip) 
            {
                audioClip = _footstepClips[Random.Range(0, _footstepClips.Length)];
                if (audioClip == null) return;
            }
            
            _audioSource.clip = audioClip;
            _audioSource.Play();
            
            _lastPlayedAudioClip = audioClip;
        }
    }
}