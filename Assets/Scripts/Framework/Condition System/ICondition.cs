using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICondition
{
    public abstract bool IsTrue(int paramInt = 0, float paramFloat = 0f, GameObject paramObject = null, string paramString = "", bool paramBool = false);
}
[Serializable]
public abstract class Condition : ICondition
{
    public int intParameter;
    public float floatParameter;
    public GameObject objectParameter;
    public string stringParameter;
    public bool boolParameter;
    [Space]
    public int priority; // Higher priority conditions will be evaluated first in a system that checks multiple conditions.

    public abstract bool IsTrue(int paramInt = 0, float paramFloat = 0f, GameObject paramObject = null, string paramString = "", bool paramBool = false);

}