using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewBitmapStrategy", menuName = "Grid/Cell Data Strategy/Bitmap")]
    public class CellDataObtainmentStrategy_Bmp : CellDataObtainmentStrategy
    {
        [SerializeField]
        private Texture2D bitmap;

        [SerializeReference, SerializeField]
        private List<GridCell> availableCellTypes;

        public override Dictionary<Vector2Int, GridCell> GenerateGrid()
        {
            Dictionary<Vector2Int, GridCell> result = new();

            if (bitmap == null)
            {
                Debug.LogError("Bitmap not assigned.");
                return result;
            }

            Dictionary<Color32, GridCell> colorLookup = new();

            foreach (var cell in availableCellTypes)
            {
                colorLookup[(Color32)cell.EditorColor] = cell;
            }

            for (int y = 0; y < bitmap.height; y++)
            {
                for (int x = 0; x < bitmap.width; x++)
                {
                    Color32 pixelColor = bitmap.GetPixel(x, y);

                    if (!colorLookup.TryGetValue(pixelColor, out GridCell prototype))
                    {
                        Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGB(pixelColor)}>No GridCell found for color {pixelColor}</color>");
                        continue;
                    }

                    GridCell newCell =
                        (GridCell)Activator.CreateInstance(prototype.GetType());

                    result.Add(
                        new Vector2Int(x, y),
                        newCell
                    );
                }
            }

            return result;
        }
    }
}