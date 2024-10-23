using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using D2SImporter.Attributes;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    [FileName("ItemTypes.txt"), PostImport(typeof(Action<Dictionary<string, ItemType>>), typeof(ItemType), nameof(PostImport))]
    public class ItemType : ID2Data
    {
        #region Properties
        [JsonIgnore, ColumnName("ItemType"), Translatable]
        public string Name { get; set; }

        /// <summary>
        /// This is a reference field to define the Item Type name
        /// </summary>
        [ColumnName("ItemType")]
        public string Index { get; set; } = string.Empty;

        /// <summary>
        /// Defines the unique pointer for this Item Type, which is used
        /// by the following files: weapons.txt, armor.txt, misc.txt,
        /// cubemain.txt, skills.txt, treasureclassex.txt
        /// </summary>
        [JsonIgnore, ColumnName, Key]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Points to the index of another Item Type to reference as a parent.
        /// This is used to create a hierarchy for Item Types where the parents
        /// will have more universal settings shared across the related children
        /// </summary>
        [JsonIgnore, ColumnName]
        public string Equiv1 { get; set; } = string.Empty;

        /// <inheritdoc cref="Equiv1"/>
        [JsonIgnore, ColumnName]
        public string Equiv2 { get; set; } = string.Empty;

        /// <summary>
        /// <para>If set, then the item can be repaired by an NPC in the shop UI.</para>
        /// <para>If not set, then the item cannot be repaired.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Repair { get; set; }

        /// <summary>
        /// <para>If set, then the item can be equipped by a character
        /// (also will require the “BodyLoc1” & “BodyLoc2” fields as
        /// parameters).</para>
        /// <para>If not set, then the item can only be carried in
        /// the inventory, stash, or Horadric Cube.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Body { get; set; }

        /// <summary>
        /// These are required parameters if the <see cref="Body"/> field is enabled.
        /// These fields specify the inventory slots where the item can be equipped.
        /// </summary>
        [JsonIgnore, ColumnName("BodyLoc1")]
        public string BodyLocation1 { get; set; } = string.Empty;

        /// <inheritdoc cref="BodyLocation2"/>
        [JsonIgnore, ColumnName("BodyLoc2")]
        public string BodyLocation2 { get; set; } = string.Empty;

        /// <summary>
        /// Points to the index of another Item Type as the required
        /// equipped Item Type to be used as ammo
        /// </summary>
        [JsonIgnore, ColumnName]
        public string Shoots { get; set; } = string.Empty;

        /// <summary>
        /// Points to the index of another Item Type as the required
        /// equipped Item Type to be used as this ammo’s weapon
        /// </summary>
        [JsonIgnore, ColumnName]
        public string Quiver { get; set; } = string.Empty;

        /// <summary>
        /// <para>If set, then it determines that this item is a throwing weapon.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Throwable { get; set; }

        /// <summary>
        /// <para>If set, then the item (considered ammo in this case) will
        /// be automatically transferred from the inventory to the required
        /// "BodyLoc" when another item runs out of that specific ammo.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Reload { get; set; }

        /// <summary>
        /// <para>If set, then the item in the inventory will replace a
        /// matching equipped item if that equipped item was destroyed.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool ReEquip { get; set; }

        /// <summary>
        /// <para>If set, then if the player picks up a matching Item Type,
        /// then they will try to automatically stack together.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool AutoStack { get; set; }

        /// <summary>
        /// <para>If set, then this item will always have the Magic quality
        /// (unless it is a Quest item).</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Magic { get; set; }

        /// <summary>
        /// <para>If set, then this item can spawn as a Rare quality.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Rare { get; set; }

        /// <summary>
        /// <para>If set, then this item will always have the Normal quality.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Normal { get; set; }

        /// <summary>
        /// <para>If set, then this item can be placed in the character’s belt slots.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool Beltable { get; set; }

        /// <summary>
        /// <para>Determines the maximum possible number of sockets that can be
        /// spawned on the item when the item level is greater than or equal to
        /// 1 and less than or equal to the <see cref="MaxSocketsLevelThreshold1"/>
        /// value.</para>
        /// <para>The number of sockets is capped by the <see cref="Equipment.GemSockets"/>
        /// value from the weapons.txt/armor.txt/misc.txt file.</para>
        /// </summary>
        /// <remarks>
        /// <para>Maximum sockets for iLvl 1-25.</para>
        /// <para>The range is hardcoded but the location is known,
        /// so you can alter around the range to your liking.</para>
        /// <para>On normal, items dropped from monsters are limited to 3,
        /// on nightmare to 4 and
        /// on hell to 6 sockets, irregardless of this columns content.</para>
        /// </remarks>
        [JsonIgnore, ColumnName("MaxSockets1", "MaxSock1")]
        public int? MaxSockets1 { get; set; }

        /// <summary>
        /// Defines the item level threshold between using the
        /// <see cref="MaxSockets1"/> and <see cref="MaxSockets2"/> field.
        /// </summary>
        [JsonIgnore, ColumnName("MaxSocketsLevelThreshold1")]
        public int? MaxSocketsLevelThreshold1 { get; set; }

        /// <summary>
        /// <para>Determines the maximum possible number of sockets that
        /// can be spawned on the item when the item level is greater than
        /// the <see cref="MaxSocketsLevelThreshold1"/> and less than or equal
        /// to the <see cref="MaxSocketsLevelThreshold2"/> value.</para>
        /// <para>The number of sockets is capped by the <see cref="Equipment.GemSockets"/>
        /// value from the weapons.txt/armor.txt/misc.txt file.</para>
        /// </summary>
        /// <remarks>
        /// <para>Maximum sockets for iLvl 26-40.</remarks>para>
        /// <para>The range is hardcoded but the location is known,
        /// so you can alter around the range to your liking.</para>
        /// <para>On normal, items dropped from monsters are limited to 3,
        /// on nightmare to 4 and
        /// on hell to 6 sockets, irregardless of this columns content.</para>
        /// </remarks>
        [JsonIgnore, ColumnName("MaxSockets2", "MaxSock25")]
        public int? MaxSockets2 { get; set; }

        /// <summary>
        /// Defines the item level threshold between using the
        /// <see cref="MaxSockets2"/> and <see cref="MaxSockets3"/> field.
        /// </summary>
        [JsonIgnore, ColumnName("MaxSocketsLevelThreshold2")]
        public int? MaxSocketsLevelThreshold2 { get; set; }

        /// <summary>
        /// <para>Determines the maximum possible number of sockets that
        /// can be spawned on the item when the item level is greater than
        /// <see cref="MaxSocketsLevelThreshold2"/> value.</para>
        /// <para>The number of sockets is capped by the <see cref="Equipment.GemSockets"/>
        /// value from the weapons.txt/armor.txt/misc.txt file.</para>
        /// </summary>
        /// <remarks>
        /// <para>Maximum sockets for iLvl 40+.</para>
        /// <para>The range is hardcoded but the location is known,
        /// so you can alter around the range to your liking.</para>
        /// <para>On normal, items dropped from monsters are limited to 3,
        /// on nightmare to 4 and
        /// on hell to 6 sockets, irregardless of this columns content.</para>
        /// </remarks>
        [JsonIgnore, ColumnName("MaxSockets3", "MaxSock40")]
        public int? MaxSockets3 { get; set; }

        /// <summary>
        /// <para>If set, then allow this Item Type to be used in default treasure classes.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public bool TreasureClass { get; set; }

        /// <summary>
        /// <para>Determines the chance for the item to spawn with stats,
        /// when created as a random Weapon/Armor/Misc item</para>
        /// <para>
        /// Used in the following formula:
        /// <code>
        /// IF RANDOM(0, (["Rarity"] - [Current Act Level])) > 0, THEN spawn stats
        /// </code>
        /// </para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? Rarity { get; set; }

        /// <summary>
        /// Determines if the Item Type should have class specific item
        /// skill modifiers
        /// </summary>
        [JsonIgnore, ColumnName]
        public CharacterClass? StaffMods { get; set; }

        /// <summary>
        /// Determines if this item should be useable only by a specific class
        /// </summary>
        [JsonIgnore, ColumnName]
        public CharacterClass? Class { get; set; }

        /// <summary>
        /// Tracks the number of inventory graphics used for this item type.
        /// This number much match the number of "InvGfx" fields used.
        /// </summary>
        [JsonIgnore, ColumnName("VarInvGfx")]
        public int? VarInventoryGraphic { get; set; }

        /// <summary>
        /// <para>Defines a DC6 file to use for the item’s inventory graphics.</para>
        /// <para>The amount of this fields used should match the value used
        /// in <see cref="VarInventoryGraphic"/></para>
        /// </summary>
        [JsonIgnore, ColumnName("InvGfx1")]
        public string InventoryGraphic1 { get; set; } = string.Empty;

        /// <inheritdoc cref="InventoryGraphic1"/>
        [JsonIgnore, ColumnName("InvGfx2")]
        public string InventoryGraphic2 { get; set; } = string.Empty;

        /// <inheritdoc cref="InventoryGraphic1"/>
        [JsonIgnore, ColumnName("InvGfx3")]
        public string InventoryGraphic3 { get; set; } = string.Empty;

        /// <inheritdoc cref="InventoryGraphic1"/>
        [JsonIgnore, ColumnName("InvGfx4")]
        public string InventoryGraphic4 { get; set; } = string.Empty;

        /// <inheritdoc cref="InventoryGraphic1"/>
        [JsonIgnore, ColumnName("InvGfx5")]
        public string InventoryGraphic5 { get; set; } = string.Empty;

        /// <inheritdoc cref="InventoryGraphic1"/>
        [JsonIgnore, ColumnName("InvGfx6")]
        public string InventoryGraphic6 { get; set; } = string.Empty;

        /// <summary>
        /// Uses a code to determine which UI tab page on the NPC shop
        /// UI to display this Item Type, such as after it is
        /// sold to the NPC.
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StorePage { get; set; } = string.Empty;

        [JsonIgnore]
        public string[] Categories { get; set; } = [];
        #endregion Properties

        public override string ToString()
        {
            return Index;
        }

        static void PostImport(Dictionary<string, ItemType> Items)
        {
            foreach (KeyValuePair<string, ItemType> item in Items)
            {
                Items[item.Key].Categories = [.. item.Value.ResolveCategories(Items)];
            }
        }

        string[] ResolveCategories(Dictionary<string, ItemType> Items)
        {
            List<string> result = [];
            result.Add(Code);

            if (!string.IsNullOrEmpty(Equiv1) && Items.TryGetValue(Equiv1, out ItemType eq1))
            {
                result.AddRange(eq1.ResolveCategories(Items));
            }

            if (!string.IsNullOrEmpty(Equiv2) && Items.TryGetValue(Equiv2, out ItemType eq2))
            {
                result.AddRange(eq2.ResolveCategories(Items));
            }

            return [.. result];
        }
    }
}
