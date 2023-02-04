
using nickmaltbie.IntoTheRoots;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public enum RootType
    {
        Restricted,
        Grow,
        Linking
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class RootsArea : MonoBehaviour
    {
        public RootType rootType;

        public void Awake()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            switch (rootType)
            {
                case RootType.Restricted:
                    sr.renderingLayerMask = RenderLayerMasks.RestrictedRootsRenderLayer;
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    break;
            }
        }
    }
}