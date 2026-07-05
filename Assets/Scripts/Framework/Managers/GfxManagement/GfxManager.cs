using Game.Singleton;
using Game.Singleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.GfxManagement
{
    public class GfxManager : SingletonComponent<GfxManager>
    {
        public string assetFilesPath = "Assets/AddressableAssets/";
        public Mesh defaultGeneratedMesh;
        public Material defaultGeneratedMaterial;
        public Sprite ddefaultGeneratedSprite;

        public bool assetsLoaded = false;
        public Dictionary<string, Object> Assets = new Dictionary<string, Object>();
        private async void Start()
        {
            // Preload all assets by traversing all structs and accessing all properties using reflection.
            await PreloadAllAssets();
            assetsLoaded = true;
            Debug.Log("[Gfx] Loaded all assets");
        }

        private async Task PreloadAllAssets()
        {
            var visitedTypes = new HashSet<System.Type>();
            var assetKeys = new HashSet<string>();
            var loadTasks = new List<Task>();

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    types = exception.Types;
                }

                foreach (var type in types)
                {
                    if (type == null || !type.IsSubclassOf(typeof(GfxManager)))
                    {
                        continue;
                    }

                    CollectAssetKeysFromNestedTypes(type, visitedTypes, assetKeys);
                }
            }

            foreach (var assetKey in assetKeys)
            {
                if (string.IsNullOrWhiteSpace(assetKey) || Assets.ContainsKey(assetKey))
                {
                    continue;
                }

                var key = assetKey;
                var handle = Addressables.LoadAssetAsync<Object>(assetKey);
                var taskCompletionSource = new TaskCompletionSource<bool>();

                handle.Completed += handle =>   
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Assets[key] = handle.Result;
                        Debug.Log($"[Gfx] Loaded asset with key: {key}");
                    }
                    else
                    {
                        Debug.LogError($"[Gfx] Failed to load asset with key: {key}");
                    }

                    taskCompletionSource.TrySetResult(true);
                };

                loadTasks.Add(taskCompletionSource.Task);
            }

            await Task.WhenAll(loadTasks);
        }

        private void CollectAssetKeysFromNestedTypes(System.Type parentType, HashSet<System.Type> visitedTypes, HashSet<string> assetKeys)
        {
            if (parentType == null || !visitedTypes.Add(parentType))
            {
                return;
            }

            var nestedTypes = parentType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var nestedType in nestedTypes)
            {
                if (nestedType.IsValueType && !nestedType.IsEnum)
                {
                    var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach (var field in fields)
                    {
                        if (!field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(AssetKey<>))
                        {
                            continue;
                        }

                        var assetKeyInstance = field.GetValue(null);
                        if (assetKeyInstance == null)
                        {
                            continue;
                        }
                        
                        var keyProperty = field.FieldType.GetField("Key", BindingFlags.Public | BindingFlags.Instance);
                        if (keyProperty == null)
                        {
                            continue;
                        }

                        var assetKey = keyProperty.GetValue(assetKeyInstance) as string;
                        if (!string.IsNullOrWhiteSpace(assetKey))
                        {
                            assetKeys.Add(assetKey);
                        }
                    }
                }

                CollectAssetKeysFromNestedTypes(nestedType, visitedTypes, assetKeys);
            }
        }
        // Convert string to object
        public static T GetAsset<T>(string assetKey) where T : Object
        {
            if (Instance.Assets.TryGetValue(assetKey, out var asset))
            {
                if (asset is T typedAsset)
                {
                    return typedAsset;
                }

                if (typeof(T) == typeof(Mesh) && asset is GameObject gameObject)
                {
                    var meshFilter = gameObject.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        return meshFilter.sharedMesh as T;
                    }

                    var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                    {
                        return skinnedMeshRenderer.sharedMesh as T;
                    }
                }

                Debug.LogError($"[Gfx] Asset type mismatch for key: {assetKey}. Requested: {typeof(T).Name}, Loaded: {asset.GetType().Name}");
                return null;
            }
            Debug.LogError($"[Gfx] Asset not found for key: {assetKey}");
            return null;
        }

        // Traverse all structs in this class and generate asset files for all AssetKey<string> fields and generate addressable keys
        // based on the struct nesting. File paths should be based on the struct nesting and field name, for example:
        // GfxManager_City.Cell.Water.Shallow.Mesh should generate an asset file at Assets/AddressableAssets/GfxManager_City/Cell/
        // Water/Shallow/Mesh.fbx and the addressable key should be GfxManager_City.Cell.Water.Shallow.Mesh.
        // If the field is of type AssetKey<Mesh>, the generated asset should be copied from the defaultGeneratedMesh,
        // if the field is of type AssetKey<Material>, the generated asset should be copied from the defaultGeneratedMaterial,
        // and if the field is of type AssetKey<Sprite>, the generated asset should be copied from the ddefaultGeneratedSprite. This method should be called from a custom editor button in the inspector.
        [Button("Generate Asset Files")]
        public void GenerateAssetFiles()
        {
#if UNITY_EDITOR
            var settings = GetAddressableSettings();
            if (settings == null)
            {
                Debug.LogError("[Gfx] Addressables settings could not be found.");
                return;
            }

            var targetGroup = GetDefaultAddressableGroup(settings);
            if (targetGroup == null)
            {
                targetGroup = GetFirstAddressableGroup(settings);
            }

            if (targetGroup == null)
            {
                Debug.LogError("[Gfx] No addressable group could be found.");
                return;
            }

            int generatedAssetCount = 0;
            int updatedEntryCount = 0;
            GenerateAssetFilesForNestedTypes(GetType(), new List<string> { GetType().Name }, settings, targetGroup, ref generatedAssetCount, ref updatedEntryCount);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(this);

            Debug.Log($"[Gfx] GenerateAssetFiles completed. Generated {generatedAssetCount} assets and updated {updatedEntryCount} addressable entries.");
#else
            Debug.LogWarning("[Gfx] GenerateAssetFiles can only be used in the Unity Editor.");
#endif
        }

