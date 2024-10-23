using System.Runtime.Serialization;

namespace D2Shared.Enums
{
    /// <summary>
    /// Item Location
    /// </summary>
    public enum ParentLocation : byte
    {
        Stored = 0x0,
        Equipped = 0x1,
        Belt = 0x2,
        Buffer = 0x4,
        Socket = 0x6,
    }

    /// <summary>
    /// Storage Location
    /// </summary>
    public enum StorageLocation : byte
    {
        None = 0x0,
        Inventory = 0x1,
        HoradricCube = 0x4,
        Stash = 0x5,
    }

    public enum EquippedSlot : byte
    {
        /// <summary>
        /// Inventory Grid
        /// </summary>
        None,

        /// <summary>
        /// Helmet
        /// </summary>
        [EnumMember(Value = "head")]
        Head,

        /// <summary>
        /// Amulet
        /// </summary>
        [EnumMember(Value = "neck")]
        Neck,

        /// <summary>
        /// Armor
        /// </summary>
        [EnumMember(Value = "tors")]
        Torso,

        /// <summary>
        /// Weapon
        /// </summary>
        [EnumMember(Value = "rarm")]
        RightHand,

        /// <summary>
        /// Shield
        /// </summary>
        [EnumMember(Value = "larm")]
        LeftHand,

        /// <summary>
        /// Ring
        /// </summary>
        [EnumMember(Value = "rrin")]
        RightFinger,

        /// <summary>
        /// Ribg
        /// </summary>
        [EnumMember(Value = "lrin")]
        LeftFinger,

        /// <summary>
        /// Belt
        /// </summary>
        [EnumMember(Value = "belt")]
        Waist,

        /// <summary>
        /// Boots
        /// </summary>
        [EnumMember(Value = "feet")]
        Feet,

        /// <summary>
        /// Gloves
        /// </summary>
        [EnumMember(Value = "glov")]
        Gloves,

        /// <summary>
        /// Alternate Right Hand (Expansion Set Only)
        /// </summary>
        SwapRight,

        /// <summary>
        /// Alternate Left Hand (Expansion Set Only)
        /// </summary>
        SwapLeft
    }

    public enum ItemQuality : byte
    {
        Any,
        /// <summary>
        /// Low Quality (ie. Crude)
        /// </summary>
        Inferior,
        /// <summary>
        /// Default value if the value is empty
        /// </summary>
        Normal,
        /// <summary>
        /// High Quality
        /// </summary>
        Superior,
        /// <summary>
        /// Uses Magic Prefixes and Suffixes
        /// </summary>
        Magic,
        Set,
        Rare,
        /// <summary>
        /// Predetermined stats
        /// </summary>
        Unique,
        Craft,
        Tempered
    }
}
