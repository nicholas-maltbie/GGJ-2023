
using nickmaltbie.IntoTheRoots.Plants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.IntoTheRoots.UI
{
    public class ResourceCounter : MonoBehaviour
    {
        public ResourceSprites resourceSprites;

        public Image icon;
        public TMP_Text Current;
        public TMP_Text Max;
        public Resource resource;

        public void Start()
        {
            icon.sprite = resourceSprites.GetIcon(resource);
        }

        public void UpdateCurrent(int current)
        {
            this.Current.text = current.ToString();
        }

        public void UpdateMax(int max)
        {
            this.Current.text = max.ToString();
        }
    }
}