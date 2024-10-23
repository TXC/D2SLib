using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using D2SImporter.Attributes;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    /*
    /// <summary>
    /// <para>If set, </para>
    /// <para>If not set, </para>
    /// </summary>
    */
    public class Equipment : ICloneable
    {
        #region Properties
        public EquipmentType EquipmentType { get; set; }
        /// <summary>
        /// <see cref="NameStr"/>
        /// </summary>
        [JsonIgnore, ColumnName("namestr"), Translatable]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// <para>Defines a unique 3 letter/number code for the item.</para>
        /// <para>This is used as an identifier to reference the item.</para>
        /// </summary>
        [JsonIgnore, ColumnName("code"), Key]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Defines the amount of the Strength attribute needed to use the item
        /// </summary>
        [JsonIgnore, ColumnName("reqstr")]
        public int? RequiredStrength { get; set; }

        /// <summary>
        /// Defines the amount of the Dexterity attribute needed to use the item
        /// </summary>
        [JsonIgnore, ColumnName("reqdex")]
        public int? RequiredDexterity { get; set; }

        /// <summary>
        /// Defines the base durability amount that the item will spawn with.
        /// </summary>
        [JsonIgnore, ColumnName("durability")]
        public int? Durability { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore, ColumnName("level")]
        public int ItemLevel { get; set; }

        /// <summary>
        /// Points to an Item Type defined in the ItemTypes.txt file,
        /// which controls how the item functions
        /// </summary>
        [JsonIgnore, ColumnName("type")]
        public ItemType Type { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, string>), typeof(Equipment), nameof(ImportRequiredClass))]
        public string RequiredClass { get; set; } = string.Empty;

        /// <summary>
        /// Defines which game version to create this item
        /// (0 = Classic mode | 100 = Expansion mode)
        /// </summary>
        [JsonIgnore, ColumnName("version")]
        public int? Version { get; set; }

        /// <summary>
        /// <para>If set, then only the item’s base stats will be stored in
        /// the character save, but not any modifiers or additional stats</para>
        /// <para>If not set, then all of the items stats will be saved.</para>
        /// </summary>
        [JsonIgnore, ColumnName("compactsave")]
        public bool CompactSave { get; set; }

        /// <summary>
        /// <para>Determines the chance that the item will randomly spawn (1/#).</para>
        /// <para>The higher the value then the rarer the item will be.</para>
        /// <para>This field depends on the <see cref="Spawnable"/> field being enabled,
        /// the <see cref="Quest"/> field being disabled, and the item level being
        /// less than or equal to the area level.</para>
        /// <para>This value is also affected by the relative Act number that
        /// the item is dropping in, where the higher the Act number, then the
        /// more common the item will drop.</para>
        /// </summary>
        [JsonIgnore, ColumnName("rarity")]
        public int? Rarity { get; set; }

        /// <summary>
        /// <para>If set, then this item can be randomly spawned.</para>
        /// <para>If not set, then this item will never randomly spawn.</para>
        /// </summary>
        [JsonIgnore, ColumnName("spawnable")]
        public bool Spawnable { get; set; }

        /// <summary>
        /// <para>If the item type is an armor, then this will affect the
        /// Walk/Run Speed reduction when wearing the armor.</para>
        /// <para>If the item type is a weapon, then this will affect the
        /// Attack Speed reduction when wearing the weapon.</para>
        /// </summary>
        [JsonIgnore, ColumnName("speed")]
        public int? Speed { get; set; }

        /// <summary>
        /// <para>If set, then the item will not have durability.</para>
        /// <para>If not set, then the item will have durability.</para>
        /// </summary>
        [JsonIgnore, ColumnName("nodurability")]
        public bool NoDurability { get; set; }

        /// <summary>
        /// Controls the base item level.
        /// This is used for determining when the item is allowed to drop,
        /// such as making sure that the item level is not greater than
        /// the monster’s level or the area level.
        /// </summary>
        [JsonIgnore, ColumnName("level")]
        public int? Level { get; set; }

        /// <summary>
        /// <para>If set, then display the item level next to the item name.</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName("ShowLevel")]
        public bool ShowLevel { get; set; }

        /// <summary>
        /// Controls the player level requirement for being able to use the item
        /// </summary>
        [JsonIgnore, ColumnName("levelreq")]
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Defines the base gold cost of the item when being sold by an NPC.
        /// This can be affected by item modifiers and the rarity of the item.
        /// </summary>
        [JsonIgnore, ColumnName("cost")]
        public int? Cost { get; set; }

        /// <summary>
        /// Defines the gambling gold cost of the item on the Gambling UI
        /// </summary>
        [JsonIgnore, ColumnName("gamble cost")]
        public int? GambleCost { get; set; }

        /// <summary>
        /// String Key that is used for the base item name
        /// </summary>
        [JsonIgnore, ColumnName("namestr")]
        public string NameStr { get; set; } = string.Empty;

        /// <summary>
        /// Defines the magic level of the item, which can affect
        /// how magical item modifiers that can appear on the item
        /// <para>(See automagic.txt)</para>
        /// </summary>
        [JsonIgnore, ColumnName("magic lvl")]
        public int? MagicLevel { get; set; }

        /// <summary>
        /// <para>Automatically picks an item affix name from a designated
        /// "group" value from the automagic.txt file, instead of using random
        /// prefixes.</para>
        /// <para>This is only used when the item is Magical quality.</para>
        /// </summary>
        [JsonIgnore, ColumnName("auto prefix")]
        public int? AutoPrefix { get; set; }

        /// <summary>
        /// Uses a unique 3 letter/number code similar to the defined
        /// "code" fields to determine what in-game graphics to display
        /// on the player character when the item is equipped
        /// </summary>
        [JsonIgnore, ColumnName("alternategfx")]
        public string AlternateGraphics { get; set; } = string.Empty;

        /// <summary>
        /// Links to a "code" field to determine the normal version of the item
        /// </summary>
        [JsonIgnore, ColumnName("normcode")]
        public string NormCode { get; set; } = string.Empty;

        /// <summary>
        /// Links to a "code" field to determine the Exceptional version of the item
        /// </summary>
        [JsonIgnore, ColumnName("ubercode")]
        public string UberCode { get; set; } = string.Empty;

        /// <summary>
        /// Links to a "code" field to determine the Elite  version of the item
        /// </summary>
        [JsonIgnore, ColumnName("ultracode")]
        public string UltraCode { get; set; } = string.Empty;

        /// <summary>
        /// Determines the layer of player animation when the item is equipped.
        /// This uses a code referenced from the Composit.txt file.
        /// </summary>
        [JsonIgnore, ColumnName("component")]
        public int? Component { get; set; }

        /// <summary>
        /// Defines the width and height of grid cells that the item occupies in the player inventory
        /// </summary>
        [JsonIgnore, ColumnName("invwidth")]
        public int? InventoryWidth { get; set; }

        /// <inheritdoc cref="InventoryWidth" />
        [JsonIgnore, ColumnName("invheight")]
        public int? InventoryHeight { get; set; }

        /// <summary>
        /// <para>If set, then the item will have its own inventory
        /// allowing for the capability to socket gems, runes, or jewels.</para>
        /// <para>If not set, then the item cannot have sockets.</para>
        /// </summary>
        [JsonIgnore, ColumnName("hasinv")]
        public bool HasInventory { get; set; }

        /// <summary>
        /// <para>Controls the maximum number of sockets allowed on this item.</para>
        /// <para>This is limited by the item’s size based on the "invwidth"
        /// and "invheight" fields.</para>
        /// <para>This also compares with the
        /// <see cref="ItemType.MaxSockets1">MaxSock1</see>,
        /// <see cref="ItemType.MaxSockets2">MaxSock25</see> and
        /// <see cref="ItemType.MaxSockets3">MaxSock40</see>
        /// fields from the ItemTypes.txt file.</para>
        /// </summary>
        [JsonIgnore, ColumnName("gemsockets")]
        public int? GemSockets { get; set; }

        /// <summary>
        /// <para>Determines which affect from a gem or rune will be
        /// applied when it is socketed into this item</para>
        /// <list type="bullet">
        /// <item><term>0</term><description>Weapon</description></item>
        /// <item><term>1</term><description>Armor or Helmet</description></item>
        /// <item><term>2</term><description>Shield</description></item>
        /// </list>
        /// </summary>
        [JsonIgnore, ColumnName("gemapplytype")]
        public GemApplyType? GemApplyType { get; set; }

        /// <summary>
        /// Controls which DC6 file to use for displaying the item
        /// in the game world when it is dropped on the ground
        /// (uses the file name as the input)
        /// </summary>
        [JsonIgnore, ColumnName("flippyfile")]
        public string FlippyFile { get; set; } = string.Empty;

        /// <summary>
        /// Controls which DC6 file to use for displaying the item
        /// graphics in the inventory (uses the file name as the input)
        /// </summary>
        [JsonIgnore, ColumnName("invfile")]
        public string InventoryFile { get; set; } = string.Empty;

        /// <summary>
        /// Controls which DC6 file to use for displaying the item
        /// graphics in the inventory when it is a Unique quality item
        /// (uses the file name as the input)
        /// </summary>
        [JsonIgnore, ColumnName("uniqueinvfile")]
        public string UniqueInventoryFile { get; set; } = string.Empty;

        /// <summary>
        /// Controls which DC6 file to use for displaying the item
        /// graphics in the inventory when it is a Set quality item
        /// (uses the file name as the input)
        /// </summary>
        [JsonIgnore, ColumnName("setinvfile")]
        public string SetInventoryFile { get; set; } = string.Empty;

        /// <summary>
        /// <para>If set, then the item can be used with the right-click
        /// mouse button command (this only works with specific belt
        /// items or quest items).</para>
        /// <para>If not set, then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName("useable")]
        public bool Useable { get; set; }

        /// <summary>
        /// <para>If set, then the item will use a quantity field and
        /// handle stacking functionality.</para>
        /// <para>This can depend on if the item type is throwable,
        /// is a type of ammunition, or is some other kind of
        /// miscellaneous item.</para>
        /// <para>If not set, then the item cannot be stacked.</para>
        /// </summary>
        [JsonIgnore, ColumnName("stackable")]
        public bool Stackable { get; set; }

        /// <summary>
        /// <para>Controls the minimum stack count or quantity that is allowed on the item.</para>
        /// <para>This field depends on the <see cref="Stackable"/> field being enabled.</para>
        /// </summary>
        [JsonIgnore, ColumnName("minstack")]
        public int? MinStack { get; set; }

        /// <summary>
        /// <para>Controls the maximum stack count or quantity that is allowed on the item.</para>
        /// <para>This field depends on the <see cref="Stackable"/> field being enabled.</para>
        /// </summary>
        [JsonIgnore, ColumnName("maxstack")]
        public int? MaxStack { get; set; }

        /// <summary>
        /// <para>Controls the stack count or quantity that the item can spawn with.</para>
        /// <para>This field depends on the <see cref="Stackable"/> field being enabled.</para>
        /// </summary>
        [JsonIgnore, ColumnName("spawnstack")]
        public int? SpawnStack { get; set; }

        /// <summary>
        /// <para>If set, then the item will use the transmogrify function.</para>
        /// <para>If not set, then ignore this.</para>
        /// <para>This field depends on the <see cref="Useable"/> field being enabled.</para>
        /// </summary>
        [JsonIgnore, ColumnName("Transmogrify")]
        public bool Transmogrify { get; set; }

        /// <summary>
        /// Links to a "code" field to determine which item is
        /// chosen to transmogrify this item to.
        /// </summary>
        [JsonIgnore, ColumnName("TMogType")]
        public int? TransmogrifyType { get; set; }

        /// <summary>
        /// <para>Controls the minimum quantity that the transmogrify
        /// item will have.</para>
        /// <para>This depends on what item was chosen in the
        /// "<see cref="TransmogrifyType">TMogType</see>" field,
        /// and that the transmogrify item has quantity.</para>
        /// </summary>
        [JsonIgnore, ColumnName("TMogMin")]
        public int? TransmogrifyMin { get; set; }

        /// <summary>
        /// <para>Controls the maximum quantity that the transmogrify
        /// item will have.</para>
        /// <para>This depends on what item was chosen in
        /// the "<see cref="TransmogrifyType">TMogType</see>" field,
        /// and that the transmogrify item has quantity.</para>
        /// </summary>
        [JsonIgnore, ColumnName("TMogMax")]
        public int? TransmogrifyMax { get; set; }

        /// <summary>
        /// Points to a secondary Item Type defined in the ItemTypes.txt file,
        /// which controls how the item functions.
        /// This is optional but can add more functionalities and
        /// possibilities with the item.
        /// </summary>
        [JsonIgnore, ColumnName("type2")]
        public ItemType? Type2 { get; set; }

        /// <summary>
        /// Points to sound defined in the sounds.txt file.
        /// Used when the item is dropped on the ground.
        /// </summary>
        [JsonIgnore, ColumnName("dropsound")]
        public string DropSound { get; set; } = string.Empty;

        /// <summary>
        /// Defines which frame in the "flippyfile" animation to play
        /// the "dropsound" sound when the item is dropped on the ground.
        /// </summary>
        [JsonIgnore, ColumnName("dropsfxframe")]
        public int? DropSfxFrame { get; set; }

        /// <summary>
        /// Points to sound defined in the sounds.txt file.
        /// Used when the item is moved in the inventory or used.
        /// </summary>
        [JsonIgnore, ColumnName("usesound")]
        public string? UseSound { get; set; }

        /// <summary>
        /// <para>If set, then the item can only spawn as a
        /// Unique quality type.</para>
        /// <para>If not set, then the item can spawn as other
        /// quality types.</para>
        /// </summary>
        [JsonIgnore, ColumnName("unique")]
        public bool Unique { get; set; }

        /// <summary>
        /// <para>If set, then the item will be drawn transparent on
        /// the player model (similar to ethereal models).</para>
        /// <para>If not set, then the item will appear solid on the
        /// player model.</para>
        /// </summary>
        [JsonIgnore, ColumnName("transparent")]
        public bool Transparent { get; set; }

        /// <summary>
        /// <para>Controls the color palette change of the item for the character model graphics</para>
        /// <para>Bit Offset</para>
        /// <list type="bullet">
        /// <item><term>0</term><description>Transparency at 25%</description></item>
        /// <item><term>1</term><description>Transparency at 50%</description></item>
        /// <item><term>2</term><description>Transparency at 75%</description></item>
        /// <item><term>3</term><description>Black Alpha Transparency</description></item>
        /// <item><term>4</term><description>White Alpha Transparency</description></item>
        /// <item><term>5</term><description>No Transparency</description></item>
        /// <item><term>6</term><description>Dark Transparency (Unused)</description></item>
        /// <item><term>7</term><description>Highlight Transparency (Used when mousing over the unit)</description></item>
        /// <item><term>8</term><description>Blended</description></item>
        /// </list>
        /// </summary>
        [JsonIgnore, ColumnName("transtbl")]
        public int? TransparencyTable { get; set; }

        //[JsonIgnore]
        //public bool Quivered { get; set; }

        /// <summary>
        /// Controls the value of the light radius that this item
        /// can apply on the monster.
        /// This only affects monsters with this item equipped,
        /// not other types of units.
        /// This is ignored if the item’s component on the monster
        /// is "lit", "med", or "hvy".
        /// </summary>
        [JsonIgnore, ColumnName("lightradius")]
        public int? LightRadius { get; set; }

        /// <summary>
        /// Controls which belt type to use for belt items only.
        /// This field determines what index entry in the belts.txt file to use.
        /// </summary>
        [JsonIgnore, ColumnName("belt")]
        public int? Belt { get; set; }

        /// <summary>
        /// <para>
        /// Controls what quest class is tied to the item which can
        /// enable certain item functionalities for a specific quest.
        /// Any value greater than 0 will also mean the item is
        /// flagged as a quest item, which can affect how it is displayed
        /// in tooltips, how it is traded with other players, its item
        /// rarity, and how it cannot be sold to an NPC.</para>
        /// <para>If equals 0, then the item will not be flagged as a quest item.</para>
        /// <item><term>0</term><description>Not a quest item</description></item>
        /// <item><term>1</term><description>Act 1 Prologue</description></item>
        /// <item>
        /// <term>2</term>
        /// <description>Den of Evil</description>
        /// </item>
        /// <item>
        /// <term>3</term>
        /// <description>Sisters’ Burial Grounds</description>
        /// </item>
        /// <item>
        /// <term>4</term>
        /// <description>Tools of the Trade</description>
        /// </item>
        /// <item>
        /// <term>5</term>
        /// <description>The Search for Cain</description>
        /// </item>
        /// <item>
        /// <term>6</term>
        /// <description>The Forgotten Tower</description>
        /// </item>
        /// <item>
        /// <term>7</term>
        /// <description>Sisters to the Slaughter</description>
        /// </item>
        /// <item>
        /// <term>8</term>
        /// <description>Act 2 Prologue</description>
        /// </item>
        /// <item>
        /// <term>9</term>
        /// <description>Radament’s Lair</description>
        /// </item>
        /// <item>
        /// <term>10</term>
        /// <description>The Horadric Staff</description>
        /// </item>
        /// <item>
        /// <term>11</term>
        /// <description>The Tainted Sun</description>
        /// </item>
        /// <item>
        /// <term>12</term>
        /// <description>The Arcane Sanctuary</description>
        /// </item>
        /// <item>
        /// <term>13</term>
        /// <description>The Summoner</description>
        /// </item>
        /// <item>
        /// <term>14</term>
        /// <description>The Seven Tombs</description>
        /// </item>
        /// <item>
        /// <term>15</term>
        /// <description>Act 2 Traversed</description>
        /// </item>
        /// <item>
        /// <term>16</term>
        /// <description>Lam Esen’s Tome</description>
        /// </item>
        /// <item>
        /// <term>17</term>
        /// <description>Khalim’s Will</description>
        /// </item>
        /// <item>
        /// <term>18</term>
        /// <description>Blade of the Old Religion</description>
        /// </item>
        /// <item>
        /// <term>19</term>
        /// <description>The Golden Bird</description>
        /// </item>
        /// <item>
        /// <term>20</term>
        /// <description>The Blackened Temple</description>
        /// </item>
        /// <item>
        /// <term>21</term>
        /// <description>The Guardian</description>
        /// </item>
        /// <item>
        /// <term>22</term>
        /// <description>Act 4 Prologue</description>
        /// </item>
        /// <item>
        /// <term>23</term>
        /// <description>The Fallen Angel</description>
        /// </item>
        /// <item>
        /// <term>24</term>
        /// <description>Terror’s End</description>
        /// </item>
        /// <item>
        /// <term>25</term>
        /// <description>The Hellforge</description>
        /// </item>
        /// <item>
        /// <term>26</term>
        /// <description>Rogue Warning</description>
        /// </item>
        /// <item>
        /// <term>27</term>
        /// <description>Guard in Town Warning</description>
        /// </item>
        /// <item>
        /// <term>28</term>
        /// <description>Guard in Desert Warning</description>
        /// </item>
        /// <item>
        /// <term>29</term>
        /// <description>Dark Wanderer Seen</description>
        /// </item>
        /// <item>
        /// <term>30</term>
        /// <description>Angel Warning</description>
        /// </item>
        /// <item>
        /// <term>31</term>
        /// <description>Respec from Akara Complete Act 5 Prologue</description>
        /// </item>
        /// <item>
        /// <term>32</term>
        /// <description>Siege on Harrogath</description>
        /// </item>
        /// <item>
        /// <term>33</term>
        /// <description>Rescue on Mount Arreat</description>
        /// </item>
        /// <item>
        /// <term>34</term>
        /// <description>Prison of Ice</description>
        /// </item>
        /// <item>
        /// <term>35</term>
        /// <description>Betrayal of Harrogath</description>
        /// </item>
        /// <item>
        /// <term>36</term>
        /// <description>Rite of Passage</description>
        /// </item>
        /// <item>
        /// <term>37</term>
        /// <description>Eve of Destruction</description>
        /// </item>
        /// </summary>
        [JsonIgnore, ColumnName("quest")]
        public int? Quest { get; set; }

        /// <summary>
        /// If set and <see cref="Quest"/> field has a value,
        /// then the game will check the current difficulty
        /// setting and will tie that difficulty setting to
        /// the quest item.
        /// This means that the player can have more than 1 of
        /// the same quest item as long each they are obtained
        /// per difficulty mode (Normal / Nightmare / Hell).
        /// If not set and <see cref="Quest"/> field has a value,
        /// then the player can only have 1 count of the quest item
        /// in the inventory, regardless of difficulty.
        /// </summary>
        [JsonIgnore, ColumnName("questdiffcheck")]
        public bool? QuestDiffCheck { get; set; }

        /// <summary>
        /// Points to the "Id" field from the Missiles.txt file,
        /// which determines what type of missile is used when
        /// using the throwing weapons
        /// </summary>
        [JsonIgnore, ColumnName("missiletype")]
        public int? MissileType { get; set; }

        /// <summary>
        /// Controls the threshold value for durability to display
        /// the low durability warning UI.
        /// This is only used if the item has durability.
        /// </summary>
        [JsonIgnore, ColumnName("durwarning")]
        public int? DurabilityWarning { get; set; }

        /// <summary>
        /// Controls the threshold value for quantity to display
        /// the low quantity warning UI.
        /// This is only used if the item has stacks.
        /// </summary>
        [JsonIgnore, ColumnName("qntwarning")]
        public int? QuantityWarning { get; set; }

        /// <summary>
        /// The minimum physical damage provided by the item
        /// </summary>
        [JsonIgnore, ColumnName("mindam")]
        public int? MinDamage { get; set; }

        /// <summary>
        /// The maximum physical damage provided by the item
        /// </summary>
        [JsonIgnore, ColumnName("maxdam")]
        public int? MaxDamage { get; set; }

        /// <summary>
        /// The percentage multiplier that gets multiplied the player’s
        /// current Strength attribute value to modify the bonus damage
        /// percent from the equipped item.
        /// If this equals 1, then default the value to 100.
        /// </summary>
        [JsonIgnore, ColumnName("StrBonus")]
        public int? StrengthBonus { get; set; }

        /// <summary>
        /// The percentage multiplier that gets multiplied the player’s
        /// current Dexterity attribute value to modify the bonus damage
        /// percent from the equipped item.
        /// If this equals 1, then default the value to 100.
        /// </summary>
        [JsonIgnore, ColumnName("DexBonus")]
        public int? DexterityBonus { get; set; }

        /// <summary>
        /// Determines the starting index offset for reading the gems.txt
        /// file when determining what effects gems or runes will have
        /// the item based on the “gemapplytype” field.
        /// For example, if this value equals 9, then the game will start
        /// with index 9 ("Chipped Emerald") and ignore the previously
        /// defined gems in the gems.txt file, which can mean that those
        /// ignored gems will not apply modifiers when socketed into the item.
        /// </summary>
        [JsonIgnore, ColumnName("gemoffset")]
        public int? GemOffset { get; set; }

        /// <summary>
        /// <para>Controls the color palette change of the item for the character model graphics</para>
        /// <para>Bit Offset</para>
        /// <list type="bullet">
        /// <item><term>1</term>
        /// <description><c>0b0001</c> Allow the item to be capable of having Magic quality</description></item>
        /// <item><term>2</term>
        /// <description><c>0b0010</c> The item is classified as metal</description></item>
        /// <item><term>4</term>
        /// <description><c>0b0100</c> The item is classified as a spellcaster item</description></item>
        /// <item><term>8</term>
        /// <description><c>0b1000</c> The item is classified as a skill based item</description></item>
        /// </list>
        /// </summary>
        [JsonIgnore, ColumnName("bitfield1")]
        public int? BitField1 { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<Equipment, Dictionary<string, string>, NPCVendors>), typeof(Equipment), nameof(ImportVendors))]
        public NPCVendors Vendors { get; set; } = new();

        /// <summary>
        /// <para>Controls the color palette change of the item for the character model graphics</para>
        /// <list type="bullet">
        /// <item><term>0</term>
        /// <description>No color change</description></item>
        /// <item><term>1</term>
        /// <description>Light Grey</description></item>
        /// <item><term>2</term>
        /// <description>Dark Grey</description></item>
        /// <item><term>3</term>
        /// <description>Gold</description></item>
        /// <item><term>4</term>
        /// <description>Brown</description></item>
        /// <item><term>5</term>
        /// <description>Grey Brown</description></item>
        /// <item><term>6</term>
        /// <description>Inventory Grey</description></item>
        /// <item><term>7</term>
        /// <description>Inventory Grey 2</description></item>
        /// <item><term>8</term>
        /// <description>Inventory Grey Brown</description></item>
        /// </list>
        /// </summary>
        [JsonIgnore, ColumnName("Transform")]
        public int? Transform { get; set; }

        /// <summary>
        /// <para>Controls the color palette change of the item for the inventory graphics</para>
        /// To decode see <see cref="Transform">Transform</see>
        /// </summary>
        [JsonIgnore, ColumnName("InvTrans")]
        public int? InventoryTransform { get; set; }

        /// <summary>
        /// <para>When set and the item is Unique rarity, then skip adding
        /// the item’s base name in its title.</para>
        /// <para>If not set then ignore this.</para>
        /// </summary>
        [JsonIgnore, ColumnName("SkipName")]
        public int? SkipName { get; set; }

        /// <summary>
        /// Links to another item’s “code” field.
        /// Used to determine which item will replace this item
        /// when being generated in the NPC’s store while the
        /// game is playing in Nightmare difficulty.
        /// If this field’s code equals “xxx”, then this item
        /// will not change in this difficulty.
        /// </summary>
        [JsonIgnore, ColumnName("NightmareUpgrade")]
        public string NightmareUpgrade { get; set; } = string.Empty;

        /// <summary>
        /// Links to another item’s “code” field.
        /// Used to determine which item will replace this item
        /// when being generated in the NPC’s store while the
        /// game is playing in Hell difficulty.
        /// If this field’s code equals “xxx”, then this item
        /// will not change in this difficulty.
        /// </summary>
        [JsonIgnore, ColumnName("HellUpgrade")]
        public string HellUpgrade { get; set; } = string.Empty;

        /// <summary>
        /// <para>When set the item’s name can be personalized by Anya for the Act 5 Betrayal of Harrogath quest reward.</para>
        /// <para>If not set then the item cannot be used for the personalized name reward.</para>
        /// </summary>
        [JsonIgnore, ColumnName("Nameable")]
        public bool Nameable { get; set; }

        /// <summary>
        /// <para>When set this item will always appear on the NPC’s store.</para>
        /// <para>If not set then the item will randomly appear on the NPC’s store when appropriate.</para>
        /// </summary>
        [JsonIgnore, ColumnName("PermStoreItem")]
        public bool PermStoreItem { get; set; }

        /// <summary>
        /// <para>The amount of weight added to the diablo clone progress when this
        /// item is sold.</para>
        /// <para>When offline, selling this item will instead immediately spawn
        /// diablo clone.</para>
        /// </summary>
        [JsonIgnore, ColumnName("diablocloneweight")]
        public int? DiabloCloneWeight { get; set; }
        #endregion Properties

        public object Clone()
        {
            return new Equipment
            {
                EquipmentType = this.EquipmentType,
                Code = this.Code,
                RequiredStrength = this.RequiredStrength,
                RequiredDexterity = this.RequiredDexterity,
                Durability = this.Durability,
                ItemLevel = this.ItemLevel,
                Type = this.Type,
                Version = this.Version,
                CompactSave = this.CompactSave,
                Rarity = this.Rarity,
                Spawnable = this.Spawnable,
                Speed = this.Speed,
                NoDurability = this.NoDurability,
                Level = this.Level,
                ShowLevel = this.ShowLevel,
                RequiredLevel = this.RequiredLevel,
                Cost = this.Cost,
                GambleCost = this.GambleCost,
                NameStr = this.NameStr,
                MagicLevel = this.MagicLevel,
                AutoPrefix = this.AutoPrefix,
                AlternateGraphics = this.AlternateGraphics,
                NormCode = this.NormCode,
                UberCode = this.UberCode,
                UltraCode = this.UltraCode,
                Component = this.Component,
                InventoryWidth = this.InventoryWidth,
                InventoryHeight = this.InventoryHeight,
                HasInventory = this.HasInventory,
                GemSockets = this.GemSockets,
                GemApplyType = this.GemApplyType,
                FlippyFile = this.FlippyFile,
                InventoryFile = this.InventoryFile,
                UniqueInventoryFile = this.UniqueInventoryFile,
                SetInventoryFile = this.SetInventoryFile,
                Useable = this.Useable,
                Stackable = this.Stackable,
                MinStack = this.MinStack,
                MaxStack = this.MaxStack,
                SpawnStack = this.SpawnStack,
                Transmogrify = this.Transmogrify,
                TransmogrifyType = this.TransmogrifyType,
                TransmogrifyMin = this.TransmogrifyMin,
                TransmogrifyMax = this.TransmogrifyMax,
                Type2 = this.Type2,
                DropSound = this.DropSound,
                DropSfxFrame = this.DropSfxFrame,
                UseSound = this.UseSound,
                Unique = this.Unique,
                Transparent = this.Transparent,
                TransparencyTable = this.TransparencyTable,
                LightRadius = this.LightRadius,
                Belt = this.Belt,
                Quest = this.Quest,
                QuestDiffCheck = this.QuestDiffCheck,
                MissileType = this.MissileType,
                DurabilityWarning = this.DurabilityWarning,
                QuantityWarning = this.QuantityWarning,
                MinDamage = this.MinDamage,
                MaxDamage = this.MaxDamage,
                StrengthBonus = this.StrengthBonus,
                DexterityBonus = this.DexterityBonus,
                GemOffset = this.GemOffset,
                BitField1 = this.BitField1,
                Vendors = this.Vendors,
                Transform = this.Transform,
                InventoryTransform = this.InventoryTransform,
                SkipName = this.SkipName,
                NightmareUpgrade = this.NightmareUpgrade,
                HellUpgrade = this.HellUpgrade,
                Nameable = this.Nameable,
                PermStoreItem = this.PermStoreItem,
                DiabloCloneWeight = this.DiabloCloneWeight,
            };
        }

        protected static void FillClassFromTable(Equipment cls, Dictionary<string, string> row)
        {
            var name = row["name"];

            if (Main.Importer.ItemTypes.TryGetValue(row["type"], out ItemType type))
            {
                cls.Type = type;
            }
            else
            {
                ExceptionHandler.LogException(new Exception($"Could not find type '{row["type"]}' in ItemTypes.txt for item '{name}' in textfile"));
            }

            if (row.TryGetValue("type2", out var type2))
            {
                if (Main.Importer.ItemTypes.TryGetValue(row["type2"], out ItemType itype2))
                {
                    cls.Type2 = itype2;
                }
                else if (string.IsNullOrEmpty(type2) == false)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find type2 '{type2}' in ItemTypes.txt for item '{name}'"));
                }
            }

            if (row.TryGetValue("code", out var code))
            {
                cls.Code = code;
            }
            else
            {
                ExceptionHandler.LogException(new Exception($"Could not find Code for item '{name}' in textfile"));
            }

            if (row.TryGetValue("reqstr", out var requiredStrength))
            {
                cls.RequiredStrength = Utility.ToNullableInt(requiredStrength);
            }
            else
            {
                //ExceptionHandler.LogException(new Exception($"Could not find Required Strength for item '{name}' in textfile"));
                cls.RequiredStrength = 0;
            }

            if (row.TryGetValue("reqdex", out var requiredDexterity))
            {
                cls.RequiredDexterity = Utility.ToNullableInt(requiredDexterity);
            }
            else
            {
                //ExceptionHandler.LogException(new Exception($"Could not find Required Dexterity for item '{name}' in textfile"));
                cls.RequiredDexterity = 0;
            }

            if (row.TryGetValue("durability", out var durability))
            {
                cls.Durability = Utility.ToNullableInt(durability);
            }
            else
            {
                //ExceptionHandler.LogException(new Exception($"Could not find Durability for item '{name}' in textfile"));
                cls.Durability = 0;
            }

            if (row.TryGetValue("level", out var itemLevel))
            {
                cls.ItemLevel = Utility.ToInt(itemLevel);
            }
            else
            {
                ExceptionHandler.LogException(new Exception($"Could not find Item Level for item '{name}' in textfile"));
            }

            if (row.TryGetValue("ShowLevel", out var showLevel))
            {
                cls.ShowLevel = Utility.ToBool(showLevel);
            }

            if (row.TryGetValue("version", out var version))
            {
                cls.Version = Utility.ToNullableInt(version);
            }

            if (row.TryGetValue("compactsave", out var compactsave))
            {
                cls.CompactSave = Utility.ToBool(compactsave);
            }

            if (row.TryGetValue("rarity", out var rarity))
            {
                cls.Rarity = Utility.ToNullableInt(rarity);
            }

            if (row.TryGetValue("spawnable", out var spawnable))
            {
                cls.Spawnable = Utility.ToBool(spawnable);
            }

            if (row.TryGetValue("speed", out var speed))
            {
                cls.Speed = Utility.ToNullableInt(speed);
            }

            if (row.TryGetValue("nodurability", out var nodurability))
            {
                cls.NoDurability = Utility.ToBool(nodurability);
            }

            if (row.TryGetValue("level", out var level))
            {
                cls.Level = Utility.ToNullableInt(level);
            }

            if (row.TryGetValue("levelreq", out var levelreq))
            {
                cls.RequiredLevel = Utility.ToInt(levelreq);
            }

            if (row.TryGetValue("cost", out var cost))
            {
                cls.Cost = Utility.ToNullableInt(cost);
            }

            if (row.TryGetValue("gamble cost", out var gambleCost))
            {
                cls.GambleCost = Utility.ToNullableInt(gambleCost);
            }

            if (row.TryGetValue("namestr", out var namestr))
            {
                cls.NameStr = namestr;
                if (Main.Importer.Table.TryGetValue(cls.NameStr, out string translatedName))
                {
                    cls.Name = translatedName;
                }
                else
                {
                    cls.Name = cls.NameStr;
                }
            }
            else
            {
                ExceptionHandler.LogException(new Exception($"Could not find NameStr for item '{name}' in textfile"));
            }

            if (row.TryGetValue("magic lvl", out var magicLvl))
            {
                cls.MagicLevel = Utility.ToNullableInt(magicLvl);
            }

            if (row.TryGetValue("auto prefix", out var autoPrefix))
            {
                cls.AutoPrefix = Utility.ToNullableInt(autoPrefix);
            }

            if (row.TryGetValue("alternategfx", out var alternategfx))
            {
                cls.AlternateGraphics = alternategfx;
            }

            if (row.TryGetValue("normcode", out var normcode))
            {
                cls.NormCode = normcode;
            }

            if (row.TryGetValue("ubercode", out var ubercode))
            {
                cls.UberCode = ubercode;
            }

            if (row.TryGetValue("ultracode", out var ultracode))
            {
                cls.UltraCode = ultracode;
            }

            if (row.TryGetValue("component", out var component))
            {
                cls.Component = Utility.ToNullableInt(component);
            }

            if (row.TryGetValue("invwidth", out var invwidth))
            {
                cls.InventoryWidth = Utility.ToNullableInt(invwidth);
            }

            if (row.TryGetValue("invheight", out var invheight))
            {
                cls.InventoryHeight = Utility.ToNullableInt(invheight);
            }

            if (row.TryGetValue("hasinv", out var hasinv))
            {
                cls.HasInventory = Utility.ToBool(hasinv);
            }

            if (row.TryGetValue("gemsockets", out var gemsockets))
            {
                cls.GemSockets = Utility.ToNullableInt(gemsockets);
            }

            if (row.TryGetValue("gemapplytype", out var gemapplytype))
            {
                int? gemtype = Utility.ToNullableInt(gemapplytype);
                cls.GemApplyType = gemtype is not null ? (GemApplyType)((int)gemtype) : null;
            }

            if (row.TryGetValue("flippyfile", out var flippyfile))
            {
                cls.FlippyFile = flippyfile;
            }

            if (row.TryGetValue("invfile", out var invfile))
            {
                cls.InventoryFile = invfile;
            }

            if (row.TryGetValue("uniqueinvfile", out var uniqueinvfile))
            {
                cls.UniqueInventoryFile = uniqueinvfile;
            }

            if (row.TryGetValue("setinvfile", out var setinvfile))
            {
                cls.SetInventoryFile = setinvfile;
            }

            if (row.TryGetValue("useable", out var useable))
            {
                cls.Useable = Utility.ToBool(useable);
            }

            if (row.TryGetValue("stackable", out var stackable))
            {
                cls.Stackable = Utility.ToBool(stackable);
            }

            if (row.TryGetValue("minstack", out var minstack))
            {
                cls.MinStack = Utility.ToNullableInt(minstack);
            }

            if (row.TryGetValue("maxstack", out var maxstack))
            {
                cls.MaxStack = Utility.ToNullableInt(maxstack);
            }

            if (row.TryGetValue("spawnstack", out var spawnstack))
            {
                cls.SpawnStack = Utility.ToNullableInt(spawnstack);
            }

            if (row.TryGetValue("Transmogrify", out var transmogrify))
            {
                cls.Transmogrify = Utility.ToBool(transmogrify);
            }

            if (row.TryGetValue("TMogMax", out var tMogMax))
            {
                cls.TransmogrifyMax = Utility.ToNullableInt(tMogMax);
            }

            if (row.TryGetValue("TMogMin", out var tMogMin))
            {
                cls.TransmogrifyMin = Utility.ToNullableInt(tMogMin);
            }

            if (row.TryGetValue("TMogType", out var tMogType))
            {
                cls.TransmogrifyType = Utility.ToNullableInt(tMogType);
            }

            if (row.TryGetValue("dropsound", out var dropsound))
            {
                cls.DropSound = dropsound;
            }

            if (row.TryGetValue("dropsfxframe", out var dropsfxframe))
            {
                cls.DropSfxFrame = Utility.ToNullableInt(dropsfxframe);
            }

            if (row.TryGetValue("usesound", out var usesound))
            {
                cls.UseSound = usesound;
            }

            if (row.TryGetValue("unique", out var unique))
            {
                cls.Unique = Utility.ToBool(unique);
            }

            if (row.TryGetValue("transparent", out var transparent))
            {
                cls.Transparent = Utility.ToBool(transparent);
            }

            if (row.TryGetValue("transtbl", out var transtbl))
            {
                cls.TransparencyTable = Utility.ToNullableInt(transtbl);
            }

            //if (row.TryGetValue("quivered", out var quivered))
            //{
            //    cls.Quivered = Utility.ToBool(quivered);
            //}

            if (row.TryGetValue("lightradius", out var lightradius))
            {
                cls.LightRadius = Utility.ToNullableInt(lightradius);
            }

            if (row.TryGetValue("belt", out var belt))
            {
                cls.Belt = Utility.ToNullableInt(belt);
            }

            if (row.TryGetValue("quest", out var quest))
            {
                cls.Quest = Utility.ToNullableInt(quest);
            }

            if (row.TryGetValue("questdiffcheck", out var questdiffcheck))
            {
                cls.QuestDiffCheck = Utility.ToBool(questdiffcheck);
            }

            if (row.TryGetValue("missiletype", out var missiletype))
            {
                cls.MissileType = Utility.ToNullableInt(missiletype);
            }

            if (row.TryGetValue("durwarning", out var durwarning))
            {
                cls.DurabilityWarning = Utility.ToNullableInt(durwarning);
            }

            if (row.TryGetValue("qntwarning", out var qntwarning))
            {
                cls.QuantityWarning = Utility.ToNullableInt(qntwarning);
            }

            if (row.TryGetValue("mindam", out var mindam))
            {
                cls.MinDamage = Utility.ToNullableInt(mindam);
            }

            if (row.TryGetValue("maxdam", out var maxdam))
            {
                cls.MaxDamage = Utility.ToNullableInt(maxdam);
            }

            if (row.TryGetValue("StrBonus", out var strBonus))
            {
                cls.StrengthBonus = Utility.ToNullableInt(strBonus);
            }

            if (row.TryGetValue("DexBonus", out var dexBonus))
            {
                cls.DexterityBonus = Utility.ToNullableInt(dexBonus);
            }

            if (row.TryGetValue("gemoffset", out var gemoffset))
            {
                cls.GemOffset = Utility.ToNullableInt(gemoffset);
            }

            if (row.TryGetValue("bitfield1", out var bitfield1))
            {
                cls.BitField1 = Utility.ToNullableInt(bitfield1);
            }

            if (row.TryGetValue("Transform", out var transform))
            {
                cls.Transform = Utility.ToNullableInt(transform);
            }

            if (row.TryGetValue("InvTrans", out var invTrans))
            {
                cls.InventoryTransform = Utility.ToNullableInt(invTrans);
            }

            if (row.TryGetValue("SkipName", out var skipName))
            {
                cls.SkipName = Utility.ToNullableInt(skipName);
            }

            if (row.TryGetValue("NightmareUpgrade", out var nightmareUpgrade))
            {
                cls.NightmareUpgrade = nightmareUpgrade;
            }

            if (row.TryGetValue("HellUpgrade", out var hellUpgrade))
            {
                cls.HellUpgrade = hellUpgrade;
            }

            if (row.TryGetValue("Nameable", out var nameable))
            {
                cls.Nameable = Utility.ToBool(nameable);
            }

            if (row.TryGetValue("PermStoreItem", out var permStoreItem))
            {
                cls.PermStoreItem = Utility.ToBool(permStoreItem);
            }

            if (row.TryGetValue("diablocloneweight", out var diablocloneweight))
            {
                cls.DiabloCloneWeight = Utility.ToNullableInt(diablocloneweight);
            }

            cls.Vendors = ImportVendors(cls, row);
        }

        static NPCVendors ImportVendors(Equipment instance, Dictionary<string, string> row)
        {
            NPCVendors Vendors = new();
            foreach (var pvp in Vendors.GetType().GetProperties())
            {
                NPCVendor npcVendor = new();
                foreach (var prop in npcVendor.GetType().GetProperties())
                {
                    if (row.TryGetValue($"{pvp.Name}{prop.Name}", out var value))
                    {
                        var vendorInfo = npcVendor.GetType().GetProperty(prop.Name);
                        vendorInfo.SetValue(npcVendor, Utility.ToNullableInt(value));
                    }
                }
                var propertyInfo = Vendors.GetType().GetProperty(pvp.Name);
                propertyInfo.SetValue(Vendors, npcVendor);
            }
            return Vendors;
        }

        static string ImportRequiredClass(IImporter importer, Dictionary<string, string> row)
        {
            if (importer.ItemTypes.TryGetValue(row["type"], out ItemType type) == false)
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(type.Equiv2) &&
                importer.ItemTypes.TryGetValue(type.Equiv2, out ItemType res))
            {
                return res.Name.Replace(" Item", "");
            }
            return string.Empty;
    }

    public override string ToString()
        {
            return Name;
        }
    }

    public class NPCVendors : ICloneable
    {
        public NPCVendor? Charsi { get; set; }
        public NPCVendor? Gheed { get; set; }
        public NPCVendor? Akara { get; set; }
        public NPCVendor? Fara { get; set; }
        public NPCVendor? Lysander { get; set; }
        public NPCVendor? Drognan { get; set; }
        public NPCVendor? Hratli { get; set; }
        public NPCVendor? Alkor { get; set; }
        public NPCVendor? Ormus { get; set; }
        public NPCVendor? Elzix { get; set; }
        public NPCVendor? Asheara { get; set; }
        public NPCVendor? Cain { get; set; }
        public NPCVendor? Halbu { get; set; }
        public NPCVendor? Jamella { get; set; }
        public NPCVendor? Larzuk { get; set; }
        public NPCVendor? Malah { get; set; }
        public NPCVendor? Anya { get; set; }

        public object Clone()
        {
            return new NPCVendors()
            {
                Charsi = this.Charsi,
                Gheed = this.Gheed,
                Akara = this.Akara,
                Fara = this.Fara,
                Lysander = this.Lysander,
                Drognan = this.Drognan,
                Hratli = this.Hratli,
                Alkor = this.Alkor,
                Ormus = this.Ormus,
                Elzix = this.Elzix,
                Asheara = this.Asheara,
                Cain = this.Cain,
                Halbu = this.Halbu,
                Jamella = this.Jamella,
                Larzuk = this.Larzuk,
                Malah = this.Malah,
                Anya = this.Anya,
            };
        }
    }

    public class NPCVendor : ICloneable
    {
        /// <summary>
        /// Minimum amount of this item type in Normal rarity that the NPC can sell at once
        /// </summary>
        [JsonIgnore]
        public int? Min { get; set; } = 0;

        /// <summary>
        /// <para>Maximum amount of this item type in Normal rarity that the NPC can sell at once.</para>
        /// <para>This must be equal to or greater than the minimum amount.</para>
        /// </summary>
        [JsonIgnore]
        public int? Max { get; set; } = 0;

        /// <summary>
        /// Minimum amount of this item type in Magical rarity that the NPC can sell at once
        /// </summary>
        [JsonIgnore]
        public int? MagicMin { get; set; } = 0;

        /// <summary>
        /// <para>Maximum amount of this item type in Magical rarity that the NPC can sell at once.</para>
        /// <para>This must be equal to or greater than the minimum amount.</para>
        /// </summary>
        [JsonIgnore]
        public int? MagicMax { get; set; } = 0;

        /// <summary>
        /// Maximum magic level allowed for this item type in Magical rarity
        /// </summary>
        [JsonIgnore]
        public int? MagicLvl { get; set; } = 0;


        public object Clone()
        {
            return new NPCVendor()
            {
                Min = this.Min,
                Max = this.Max,
                MagicMin = this.MagicMin,
                MagicMax = this.MagicMax,
                MagicLvl = this.MagicLvl,
            };
        }
    }
}
