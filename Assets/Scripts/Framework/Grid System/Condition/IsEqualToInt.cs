using System;
using UnityEngine;

namespace Game.GridSystem
{
    [Serializable]
    public class IsEqualToInt : Condition
    {
        public override bool IsTrue(int paramInt, float paramFloat, GameObject paramObject, string paramString, bool paramBool)
        {
            return paramInt == intParameter;
        }
    }
    [Serializable]
    public class IsEqualToBool : Condition
    {
        public override bool IsTrue(int paramInt, float paramFloat, GameObject paramObject, string paramString, bool paramBool)
        {
            return paramBool == boolParameter;
        }
    }
    [Serializable]
    public class IsEqualToFloat : Condition
        {
        public override bool IsTrue(int paramInt, float paramFloat, GameObject paramObject, string paramString, bool paramBool)
        {
            return Mathf.Approximately(paramFloat, floatParameter);
        }
    }
}