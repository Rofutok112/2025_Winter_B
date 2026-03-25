using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Projects.Scripts.Audio
{
    public class AudioSubscriber : MonoBehaviour
    {
        [Serializable]
        private class AudioClipEntry
        {
            public string key;
            public AudioClip clip;
        }
        
        [SerializeField] private AudioClipEntry[] audioClipEntries;
        
        private void Awake()
        {
            foreach (var entry in audioClipEntries)
            {
                if (entry.clip == null) continue;
                AudioManager.Register(entry.key, entry.clip);
            }
        }
    }
}