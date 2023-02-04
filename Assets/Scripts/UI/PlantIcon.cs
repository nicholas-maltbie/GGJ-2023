
using nickmaltbie.IntoTheRoots.Plants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.IntoTheRoots.UI
{
    public class PlantIcon : MonoBehaviour
    {
        public Image icon;
        public Image border;
        public TMP_Text plantName;
        public TMP_Text plantNumber;

        public int plantIndex;
        public Plant plant;

        public void SetSelected(bool value)
        {
            border.enabled = value;
        }

        public void Start()
        {
            plantName.text = plant.name;
            plantNumber.text = plantIndex.ToString();
            icon.sprite = plant.GetComponent<SpriteRenderer>().sprite;
        }
    }
}