using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots
{
    public class PlantDetailsDisplay : MonoBehaviour
    {

        /// <summary>
        /// Singleton plants display, which will be accessed when a new plant is selected.
        /// </summary>
        public static PlantDetailsDisplay singleton;

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

        private void Awake()
        {
            singleton = this;
        }

        public static void UpdateDisplay(Plants.Plant selectedPlant)
        {
            if (singleton)
            {
                singleton.seedCost.text = "" + selectedPlant.cost.Seed;
                singleton.sunCost.text = "" + selectedPlant.cost.Sun;
                singleton.waterCost.text = "" + selectedPlant.cost.Water;

                singleton.seedGain.text = "" + selectedPlant.production.Seed / selectedPlant.produceInterval;
                singleton.sunGain.text = "" + selectedPlant.production.Sun / selectedPlant.produceInterval;
                singleton.waterGain.text = "" + selectedPlant.production.Water / selectedPlant.produceInterval;

                singleton.description.text = selectedPlant.plantDescription;
                singleton.plantName.text = selectedPlant.GetComponent<NetworkBehaviour>().name;
            }
        }
    }
}
