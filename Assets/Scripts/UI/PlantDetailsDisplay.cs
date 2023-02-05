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

using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.IntoTheRoots
{
    public class PlantDetailsDisplay : MonoBehaviour
    {

        /// <summary>
        /// Singleton plants display, which will be accessed when a new plant is selected.
        /// </summary>
        public static PlantDetailsDisplay Singleton { get; private set; }

        /// <summary>
        /// TMP for plant name.
        /// </summary>
        public TextMeshProUGUI plantName;

        /// <summary>
        /// TMP for plant description.
        /// </summary>
        public TextMeshProUGUI description;

        /// <summary>
        /// TMP for seed cost and seed gain.
        /// </summary>
        public TextMeshProUGUI seedCost, seedGain;

        /// <summary>
        /// TMP for sun cost and sun gain.
        /// </summary>
        public TextMeshProUGUI sunCost, sunGain;

        /// <summary>
        /// TMP for water cost and water gain.
        /// </summary>
        public TextMeshProUGUI waterCost, waterGain;

        /// <summary>
        /// TMP for ticks per second & VP amount
        /// </summary>
        public TextMeshProUGUI produceInterval, vpAmount;

        /// <summary>
        /// Images for resource types.
        /// </summary>
        public Image seedSprite, sunSprite, waterSprite;

        private Color orange;

        public void Awake()
        {
            Singleton = this;
            orange = Color.Lerp(Color.red, Color.yellow, 0.5f);
        }

        public static void UpdateDisplay(Plants.Plant selectedPlant)
        {
            if (Singleton)
            {
                Singleton.seedCost.text = "" + selectedPlant.cost.Seed;
                Singleton.sunCost.text = "" + selectedPlant.cost.Sun;
                Singleton.waterCost.text = "" + selectedPlant.cost.Water;

                Singleton.seedGain.text = "" + selectedPlant.production.Seed / selectedPlant.produceInterval;
                Singleton.sunGain.text = "" + selectedPlant.production.Sun / selectedPlant.produceInterval;
                Singleton.waterGain.text = "" + selectedPlant.production.Water / selectedPlant.produceInterval;

                Singleton.vpAmount.text = "" + selectedPlant.victoryPoints;
                Singleton.produceInterval.text = "" + selectedPlant.produceInterval;

                Singleton.description.text = selectedPlant.plantDescription;
                Singleton.plantName.text = selectedPlant.GetComponent<NetworkBehaviour>().name;
            }
        }

        private void SetResoureAsNeeded(Image resourceImage, bool isNeeded)
        {
            if (isNeeded)
            {
                resourceImage.color = orange;
            }
            else
            {
                resourceImage.color = Color.white;
            }
        }

        public void SetResourceHighlight(Plants.Resource resource, bool isNeeded)
        {
            switch (resource)
            {
                case Plants.Resource.Seeds:
                    SetResoureAsNeeded(seedSprite, isNeeded);
                    break;
                case Plants.Resource.Sun:
                    SetResoureAsNeeded(sunSprite, isNeeded);
                    break;
                case Plants.Resource.Water:
                    SetResoureAsNeeded(waterSprite, isNeeded);
                    break;
                default:
                    break;
            }
        }
    }
}
