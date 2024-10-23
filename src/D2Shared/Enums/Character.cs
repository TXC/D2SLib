using System.Runtime.Serialization;

namespace D2Shared.Enums
{
    public enum CharacterClass : byte
    {
        [EnumMember(Value = "ama")]
        Amazon = 0x00,

        [EnumMember(Value = "sor")]
        Sorceress = 0x01,

        [EnumMember(Value = "nec")]
        Necromancer = 0x02,

        [EnumMember(Value = "pal")]
        Paladin = 0x03,

        [EnumMember(Value = "dru")]
        Druid = 0x05,

        [EnumMember(Value = "bar")]
        Barbarian = 0x04,

        [EnumMember(Value = "ass")]
        Assassin = 0x06,

        Unknown = 0xff,
    }
}
