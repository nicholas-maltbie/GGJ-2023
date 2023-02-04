
using System;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plant
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceScriptableObject", order = 1)]
    public class ResourceSprites : ScriptableObject
    {
        public Sprite errorIcon;
        public Sprite waterIcon;
        public Sprite sunIcon;

        public Sprite GetIcon(Resource resource)
        {
            switch(resource)
            {
                case Resource.Sun:
                    return sunIcon;
                case Resource.Water:
                    return waterIcon;
                default:
                    return errorIcon;
            }
        }
    }
}