#if UNITY_EDITOR
        private void GenerateAssetFilesForNestedTypes(System.Type parentType, List<string> typePath, object settings, object targetGroup, ref int generatedAssetCount, ref int updatedEntryCount)
        {
            var nestedTypes = parentType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var nestedType in nestedTypes)
            {
                var nestedTypePath = new List<string>(typePath)
                {
                    nestedType.Name
                };

                if (nestedType.IsValueType && !nestedType.IsEnum)
                {
                    var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach (var field in fields)
                    {
                        if (!field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(AssetKey<>))
                        {
                            continue;
                        }

                        GenerateAssetFileForField(field, nestedTypePath, settings, targetGroup, ref generatedAssetCount, ref updatedEntryCount);
                    }
                }

                GenerateAssetFilesForNestedTypes(nestedType, nestedTypePath, settings, targetGroup, ref generatedAssetCount, ref updatedEntryCount);
            }
        }

        private void GenerateAssetFileForField(FieldInfo field, List<string> typePath, object settings, object targetGroup, ref int generatedAssetCount, ref int updatedEntryCount)
        {
            var assetType = field.FieldType.GetGenericArguments()[0];
            var templateAsset = GetTemplateAsset(assetType);
            if (templateAsset == null)
            {
                Debug.LogWarning($"[Gfx] Missing default generated asset for type {assetType.Name}. Skipping {field.DeclaringType.FullName}.{field.Name}.");
                return;
            }

            var templateAssetPath = AssetDatabase.GetAssetPath(templateAsset);
            if (string.IsNullOrWhiteSpace(templateAssetPath))
            {
                Debug.LogWarning($"[Gfx] Could not resolve template asset path for {field.DeclaringType.FullName}.{field.Name}.");
                return;
            }

            var extension = Path.GetExtension(templateAssetPath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = GetDefaultExtension(assetType);
            }

            // Make file name same as path name except putting "_" instead of "/"
            var typePathWithoutFirstElement = typePath.GetRange(1, typePath.Count - 1);
            string fileName = string.Join("_", typePathWithoutFirstElement) + "_" + field.Name;
            var assetPathSegments = new List<string>(typePath)
            {
                fileName
            };

            var assetPath = BuildAssetPath(assetPathSegments, extension);
            var directoryPath = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(assetPath))
            {
                if (!AssetDatabase.CopyAsset(templateAssetPath, assetPath))
                {
                    Debug.LogError($"[Gfx] Failed to copy template asset from '{templateAssetPath}' to '{assetPath}'.");
                    return;
                }

                generatedAssetCount++;
            }

            AssetDatabase.ImportAsset(assetPath);

            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrWhiteSpace(assetGuid))
            {
                Debug.LogError($"[Gfx] Failed to resolve GUID for generated asset at '{assetPath}'.");
                return;
            }

            var entry = CreateOrMoveAddressableEntry(settings, assetGuid, targetGroup);
            if (entry == null)
            {
                Debug.LogError($"[Gfx] Failed to create addressable entry for '{assetPath}'.");
                return;
            }

            var generatedAddress = string.Join(".", assetPathSegments);
            var assetKey = GetAssetKey(field);
            var desiredAddress = string.IsNullOrWhiteSpace(assetKey) ? generatedAddress : assetKey;
            var currentAddress = GetAddressableEntryAddress(entry);
            if (currentAddress != desiredAddress)
            {
                SetAddressableEntryAddress(entry, desiredAddress);
                updatedEntryCount++;
            }
        }

        private object GetAddressableSettings()
        {
            var settingsType = System.Type.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
            if (settingsType == null)
            {
                return null;
            }

            var settingsProperty = settingsType.GetProperty("Settings", BindingFlags.Public | BindingFlags.Static);
            if (settingsProperty == null)
            {
                return null;
            }

            return settingsProperty.GetValue(null);
        }

        private object GetDefaultAddressableGroup(object settings)
        {
            if (settings == null)
            {
                return null;
            }

            var defaultGroupProperty = settings.GetType().GetProperty("DefaultGroup", BindingFlags.Public | BindingFlags.Instance);
            return defaultGroupProperty?.GetValue(settings);
        }

        private object GetFirstAddressableGroup(object settings)
        {
            if (settings == null)
            {
                return null;
            }

            var groupsField = settings.GetType().GetField("groups", BindingFlags.Public | BindingFlags.Instance);
            var groups = groupsField?.GetValue(settings) as System.Collections.IList;
            if (groups == null || groups.Count == 0)
            {
                return null;
            }

            return groups[0];
        }

        private object CreateOrMoveAddressableEntry(object settings, string assetGuid, object targetGroup)
        {
            if (settings == null || targetGroup == null)
            {
                return null;
            }

            var method = settings.GetType().GetMethod("CreateOrMoveEntry", new[] { typeof(string), targetGroup.GetType(), typeof(bool), typeof(bool) });
            if (method == null)
            {
                foreach (var candidate in settings.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (candidate.Name != "CreateOrMoveEntry")
                    {
                        continue;
                    }

                    var parameters = candidate.GetParameters();
                    if (parameters.Length == 4 && parameters[0].ParameterType == typeof(string))
                    {
                        method = candidate;
                        break;
                    }
                }
            }

            return method?.Invoke(settings, new[] { assetGuid, targetGroup, false, false });
        }

        private string GetAddressableEntryAddress(object entry)
        {
            if (entry == null)
            {
                return null;
            }

            var addressField = entry.GetType().GetField("address", BindingFlags.Public | BindingFlags.Instance);
            if (addressField != null)
            {
                return addressField.GetValue(entry) as string;
            }

            var addressProperty = entry.GetType().GetProperty("address", BindingFlags.Public | BindingFlags.Instance)
                ?? entry.GetType().GetProperty("Address", BindingFlags.Public | BindingFlags.Instance);
            return addressProperty?.GetValue(entry) as string;
        }

        private void SetAddressableEntryAddress(object entry, string address)
        {
            if (entry == null)
            {
                return;
            }

            var addressField = entry.GetType().GetField("address", BindingFlags.Public | BindingFlags.Instance);
            if (addressField != null)
            {
                addressField.SetValue(entry, address);
                return;
            }

            var addressProperty = entry.GetType().GetProperty("address", BindingFlags.Public | BindingFlags.Instance)
                ?? entry.GetType().GetProperty("Address", BindingFlags.Public | BindingFlags.Instance);
            if (addressProperty != null && addressProperty.CanWrite)
            {
                addressProperty.SetValue(entry, address);
            }
        }

        private Object GetTemplateAsset(System.Type assetType)
        {
            if (assetType == typeof(Mesh))
            {
                return defaultGeneratedMesh;
            }

            if (assetType == typeof(Material))
            {
                return defaultGeneratedMaterial;
            }

            if (assetType == typeof(Sprite))
            {
                return ddefaultGeneratedSprite;
            }

            return null;
        }

        private string GetAssetKey(FieldInfo field)
        {
            var assetKeyInstance = field.GetValue(null);
            if (assetKeyInstance == null)
            {
                return null;
            }

            var keyField = field.FieldType.GetField("Key", BindingFlags.Public | BindingFlags.Instance);
            if (keyField == null)
            {
                return null;
            }

            return keyField.GetValue(assetKeyInstance) as string;
        }

        private string BuildAssetPath(List<string> assetPathSegments, string extension)
        {
            var sanitizedRoot = assetFilesPath.Replace("\\", "/").TrimEnd('/');
            var combinedPath = sanitizedRoot;

            foreach (var segment in assetPathSegments)
            {
                combinedPath += "/" + segment.Trim('/');
            }

            return combinedPath + extension;
        }

        private string GetDefaultExtension(System.Type assetType)
        {
            if (assetType == typeof(Material))
            {
                return ".mat";
            }

            if (assetType == typeof(Sprite))
            {
                return ".png";
            }

            return ".asset";
        }
#endif

        public readonly struct AssetKey<T>
            where T : UnityEngine.Object
        {
            public readonly string Key;

            public AssetKey(string key)
            {
                Key = key;
            }

            public static implicit operator T(AssetKey<T> assetKey)
            {
                return GfxManager.GetAsset<T>(assetKey.Key);
            }

            public static implicit operator AssetKey<T>(string key)
            {
                return new AssetKey<T>(key);
            }
        }
    }
}
