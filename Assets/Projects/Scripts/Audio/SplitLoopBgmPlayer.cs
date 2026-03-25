using UnityEngine;

namespace Projects.Scripts.Audio
{
    /// <summary>
    /// イントロ用クリップを1回再生し、その後ループ用クリップへ切り替えて再生する BGM プレイヤー。
    /// </summary>
    [DisallowMultipleComponent]
    public class SplitLoopBgmPlayer : MonoBehaviour
    {
        [Header("Clips")]
        [SerializeField] private AudioClip introClip;
        [SerializeField] private AudioClip loopClip;

        [Header("Playback")]
        [SerializeField] private bool playOnAwake = true;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        private AudioSource _introSource;
        private AudioSource _loopSource;
        private bool _isPaused;

        private void Awake()
        {
            _introSource = CreateSource("IntroSource", false);
            _loopSource = CreateSource("LoopSource", true);
            ApplyVolume();
        }

        private void Start()
        {
            if (!playOnAwake)
            {
                return;
            }

            Play();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            _isPaused = false;

            if (loopClip == null && introClip == null)
            {
                Debug.LogWarning($"{nameof(SplitLoopBgmPlayer)} failed: no clips assigned.", this);
                return;
            }

            StopSources();
            ApplyVolume();

            if (introClip != null && loopClip != null)
            {
                var startTime = AudioSettings.dspTime + 0.05d;

                _introSource.clip = introClip;
                _loopSource.clip = loopClip;
                _loopSource.loop = true;

                _introSource.PlayScheduled(startTime);
                _loopSource.PlayScheduled(startTime + introClip.length);
                return;
            }

            if (introClip != null)
            {
                _introSource.clip = introClip;
                _introSource.loop = false;
                _introSource.Play();
                return;
            }

            _loopSource.clip = loopClip;
            _loopSource.loop = true;
            _loopSource.Play();
        }

        public void Stop()
        {
            _isPaused = false;
            StopSources();
        }

        public void Pause()
        {
            if (_isPaused)
            {
                return;
            }

            _introSource.Pause();
            _loopSource.Pause();
            _isPaused = true;
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                return;
            }

            _introSource.UnPause();
            _loopSource.UnPause();
            _isPaused = false;
        }

        public void SetVolume(float value)
        {
            volume = Mathf.Clamp01(value);
            ApplyVolume();
        }

        private AudioSource CreateSource(string childName, bool shouldLoop)
        {
            var child = FindDirectChild(childName);
            GameObject childObject;

            if (child == null)
            {
                childObject = new GameObject(childName);
                childObject.transform.SetParent(transform, false);
            }
            else
            {
                childObject = child.gameObject;
            }

            var source = childObject.GetComponent<AudioSource>();
            if (source == null)
            {
                source = childObject.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.loop = shouldLoop;
            return source;
        }

        private Transform FindDirectChild(string childName)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }

        private void ApplyVolume()
        {
            if (_introSource != null)
            {
                _introSource.volume = volume;
            }

            if (_loopSource != null)
            {
                _loopSource.volume = volume;
            }
        }

        private void StopSources()
        {
            _introSource.Stop();
            _introSource.clip = null;
            _introSource.loop = false;

            _loopSource.Stop();
            _loopSource.clip = null;
            _loopSource.loop = true;
        }
    }
}
