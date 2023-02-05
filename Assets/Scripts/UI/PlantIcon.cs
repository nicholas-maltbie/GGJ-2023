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

using nickmaltbie.IntoTheRoots.Plants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.IntoTheRoots.UI
{
    public class PlantIcon : MonoBehaviour
    {
        public float transparency = 0.75f;
        public Color selectedColor = new Color(255, 255, 153);
        public Image panel;
        public Image icon;
        public Image border;
        public TMP_Text plantName;
        public TMP_Text plantNumber;

        public int plantIndex;
        public Plant plant;

        private Color basicColor = new Color(0, 0, 0);

        public void SetSelected(bool value)
        {
            border.enabled = value;
            panel.color = value ? selectedColor : basicColor;
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, transparency);
            if( value )
            {
                PlantDetailsDisplay.UpdateDisplay(plant);
            }
        }

        public void Awake()
        {
            basicColor = panel.color;
        }

        public void Start()
        {
            plantName.text = plant.name;
            plantNumber.text = plantIndex.ToString();
            icon.sprite = plant.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
