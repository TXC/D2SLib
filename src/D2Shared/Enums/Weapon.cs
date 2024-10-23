using System.Runtime.Serialization;

namespace D2Shared.Enums
{
    public enum WeaponClass
    {
        /// <summary>
        /// Hand to Hand (Default value if the value is empty)
        /// </summary>
        [EnumMember(Value = "hth")]
        HandToHand,

        /// <summary>
        /// One Handed Swing
        /// </summary>
        [EnumMember(Value = "1hs")]
        OneHandedSwing,

        /// <summary>
        /// One Handed Thrust
        /// </summary>
        [EnumMember(Value = "1ht")]
        OneHandedThrust,

        /// <summary>
        /// Bow
        /// </summary>
        [EnumMember(Value = "bow")]
        Bow,

        /// <summary>
        /// Two Handed Swing
        /// </summary>
        [EnumMember(Value = "2hs")]
        TwoHandedSwing,

        /// <summary>
        /// Two Handed Thrust
        /// </summary>
        [EnumMember(Value = "2ht")]
        TwoHandedThrust,

        /// <summary>
        /// Dual Wielding - Left Jab Right Swing
        /// </summary>
        [EnumMember(Value = "1js")]
        LeftJabRightSwing,

        /// <summary>
        /// Dual Wielding - Left Jab Right Thrust
        /// </summary>
        [EnumMember(Value = "1jt")]
        LeftJabRightThrust,

        /// <summary>
        /// Dual Wielding - Left Swing Right Swing
        /// </summary>
        [EnumMember(Value = "1ss")]
        LeftSwingRightSwing,

        /// <summary>
        /// Dual Wielding - Left Swing Right Thrust
        /// </summary>
        [EnumMember(Value = "1st")]
        LeftSwingRightThrust,

        /// <summary>
        /// Staff
        /// </summary>
        [EnumMember(Value = "stf")]
        Staff,

        /// <summary>
        /// Crossbow
        /// </summary>
        [EnumMember(Value = "xbw")]
        Crossbow,

        /// <summary>
        /// One Hand to Hand
        /// </summary>
        [EnumMember(Value = "ht1")]
        OneHandToHand,

        /// <summary>
        /// Two Hand to Hand
        /// </summary>
        [EnumMember(Value = "ht2")]
        TwoHandToHand,
    }
}
