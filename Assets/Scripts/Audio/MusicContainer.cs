using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace nickmaltbie.IntoTheRoots
{
    public class MusicContainer : MonoBehaviour
    {
        public AudioMixer mixer;

        // Start is called before the first frame update
        private void Start()
        {
            if (FindObjectsOfType<MusicContainer>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            float musicVol = PlayerPrefs.GetFloat("Music", .5f);
            mixer.SetFloat("Music Volume", Mathf.Log10(musicVol) * 20);
        }
    }
}
