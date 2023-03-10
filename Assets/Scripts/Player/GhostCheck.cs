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

using System.Collections.Generic;
using System.Linq;
using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class GhostCheck : NetworkBehaviour
    {
        public const int MaxDegreeGhost = 25;

        public Color validColor = Color.green;
        public Color brokeColor = new Color(1.0f, 1.0f, 0.0f, 0.2f);
        public Color invalidColor = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        public Color overlapColor = new Color(1.0f, 0.0f, 0.0f, 0.2f);
        public Color plantColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);

        public Sprite circleImage;
        public Sprite rootSprite;

        private Plant Plant => GetComponent<Planter>().toPlant;
        private SpriteRenderer ghostSprite;
        private SpriteRenderer plantSprite;

        private List<GameObject> linkPool;

        public void Start()
        {
            if (IsLocalPlayer)
            {
                var spriteHolder = new GameObject();
                spriteHolder.transform.SetParent(transform);
                spriteHolder.transform.localPosition = Vector3.zero;
                plantSprite = spriteHolder.AddComponent<SpriteRenderer>();
                plantSprite.sprite = null;
                plantSprite.sortingLayerName = "Plant Ghost";
                plantSprite.sortingOrder = 1;
                plantSprite.enabled = true;
                plantSprite.color = plantColor;

                var ghostGo = new GameObject();
                ghostGo.transform.SetParent(transform);
                ghostGo.transform.localPosition = Vector3.zero;
                ghostSprite = ghostGo.AddComponent<SpriteRenderer>();
                ghostSprite.sprite = circleImage;
                ghostSprite.sortingLayerName = "Plant Ghost";
                ghostSprite.sortingOrder = 0;
                ghostSprite.enabled = false;

                linkPool = new List<GameObject>();

                for (int i = 0; i < MaxDegreeGhost; i++)
                {
                    var ghostLink = new GameObject();
                    ghostLink.transform.SetParent(transform);
                    ghostLink.transform.localPosition = Vector3.zero;
                    SpriteRenderer linkSr = ghostLink.AddComponent<SpriteRenderer>();
                    linkSr.sprite = rootSprite;
                    linkSr.tileMode = SpriteTileMode.Continuous;
                    linkSr.drawMode = SpriteDrawMode.Tiled;
                    linkSr.sortingLayerName = "Root Ghost";
                    linkSr.color = plantColor;
                    ghostLink.SetActive(false);
                    linkPool.Add(ghostLink);
                }
            }
        }

        public void LateUpdate()
        {
            if (!IsLocalPlayer)
            {
                return;
            }

            ghostSprite.enabled = Plant != null;

            if (Plant == null)
            {
                return;
            }

            foreach (GameObject go in linkPool)
            {
                go.SetActive(false);
            }

            // Get the radius of the plant
            float diameter = Plant.Radius() * 2;
            ghostSprite.transform.localScale = new Vector3(diameter, diameter, 1);

            plantSprite.sprite = Plant.GetComponent<SpriteRenderer>()?.sprite;

            Planter planter = GetComponent<Planter>();

            bool allowedPlacement = PlantUtils.IsPlantPlacementAllowed(transform.position, Plant, OwnerClientId);
            bool inGrowZone = PlantUtils.IsInGrowRadius(transform.position, Plant, OwnerClientId);
            if (allowedPlacement && inGrowZone)
            {
                ghostSprite.color = validColor;
            }
            else if (!allowedPlacement)
            {
                ghostSprite.color = invalidColor;

                IEnumerable<GameObject> overlapping = PlantUtils.GetPlantsAndRootsInRadius(transform.position, Plant.Radius());
                if (overlapping.Any())
                {
                    ghostSprite.color = overlapColor;
                }
            }

            // For each link within range, check if it will be created
            // Get all plants with a non-zero grow range owned
            // by this player
            float radius = Plant.Radius();
            Plant[] growZonesInRange = PlantUtils.GetPlantsOwnedByPlayer(OwnerClientId)
                .Where(plant => plant.growRange > 0)
                .Where(growZone =>
                {
                    float dist = Vector2.Distance(transform.position, growZone.transform.position);
                    bool inRange = dist <= growZone.growRange + radius;
                    return inRange;
                }).ToArray();

            // For each grow zone in range, draw a line to check if a
            // root can be created
            int linkAvailable = 0;
            bool allOverlap = true;
            for (int i = 0; i < Mathf.Min(linkPool.Count, growZonesInRange.Length); i++)
            {
                Plant growZone = growZonesInRange[i];
                bool connected = PlantUtils.CanDrawRootToPlant(transform.position, growZone, out RaycastHit2D hit);

                // Create a ghost root graphic between the plant and the grow zone.
                GameObject link = linkPool[i];
                link.SetActive(true);
                SpriteRenderer linkSr = link.GetComponent<SpriteRenderer>();
                Root.ShapePath(gameObject, growZone.gameObject, link, linkSr);

                if (connected)
                {
                    linkSr.color = validColor;
                    linkAvailable++;
                    allOverlap = false;
                }
                else
                {
                    linkSr.color = overlapColor;
                }
            }

            if (linkAvailable == 0)
            {
                ghostSprite.color = invalidColor;
            }

            if (growZonesInRange.Length > 0 && allOverlap)
            {
                ghostSprite.color = overlapColor;
            }

            if (ghostSprite.color == validColor && !planter.HasResourcesForPlant(Plant))
            {
                ghostSprite.color = brokeColor;
            }
        }
    }
}
