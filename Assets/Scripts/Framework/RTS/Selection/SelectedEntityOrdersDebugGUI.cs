using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.RTS
{
    /// <summary>
    /// Runtime-only diagnostic overlay showing the orders exposed by selected entities.
    /// It is added automatically by <see cref="SelectionManager"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SelectedEntityOrdersDebugGUI : MonoBehaviour
    {
        private const float PanelWidth = 420f;

        private readonly StringBuilder textBuilder = new StringBuilder();
        private Vector2 scrollPosition;
        private RTS_InputActions inputActions;

        private void OnEnable()
        {
            inputActions = new RTS_InputActions();
        }

        private void OnDisable()
        {
            inputActions?.Dispose();
            inputActions = null;
        }

        private void OnGUI()
        {
            var panelHeight = Mathf.Min(Screen.height - 24f, 520f);
            GUILayout.BeginArea(new Rect(Screen.width - PanelWidth - 12f, 12f, PanelWidth, panelHeight), GUI.skin.box);
            GUILayout.Label("Selected Entity Orders", GUI.skin.label);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            var hasSelectedEntity = false;
            foreach (var selectable in SelectableComponent.All)
            {
                if (selectable == null || !selectable.IsSelected
                    || !selectable.TryGetComponent<EntityController>(out var entityController))
                {
                    continue;
                }

                hasSelectedEntity = true;
                DrawEntityOrders(entityController);
            }

            if (!hasSelectedEntity)
            {
                GUILayout.Label("No entity selected.");
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawEntityOrders(EntityController entityController)
        {
            var entity = entityController.referenceEntity;
            var entityName = entity != null && !string.IsNullOrWhiteSpace(entity._name)
                ? entity._name
                : entityController.name;

            GUILayout.Space(6f);
            GUILayout.Label(entityName, GUI.skin.box);

            if (entity == null || entity.orderDataArray == null || entity.orderDataArray.Length == 0)
            {
                GUILayout.Label("  No available orders.");
                return;
            }

            foreach (var orderData in entity.orderDataArray)
            {
                if (orderData?.order == null)
                {
                    continue;
                }

                textBuilder.Clear();
                textBuilder.Append(orderData.order.Name);
                textBuilder.Append("  [");
                textBuilder.Append(GetShortcutLabel(orderData.inputActionOverride));
                textBuilder.Append("]\n");
                textBuilder.Append(orderData.order.Description);
                GUILayout.Label(textBuilder.ToString(), GUI.skin.box, GUILayout.ExpandWidth(true));
            }
        }

        private string GetShortcutLabel(string configuredAction)
        {
            if (string.IsNullOrWhiteSpace(configuredAction) || inputActions == null)
            {
                return "Unassigned";
            }

            // Serialized order data may contain either the generated field name
            // (m_RTS_Skill1) or the Input System action name (Skill1).
            const string fieldPrefix = "m_RTS_";
            var actionName = configuredAction.StartsWith(fieldPrefix)
                ? configuredAction.Substring(fieldPrefix.Length)
                : configuredAction;
            var action = inputActions.asset.FindAction($"RTS/{actionName}", throwIfNotFound: false);

            if (action == null)
            {
                return actionName;
            }

            var binding = action.GetBindingDisplayString();
            return string.IsNullOrWhiteSpace(binding) ? actionName : binding;
        }
    }
}
