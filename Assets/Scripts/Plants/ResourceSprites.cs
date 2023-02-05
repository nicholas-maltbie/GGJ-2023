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

using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceScriptableObject", order = 1)]
    public class ResourceSprites : ScriptableObject
    {
        public Sprite errorIcon;
        public Sprite waterIcon;
        public Sprite sunIcon;
        public Sprite seedIcon;
        public Sprite vpIcon;

        public Sprite GetIcon(Resource resource)
        {
            switch (resource)
            {
                case Resource.Sun:
                    return sunIcon;
                case Resource.Water:
                    return waterIcon;
                case Resource.Seeds:
                    return seedIcon;
                case Resource.VP:
                    return vpIcon;
                default:
                    return errorIcon;
            }
        }
    }
}
