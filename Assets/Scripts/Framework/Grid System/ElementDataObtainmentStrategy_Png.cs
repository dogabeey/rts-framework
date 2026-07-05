using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    /// <summary>
    /// Provides a strategy for obtaining grid element data by interpreting a PNG image as a map of elements and its alpha channel as number of elements
    /// to be created in corresponding grid cell of that coordinates. The color of each pixel in the PNG is mapped to a specific type 
    /// of grid element based on a predefined color-to-element mapping. The alpha channel of each pixel determines how many instances 
    /// of that element type should be created in the corresponding grid cell.
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewPngStrategy", menuName = "Grid/Element Data Strategy/PNG")]
    public class ElementDataObtainmentStrategy_Png : ElementDataObtainmentStrategy
    {
        [SerializeField] private Texture2D bitmap;
        [SerializeField] private int maxCount = 10; // This means at max alpha value of 255 will create 10 elements, and alpha value of 128 will create 5 elements, etc.
        [SerializeReference, SerializeField] private List<GridElement> availableElementTypes;

        public ElementDataObtainmentStrategy_Png()
        {
        }

        public override Dictionary<Vector2Int, List<GridElement>> GenerateElements()
        {
            var result = new Dictionary<Vector2Int, List<GridElement>>();

            if (bitmap == null)
            {
                Debug.LogError("PNG strategy has no bitmap assigned.");
                return result;
            }

            var colorLookup = new Dictionary<Color32, System.Type>();

            foreach (var element in availableElementTypes)
            {
                Color32 color = element.EditorColor;

                if (colorLookup.ContainsKey(color))
                {
                    Debug.LogWarning(
                        $"Duplicate EditorColor detected: {color}."
                    );
                    continue;
                }

                colorLookup.Add(color, element.GetType());
            }

            for (int y = 0; y < bitmap.height; y++)
            {
                for (int x = 0; x < bitmap.width; x++)
                {
                    Color32 pixel = bitmap.GetPixel(x, y);

                    if (pixel.a == 0)
                        continue;

                    Color32 rgbOnly = new Color32(
                        pixel.r,
                        pixel.g,
                        pixel.b,
                        255);

                    if (!colorLookup.TryGetValue(rgbOnly, out var elementType))
                    {
                        Debug.LogWarning(
                            $"No element type mapped to color {rgbOnly}."
                        );
                        continue;
                    }
                    if(pixel == Color.black)
                    {
                        continue;
                    }

                    Vector2Int position = new Vector2Int(x  , y);

                    if (!result.TryGetValue(position, out var elements))
                    {
                        elements = new List<GridElement>();
                        result.Add(position, elements);
                    }

                    int count = Mathf.CeilToInt((pixel.a / 255f) * maxCount);

                    for (int i = 0; i < count; i++)
                    {
                        GridElement instance =
                            (GridElement)System.Activator.CreateInstance(elementType);

                        instance.InitializeElement(position);

                        elements.Add(instance);
                    }
                }
            }

            return result;
        }
    }
}