using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "ScriptableObjects/SoundLibrary", order = 1)]
    public class SoundLibrary : ScriptableObject
    {
        public AudioClip victoryClip;
        public AudioClip failureClip;

        public AudioClip placementClipLight;
        public AudioClip placementClipHefty;
        public AudioClip placementClipHeavy;

        public AudioClip? GetClip(string clipName)
        {
            switch (clipName)
            {
                case "victoryClip":
                    return victoryClip;
                case "failureClip":
                    return failureClip;
                case "placementClipLight":
                    return placementClipLight;
                case "placementClipHefty":
                    return placementClipHefty;
                case "placmentClipHeavy":
                    return placementClipHeavy;
                default:
                    return null;
            }
        }
    }
}
