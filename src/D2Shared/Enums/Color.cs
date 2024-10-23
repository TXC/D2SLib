using System.Runtime.Serialization;

namespace D2Shared.Enums
{
    public enum Color
    {
        [EnumMember(Value = "whit")]
        White,

        [EnumMember(Value = "lgry")]
        LightGrey,

        [EnumMember(Value = "dgry")]
        DarkGrey,

        [EnumMember(Value = "blac")]
        Black,

        [EnumMember(Value = "lblu")]
        LightBlue,

        [EnumMember(Value = "dblu")]
        DarkBlue,

        [EnumMember(Value = "cblu")]
        CrystalBlue,

        [EnumMember(Value = "lred")]
        LightRed,

        [EnumMember(Value = "dred")]
        DarkRed,

        [EnumMember(Value = "cred")]
        CrystalRed,

        [EnumMember(Value = "lgrn")]
        LightGreen,

        [EnumMember(Value = "dgrn")]
        DarkGreen,

        [EnumMember(Value = "cgrn")]
        CrystalGreen,

        [EnumMember(Value = "lyel")]
        LightYellow,

        [EnumMember(Value = "dyel")]
        DarkYellow,

        [EnumMember(Value = "lgld")]
        LightGold,

        [EnumMember(Value = "dgld")]
        DarkGold,

        [EnumMember(Value = "lpur")]
        LightPurple,

        [EnumMember(Value = "dpur")]
        DarkPurple,

        [EnumMember(Value = "oran")]
        Orange,

        [EnumMember(Value = "bwht")]
        BrightWhite,
    }

    public enum Palette
    {
        NoColorChange = 0,

        Grey = 1,

        Grey2 = 2,

        Gold = 3,

        Brown = 4,

        GreyBrown = 5,

        InventoryGrey = 6,

        InventoryGrey2 = 7,

        InventoryGreyBrown = 8,
    }
}
