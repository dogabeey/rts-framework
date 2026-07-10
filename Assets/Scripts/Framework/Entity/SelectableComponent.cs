using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class SelectableComponent : MonoBehaviour
    {
        public static IReadOnlyCollection<SelectableComponent> All => allSelectables;
        public static IReadOnlyCollection<SelectableComponent> Selected => selected;

        [SerializeField] private Renderer[] selectionRenderers;
        [SerializeField] private Color selectedColor = Color.green;

        public bool IsSelected { get; private set; }

        public event Action<SelectableComponent, bool> SelectionChanged;

        private static readonly HashSet<SelectableComponent> allSelectables = new HashSet<SelectableComponent>();
        private static readonly HashSet<SelectableComponent> selected = new HashSet<SelectableComponent>();

        private Color[] originalColors;

        private void Awake()
        {
            if (selectionRenderers == null || selectionRenderers.Length == 0)
            {
                selectionRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);
            }

            CacheOriginalColors();
        }

        private void OnEnable()
        {
            allSelectables.Add(this);
        }

        private void OnDisable()
        {
            allSelectables.Remove(this);
            SetSelected(false);
        }

        private void OnDestroy()
        {
            allSelectables.Remove(this);
            SetSelected(false);
        }

        public void SetSelected(bool selectedState)
        {
            if (IsSelected == selectedState)
            {
                return;
            }

            IsSelected = selectedState;

            if (selectedState)
            {
                selected.Add(this);
            }
            else
            {
                selected.Remove(this);
            }

            ApplySelectionVisuals();
            SelectionChanged?.Invoke(this, IsSelected);
        }

        public static void DeselectAll()
        {
            if (selected.Count == 0)
            {
                return;
            }

            var snapshot = new List<SelectableComponent>(selected);
            foreach (var selectable in snapshot)
            {
                if (selectable != null)
                {
                    selectable.SetSelected(false);
                }
            }
        }

        private void CacheOriginalColors()
        {
            originalColors = new Color[selectionRenderers.Length];
            for (var i = 0; i < selectionRenderers.Length; i++)
            {
                var rendererRef = selectionRenderers[i];
                if (rendererRef == null || rendererRef.sharedMaterial == null)
                {
                    originalColors[i] = Color.white;
                    continue;
                }

                originalColors[i] = rendererRef.material.color;
            }
        }

        private void ApplySelectionVisuals()
        {
            for (var i = 0; i < selectionRenderers.Length; i++)
            {
                var rendererRef = selectionRenderers[i];
                if (rendererRef == null || rendererRef.sharedMaterial == null)
                {
                    continue;
                }

                rendererRef.material.color = IsSelected ? selectedColor : originalColors[i];
            }
        }
    }
}