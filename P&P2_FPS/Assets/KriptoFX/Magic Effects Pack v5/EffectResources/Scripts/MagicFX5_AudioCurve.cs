using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MagicFX5
{
    public class MagicFX5_AudioCurve : MagicFX5_IScriptInstance
    {
        public bool           UseVolumeCurve   = true;
        public bool           UsePitchCurve   = false;
        public AnimationCurve VolumeOverTime   = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve PitchOverTime    = AnimationCurve.EaseInOut(0, 1, 1, 1);
        public float          StartDelay       = 0;
        public float          Duration         = 2;
        public bool           Loop             = false;
        public bool           AutoDeactivation = true;

        [Space]
        public bool UseRandomPitch = false;
        public float PitchOffset = 0.15f;

        [Space] 
        public float AudioClipStartTimeOffset = 0;

        [Space] 
        public AudioClip[] RandomClips;

        private AudioSource _audioSource;
        private float       _leftTime;
        private float       _startVolume;
        private float       _startPitch;
        private bool        _canUpdate;


        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        internal override void OnEnableExtended()
        {
            _leftTime    = -StartDelay;
            _startVolume = _audioSource.volume;
            _startPitch  = _audioSource.pitch;

            if (RandomClips.Length > 0)
            {
                _audioSource.clip = RandomClips[Random.Range(0, RandomClips.Length - 1)];
                _audioSource.PlayDelayed(StartDelay);
            }

            _audioSource.time = AudioClipStartTimeOffset;

            if (StartDelay > 0)
            {
                Debug.Assert(!_audioSource.playOnAwake, "playOnAwake must be disabled using start delay");
                _audioSource.PlayDelayed(StartDelay);
            }

            if (UseVolumeCurve)
            {
                _audioSource.enabled = true;
                _audioSource.volume  = VolumeOverTime.Evaluate(0);
                _canUpdate           = true;
            }
            if (UsePitchCurve)
            {
                _audioSource.enabled = true;
                _audioSource.pitch   = PitchOverTime.Evaluate(0);
                _canUpdate           = true;
            }

            if (UseRandomPitch && !UsePitchCurve) _audioSource.pitch = _startPitch + Random.Range(-PitchOffset, PitchOffset);
        }

        internal override void OnDisableExtended()
        {
            _audioSource.volume = _startVolume;
            _audioSource.pitch  = _startPitch;
        }

        internal override void ManualUpdate()
        {
            if (!_canUpdate) return;

            _leftTime += Time.deltaTime;
            if (Loop) _leftTime %= Duration;

            if (UseVolumeCurve) _audioSource.volume = VolumeOverTime.Evaluate(_leftTime / Duration) * _startVolume;
            if (UsePitchCurve) _audioSource.pitch   = PitchOverTime.Evaluate(_leftTime  / Duration) * _startPitch;

            if (!Loop && _leftTime > Duration)
            {
                _canUpdate = true;
                if (AutoDeactivation) _audioSource.enabled = false;
            }
        }
    }
}