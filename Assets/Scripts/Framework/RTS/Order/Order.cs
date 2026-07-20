using UnityEngine;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace Game.RTS.Orders
{
    [CreateAssetMenu(fileName = "RTSOrder", menuName = "Scriptable Objects/RTSOrder")]
    public class Order : ScriptableObject
    {
        
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
