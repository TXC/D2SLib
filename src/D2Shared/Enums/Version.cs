namespace D2Shared.Enums
{
    public enum SaveVersion : int
    {
        Any = 0,
        /// <summary>
        /// v1.00 through v1.06
        /// </summary>
        v106 = 0x47,    // 71
        /// <summary>
        /// v1.07 or Expansion Set v1.08
        /// </summary>
        v107 = 0x57,    // 87
        /// <summary>
        /// Standard game v1.08
        /// </summary>
        v108 = 0x59,    // 89
        /// <summary>
        /// v1.09
        /// </summary>
        v109 = 0x5c,    // 92
        /// <summary>
        /// v1.10 - v1.14d
        /// </summary>
        v11x = 0x60,    // 96
        /// <summary>
        /// v1.15 or v2.?? Alpha? (D2R)
        /// </summary>
        v200 = 0x61,    // 97
        /// <summary>
        /// v2.40 (D2R)
        /// </summary>
        v240 = 0x62,    // 98
        /// <summary>
        /// v2.50 (D2R)
        /// </summary>
        v250 = 0x63,    // 99

        Unknown = 0xffffff
    }
}
