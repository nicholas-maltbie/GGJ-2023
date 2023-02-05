using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudio : MonoBehaviour
    {
        public static GlobalAudio singleton;
        public SoundLibrary soundLibrary;
        void Start()
        {
            if (FindObjectsOfType<GlobalAudio>().Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                singleton = this;
            }
        }

        public void PlayClip(string clipName)
        {
            AudioClip clip = soundLibrary.GetClip(clipName);
            if (clip)
            {
                singleton.GetComponent<AudioSource>().PlayOneShot(clip);
            }
        }

    }
}
