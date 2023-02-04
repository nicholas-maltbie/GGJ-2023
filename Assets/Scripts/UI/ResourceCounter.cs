
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

        private int max;
        private int current;

        public void Start()
        {
            icon.sprite = resourceSprites.GetIcon(resource);
        }

        public void UpdateCurrent(int current)
        {
            if (this.current != current)
            {
                Current.text = current.ToString();
                this.current = current;
            }
        }

        public void UpdateMax(int max)
        {
            if (this.max != max)
            {
                Max.text = max.ToString();
                this.max = max;
            }
        }
    }
}
