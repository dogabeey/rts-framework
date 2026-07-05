using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace Game.GfxManagement
{
    public class GfxManager_City : GfxManager
    {

        public struct Cell
        {
            public readonly struct Water
            {
                public readonly struct Shallow
                {
                    public static readonly AssetKey<Mesh> Mesh = "City/Cell/Water/Shallow/Mesh";
                    public static readonly AssetKey<Material> Material = "City/Cell/Water/Shallow/Material";
                }
                public readonly struct Deep
                {
                    public static readonly AssetKey<Mesh> Mesh = "City/Cell/Water/Deep/Mesh";
                    public static readonly AssetKey<Material> Material = "City/Cell/Water/Deep/Material";
                }
            }
            public readonly struct Ground
            {
                public readonly struct Coast
                {
                    public static readonly AssetKey<Mesh> Mesh = "City/Cell/Ground/Coast/Mesh";
                    public static readonly AssetKey<Material> Material = "City/Cell/Ground/Coast/Material";
                }
                public readonly struct Inner
                {
                    public static readonly AssetKey<Mesh> Mesh = "City/Cell/Ground/Inner/Mesh";
                    public static readonly AssetKey<Material> Material = "City/Cell/Ground/Inner/Material";
                }
            }
            public readonly struct Pavement
            {
                public static readonly AssetKey<Mesh> Mesh = "City/Cell/Pavement/Mesh";
                public static readonly AssetKey<Material> Material = "City/Cell/Pavement/Material";
            }
            public readonly struct Grass
            {
                public static readonly AssetKey<Mesh> Mesh = "City/Cell/Grass/Mesh";
                public static readonly AssetKey<Material> Material = "City/Cell/Grass/Material";
            }
            public readonly struct Empty
            {
                public static readonly AssetKey<Mesh> Mesh = "City/Cell/Empty/Mesh";
                public static readonly AssetKey<Material> Material = "City/Cell/Empty/Material";
            }
        }
        public struct Element
        {
            public readonly struct BuildingBlock
            {
                public readonly struct Residential
                {
                    public readonly struct FirstFloor
                    {
                        public static readonly AssetKey<Mesh> Mesh = "City/Element/BuildingBlock/Residential/FirstFloor/Mesh";
                        public static readonly AssetKey<Material> Material = "City/Element/BuildingBlock/Residential/FirstFloor/Material";
                    }
                    public readonly struct UpperFloor
                    {
                        public static readonly AssetKey<Mesh> Mesh = "City/Element/BuildingBlock/Residential/UpperFloor/Mesh";
                        public static readonly AssetKey<Material> Material = "City/Element/BuildingBlock/Residential/UpperFloor/Material";
                    }
                    public readonly struct Roof
                    {
                        public static readonly AssetKey<Mesh> Mesh = "City/Element/BuildingBlock/Residential/Roof/Mesh";
                        public static readonly AssetKey<Material> Material = "City/Element/BuildingBlock/Residential/Roof/Material";
                    }
                }
            }
            public readonly struct Park
            {
                public static readonly AssetKey<Mesh> Mesh = "City/Element/Park/Mesh";
                public static readonly AssetKey<Material> Material = "City/Element/Park/Material";
            }
        }
    }
}
