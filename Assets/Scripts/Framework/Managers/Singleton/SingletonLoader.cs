using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Singleton
{
    public static class SingletonLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeAllSingletons()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (IsSubclassOfRawGeneric(typeof(SingletonComponent<>), type) && !type.IsAbstract && !type.IsGenericTypeDefinition)
                        {
                            // This type is a SingletonComponent. Access its Instance property to force initialization.
                            var instanceProp = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                            if (instanceProp != null)
                            {
                                instanceProp.GetValue(null, null);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // Ignore assemblies that fail to load types
                }
            }
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
