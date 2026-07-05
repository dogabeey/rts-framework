using Game.Core;
using UnityEngine;

namespace Game.GridSystem
{
    public class GridElementController : MonoBehaviour
    {
        public GridElement referenceElement { get; private set; }

        [SerializeField] private Renderer elementRenderer;
        public void Initialize(GridElement element, Vector2Int position)
        {
            referenceElement = element;
            referenceElement.InitializeElement(position);
            referenceElement.SetVisualData(ref elementRenderer);
        }
    }
}