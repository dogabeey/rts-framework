using UnityEngine;
using System.Collections.Generic;

namespace Game.RTS
{
    [System.Serializable]
    public abstract class DamageType
    {
        public abstract string Name { get; set; }
        public abstract Dictionary<ArmorType, float> DamageModifiers { get; set; }
    }
    [System.Serializable]
    public class PiercingDamage : DamageType
    {
        public override string Name { get; set; } = "Piercing";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            { ArmorType.Light, 1.5f },
            { ArmorType.Heavy, 0.5f },
            { ArmorType.Biological, 1.0f },
            { ArmorType.Mechanical, 0.75f },
            { ArmorType.Psychic, 1.0f },
            { ArmorType.Structural, 0.25f }
        };
    }
    [System.Serializable]
    public class SlashingDamage : DamageType
    {
        public override string Name { get; set; } = "Slashing";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            { ArmorType.Light, 1.0f },
            { ArmorType.Heavy, 0.75f },
            { ArmorType.Biological, 1.25f },
            { ArmorType.Mechanical, 0.5f },
            { ArmorType.Psychic, 1.0f },
            { ArmorType.Structural, 0.25f }
        };
    }
    [System.Serializable]
    public class BluntDamage : DamageType
    {
        public override string Name { get; set; } = "Blunt";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            { ArmorType.Light, 0.75f },
            { ArmorType.Heavy, 1.5f },
            { ArmorType.Biological, 1.0f },
            { ArmorType.Mechanical, 1.25f },
            { ArmorType.Psychic, 1.0f },
            { ArmorType.Structural, 0.5f }
        };
    }
    [System.Serializable]
    public class FireDamage : DamageType
    {
        public override string Name { get; set; } = "Fire";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            { ArmorType.Light, 1.25f },
            { ArmorType.Heavy, 0.75f },
            { ArmorType.Biological, 1.5f },
            { ArmorType.Mechanical, 0.5f },
            { ArmorType.Psychic, 1.0f },
            { ArmorType.Structural, 0.25f }
        };
    }
    public class PsychicDamage : DamageType
    {
        public override string Name { get; set; } = "Psychic";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            
        };
    }
    public class NatureDamage : DamageType
    {
        public override string Name { get; set; } = "Nature";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            
        };
    }
    public class ElectricDamage : DamageType
    {
        public override string Name { get; set; } = "Electric";
        public override Dictionary<ArmorType, float> DamageModifiers { get; set; } = new Dictionary<ArmorType, float>
        {
            
        };
    }
}
