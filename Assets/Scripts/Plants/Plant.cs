// Copyright (C) 2023 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using nickmaltbie.IntoTheRoots.Player;
using nickmaltbie.IntoTheRoots.UI;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public enum PlantType
    {
        None,
        Producer,
        Tree,
        Vegetable,
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Plant : NetworkBehaviour
    {
        /// <summary>
        /// Circle mask for roots layer.
        /// </summary>
        [SerializeField]
        public Sprite rootsMask;

        /// <summary>
        /// Name of this type of plant.
        /// </summary>
        [SerializeField]
        public PlantType plantType;

        /// <summary>
        /// Cost to place the plant.
        /// </summary>
        [SerializeField]
        public ResourceValues cost;

        /// <summary>
        /// Production of the plant.
        /// </summary>
        [SerializeField]
        public ResourceValues production;

        /// <summary>
        /// Interval between producing resources.
        /// </summary>
        [SerializeField]
        public float produceInterval = 1.0f;

        /// <summary>
        /// Restricted radius for building around this plant.
        /// </summary>
        [SerializeField]
        public int restrictedDistance = 2;

        /// <summary>
        /// Victory points for this plant type.
        /// </summary>
        [SerializeField]
        public int victoryPoints = 1;

        /// <summary>
        /// Grow range for improving the range of this plant.
        /// </summary>
        [SerializeField]
        public int growRange = 0;

        /// <summary>
        /// Resource Sprites for getting resource icons
        /// </summary>
        public ResourceSprites resourceSprites;

        /// <summary>
        /// Elapsed time since this plant has produced anything.
        /// </summary>
        private float elapsedSinceProduced;

        /// <summary>
        ///  Resource Particle System
        /// </summary>
        private ParticleSystem resourceParticle;

        /// <summary>
        /// Restricted area for drawing roots.
        /// </summary>
        private SpriteMask restrictedArea;

        /// <summary>
        /// Area in which the player can grow plants.
        /// </summary>
        private SpriteMask growArea;

        /// <summary>
        /// String description of this plant's costs and benefits.
        /// </summary>
        public string plantDescription;

        /// <summary>
        /// Get the radius of this object
        /// </summary>
        /// <returns></returns>
        public float Radius()
        {
            if (GetComponent<Collider2D>() is Collider2D collider)
            {
                if (collider is CircleCollider2D circle)
                {
                    return circle.radius;
                }
                else
                {
                    return collider.bounds.extents.magnitude;
                }
            }
            else
            {
                return 0.0f;
            }
        }

        public void SetRestrictedAreaVisible(bool visible)
        {
            restrictedArea.enabled = visible;
        }

        public void Start()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Collider2D collider = GetComponent<Collider2D>();
            sr.sortingOrder = -Mathf.RoundToInt(collider.bounds.min.y * 100);
            resourceParticle = GetComponent<ParticleSystem>();

            if (IsOwner)
            {
                VisualizeRange();
                SetRestrictedAreaVisible(PlantIconTableau.Singleton.SelectedType() == plantType);
            }
        }

        public void VisualizeRange()
        {
            restrictedArea = new GameObject().AddComponent<SpriteMask>();
            restrictedArea.transform.SetParent(transform);
            restrictedArea.transform.localPosition = Vector3.zero;

            float diameter = restrictedDistance * 2;
            restrictedArea.sprite = rootsMask;
            restrictedArea.transform.localScale = new Vector3(diameter, diameter, 1);
            restrictedArea.renderingLayerMask = RenderLayerMasks.RestrictedRootsRenderLayer;
            restrictedArea.isCustomRangeActive = true;
            restrictedArea.frontSortingLayerID = RenderLayerMasks.RestrictedRangeSortingLayer;
            restrictedArea.backSortingLayerID = RenderLayerMasks.RestrictedRangeSortingLayer;
            restrictedArea.backSortingOrder = -1;

            if (growRange > 0)
            {
                growArea = new GameObject().AddComponent<SpriteMask>();
                growArea.transform.SetParent(transform);
                growArea.transform.localPosition = Vector3.zero;
                float growDiameter = growRange * 2;

                growArea.sprite = rootsMask;
                growArea.transform.localScale = new Vector3(growDiameter, growDiameter, 1);
                growArea.renderingLayerMask = RenderLayerMasks.GrowRangeRenderLayer;

                growArea.isCustomRangeActive = true;
                growArea.frontSortingLayerID = RenderLayerMasks.GrowRangeSortingLayer;
                growArea.backSortingLayerID = RenderLayerMasks.GrowRangeSortingLayer;
                growArea.backSortingOrder = -1;
            }
        }

        public void Update()
        {
            if (!IsServer)
            {
                return;
            }

            elapsedSinceProduced += Time.deltaTime;

            while (elapsedSinceProduced > produceInterval)
            {
                Produce();
                elapsedSinceProduced -= produceInterval;
            }
        }

        public void Produce()
        {
            var resources = PlayerResources.GetResources(OwnerClientId);

            if (resources == null)
            {
                return;
            }

            foreach ((Resource, int) produced in production.EnumerateResources())
            {
                resources.AddResources(produced.Item1, produced.Item2);
                //Produce corresponding resource particle effect
                ParticleSystem.TextureSheetAnimationModule ts = resourceParticle.textureSheetAnimation;
                ts.SetSprite(0, resourceSprites.GetIcon(produced.Item1));
                resourceParticle.Play();
            }
        }
    }
}
