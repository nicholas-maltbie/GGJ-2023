
using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class GhostCheck : NetworkBehaviour
    {
        public Color validColor = Color.green;
        public Color invalidColor = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        public Color plantColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);

        public Sprite circleImage;

        private Plant Plant => GetComponent<Planter>().toPlant;
        private SpriteRenderer ghostSprite;
        private SpriteRenderer plantSprite;

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
            }
        }

        public void Update()
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
            }
        }
    }
}