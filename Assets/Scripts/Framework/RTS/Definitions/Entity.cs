using UnityEngine;
using Game.Core;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEngine.InputSystem;
using System.Linq;

namespace Game.RTS
{
    public abstract class Entity : ScriptableObject, IVisualData
    {
        [System.Serializable]
        public class OrderData
        {
            public Order order;
            public int orderIndex;
            [ValueDropdown("@Entity.GetAllInputActionFieldNamesInRTSInputActionAsset()", ExpandAllMenuItems = true)]
            [Tooltip("This is the name of the InputAction field in the RTS_InputActions asset that will be used to trigger this order. If left empty, the default InputAction for this order will be used.")]
            public string inputActionOverride;
        }
        public Mesh MeshReference => meshReference;
        public Material MaterialReference => materialReference;
        public Sprite SpriteReference => spriteReference;

        public Mesh meshReference;
        public Material materialReference;
        public Sprite spriteReference;

        [FoldoutGroup("General")] public string _name;
        [FoldoutGroup("General")] public OrderData[] orderDataArray;

        public static ValueDropdownList<string> GetAllInputActionFieldNamesInRTSInputActionAsset()
        {
            ValueDropdownList<string> inputActionFieldNames = new ValueDropdownList<string>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            foreach (FieldInfo field in typeof(RTS_InputActions).GetFields(bindingFlags))
            {
                if (field.FieldType != typeof(InputAction))
                {
                    continue;
                }

                if (!field.Name.StartsWith("m_RTS_"))
                {
                    continue;
                }

                inputActionFieldNames.Add(field.Name.Replace("m_RTS_", "").Replace("_", " "), field.Name);
            }

            return inputActionFieldNames;
        }
    }
}
