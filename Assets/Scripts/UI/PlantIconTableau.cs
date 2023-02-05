using System;
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
using nickmaltbie.IntoTheRoots.Plants;
using nickmaltbie.IntoTheRoots.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace nickmaltbie.IntoTheRoots.UI
{
    public class PlantIconTableau : MonoBehaviour
    {
        public InputActionReference changeSelectedScroll;
        public InputActionReference changeSelectedKey;
        public static PlantIconTableau Singleton;
        public PlantDatabase plantDatabase;
        public PlantIcon plantIconPrefab;
        public int bufferPixels = 5;
        private int selected = 0;
        private Dictionary<int, PlantIcon> icons = new Dictionary<int, PlantIcon>();

        private PlantType previouslySelected = PlantType.None;

        public Plant SelectedPlant()
        {
            return icons[selected].plant;
        }

        public PlantType SelectedType()
        {
            return SelectedPlant().plantType;
        }

        public void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            Singleton ??= this;

            float width = plantIconPrefab.GetComponent<RectTransform>().sizeDelta.x;

            // Within this object, spawn a resource counter
            // and space them out by buffer pixels vertically
            // for each resource specified.
            int index = 0;
            foreach (Plant plant in plantDatabase.EnumeratePlants())
            {
                // Spawn each one at the height of the previous + 5 * index
                GameObject go = Instantiate(plantIconPrefab.gameObject, transform.position, Quaternion.identity);
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3((width + bufferPixels) * index, 0);
                icons[index] = go.GetComponent<PlantIcon>();
                icons[index].plant = plant;
                icons[index].plantIndex = index;
                icons[index].SetSelected(false);

                if (icons[index].GetComponent<Button>() is Button button)
                {
                    int idx = index;
                    button.onClick.AddListener(() => SetSelected(idx));
                }

                index++;
            }

            icons[selected].SetSelected(true);
            changeSelectedScroll.action.Enable();
            changeSelectedKey.action.Enable();
        }

        public void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
            }
        }

        public void SetSelected(int index)
        {
            // update player selected plant based on selected
            if (NetworkManager.Singleton?.SpawnManager?.GetLocalPlayerObject() is NetworkObject localPlayer &&
                localPlayer.GetComponent<Planter>() is Planter planter)
            {
                planter.toPlant = SelectedPlant();
            }

            if (index == selected)
            {
                return;
            }

            icons[selected].SetSelected(false);
            selected = index;
            icons[selected].SetSelected(true);

            ulong localId = NetworkManager.Singleton.LocalClientId;

            foreach (Plant plant in PlantUtils.GetPlantsOwnedByPlayer(localId))
            {
                plant.SetRestrictedAreaVisible(false);
            }

            foreach (Plant plant in PlantUtils.GetAllPlayerPlantsOfType(localId, SelectedType()))
            {
                plant.SetRestrictedAreaVisible(true);
            }

            previouslySelected = SelectedType();
        }

        public void Update()
        {
            //Get Scroll wheel input
            float value = changeSelectedScroll.action.ReadValue<float>();
            int nextSelected = selected + Mathf.RoundToInt(value);

            //Get Keyboard input
            float value1 = changeSelectedKey.action.ReadValue<float>();

            //If Keyboard input, change nextSelected
            if(changeSelectedKey.action.triggered) {
                nextSelected = Mathf.RoundToInt(value1);
            }

            while (nextSelected < 0)
            {
                nextSelected += icons.Count;
            }

            nextSelected %= icons.Count;

            SetSelected(nextSelected);
        }
    }
}
