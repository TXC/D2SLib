using D2Shared.IO;
using D2SImporter.Model;
using D2Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using D2SImporter;

namespace D2SLib.Model.Save;

public sealed class ItemList : IDisposable
{
    private ItemList()
    {
        Header = null;
        Count = null;
        Items = [];
    }

    private ItemList(ushort header, ushort count)
    {
        Header = header;
        Count = count;
        Items = new List<Item>(count);
    }

    public ushort? Header { get; set; }
    public ushort? Count { get; set; }
    public List<Item> Items { get; }

    public void Write(IBitWriter writer, uint version)
        => Write(writer, (SaveVersion)version);

    public void Write(IBitWriter writer, SaveVersion version)
    {
        Count ??= 0;
        writer.WriteUInt16(Header ?? 0x4D4A);
        writer.WriteUInt16(Count.Value);
        for (int i = 0; i < Count; i++)
        {
            Items[i].Write(writer, version);
        }
    }

    public static ItemList Read(IBitReader reader, SaveVersion version)
    {
        var itemList = new ItemList
        {
            Header = reader.ReadUInt16(),
            Count = reader.ReadUInt16()
        };
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList.Items.Add(Item.Read(reader, version));
        }
        return itemList;
    }

    [Obsolete("Try the non-allocating overload!")]
    public static byte[] Write(ItemList itemList, uint version)
    {
        using var writer = new BitWriter();
        itemList.Write(writer, version);
        return writer.ToArray();
    }

    public void Dispose()
    {
        foreach (var item in Items)
        {
            item?.Dispose();
        }
        Items.Clear();
    }
}

public sealed class Item : IDisposable
{
    private InternalBitArray _flags = new(4);

    #region Properties
    public ushort? Header { get; set; }

    /// <summary>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Bit - Size</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>0 - 16</term>
    ///         <description>"JM" (separate from the list header)</description>
    ///     </item>
    ///     <item>
    ///         <term>16 - 4</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>20 - 1</term>
    ///         <description>Identified</description>
    ///     </item>
    ///     <item>
    ///         <term>21 - 6</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>27 - 1</term>
    ///         <description>Socketed</description>
    ///     </item>
    ///     <item>
    ///         <term>28 - 1</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>29 - 1</term>
    ///         <description>Picked up since last save</description>
    ///     </item>
    ///     <item>
    ///         <term>30 - 2</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>32 - 1</term>
    ///         <description>Ear</description>
    ///     </item>
    ///     <item>
    ///         <term>33 - 1</term>
    ///         <description>Starter Gear</description>
    ///     </item>
    ///     <item>
    ///         <term>34 - 3</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>37 - 1</term>
    ///         <description>Compact</description>
    ///     </item>
    ///     <item>
    ///         <term>38 - 1</term>
    ///         <description>Ethereal</description>
    ///     </item>
    ///     <item>
    ///         <term>39 - 1</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>40 - 1</term>
    ///         <description>Personalized</description>
    ///     </item>
    ///     <item>
    ///         <term>41 - 1</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>42 - 1</term>
    ///         <description>Runeword</description>
    ///     </item>
    ///     <item>
    ///         <term>43 - 15</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>58 - 3</term>
    ///         <description>Item Location (0 - Stored, 1 = Equipped, 2 = Belt, 4 = Moved, 6 = Socket</description>
    ///     </item>
    ///     <item>
    ///         <term>61 - 4</term>
    ///         <description>Equipped Location (<see cref="EquippedSlot"/>)</description>
    ///     </item>
    ///     <item>
    ///         <term>65 - 4</term>
    ///         <description>
    ///         <para>Column number of the left corner of the item, counting from 0.
    ///         Your inventory has ten columns (numbered 0-9), your stash has six,
    ///         and the Horadric Cube has four.</para>
    ///         <para>Your belt is considered (for the purposes of the item format)
    ///         to have no rows, but either 4, 8, 12, or 16 columns.
    ///         If you prefer, you can divide this field and use the 2 bits at
    ///         position 67-68 for the row (but only for belts).</para>
    ///         <para>If the item is equipped, glued to a socket, or in transit,
    ///         then this field appears to contain old data from the last time
    ///         the item was stored.
    ///         I.e., it may be non-zero, but the value is unused.</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>69 - 3</term>
    ///         <description>Row number of the top of the item, counting from 0.
    ///         Your inventory has four rows (numbered 0-3), your stash has four in
    ///         normal characters or eight in Expansion Set characters, and the
    ///         Horadric Cube has four.
    ///         (In the belt, this field is always zero.)</description>
    ///     </item>
    ///     <item>
    ///         <term>72 - 1</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>73 - 3</term>
    ///         <description><para>Actually, bit 74 seems to always be 0, but since
    ///         bits 73 and 75 are related I just lump them all together.
    ///         If the item is neither equipped nor in your belt, this field
    ///         tells where it is.</para>
    ///         <para>Possible values are:
    ///         0 - Not Here,
    ///         1 - Inventory
    ///         4 - Horadric Cube
    ///         5 - Stash
    ///         </para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>76 - 4</term>
    ///         <description>?</description>
    ///     </item>
    ///     <item>
    ///         <term>80 - 24</term>
    ///         <description>Type code (3 letters)</description>
    ///     </item>
    ///     <item>
    ///         <term>108 - </term>
    ///         <description>Extended Item Data</description>
    ///     </item>
    /// </list>
    /// </summary>
    [JsonIgnore]
    public IList<bool> Flags
    {
        get => [.. _flags];
        set
        {
            if (value is InternalBitArray flags && flags != null)
            {
                _flags?.Dispose();
                _flags = flags;
            }
            else
            {
                throw new ArgumentException("Flags were not of expected type.");
            }
        }
    }

    public string? Version { get; set; }

    /// <summary>
    /// Parent Location
    /// <para>Offset: Bit 58</para>
    /// <para>Size: 3 bits</para>
    /// </summary>
    public ParentLocation ParentLocation { get; set; }

    /// <summary>
    /// Equipped at
    /// <para>Offset: Bit 61</para>
    /// <para>Size: 4 bits</para>
    /// </summary>
    public EquippedSlot SlotLocation { get; set; }

    /// <summary>
    /// Column
    /// <para>Offset: Bit 65</para>
    /// <para>Size: 4 bits</para>
    /// <para>Column number of the left corner of the item, counting from 0.</para>
    /// <para>Your inventory has ten columns (numbered 0-9), your stash has six,
    /// and the Horadric Cube has four.</para>
    /// <para><b>Note: </b>Your belt is considered (for the purposes of the
    /// item format) to have no rows, but either 4, 8, 12, or 16 columns.
    /// If you prefer, you can divide this field and use the last 2 bits for
    /// the row (but only for belts).</para>
    /// <para><b>Note: </b>If the item is equipped, glued to a socket, or in
    /// transit, then this field appears to contain old data from the last
    /// time the item was stored.
    /// I.e., it may be non-zero, but the value is unused.</para>
    /// </summary>
    public byte X { get; set; }

    /// <summary>
    /// Row
    /// <para>Offset: Bit 69</para>
    /// <para>Size: 3 bits</para>
    /// </summary>
    public byte Y { get; set; }
    public StorageLocation StoredLocation { get; set; }
    public CharacterClass EarClass { get; set; }
    public byte EarLevel { get; set; }
    public string PlayerName { get; set; } = string.Empty; //used for personalized or ears
    public string Code { get; set; } = string.Empty;
    public byte NumberOfSocketedItems { get; set; }
    public byte TotalNumberOfSockets { get; set; }
    public List<Item> SocketedItems { get; set; } = new();
    public uint Id { get; set; }
    public byte ItemLevel { get; set; }
    public ItemQuality Quality { get; set; }
    public bool HasMultipleGraphics { get; set; }
    public byte GraphicId { get; set; }
    public bool IsClassSpecific { get; set; }
    /// <summary>
    /// Is only set if <see cref="IsClassSpecific"/> is true
    /// </summary>
    public ushort AutoAffixId { get; set; } //?
    public uint FileIndex { get; set; }
    public ushort[] MagicPrefixIds { get; set; } = new ushort[3];
    public ushort[] MagicSuffixIds { get; set; } = new ushort[3];
    public ushort RarePrefixId { get; set; }
    public ushort RareSuffixId { get; set; }
    public uint RunewordId { get; set; }
    [JsonIgnore]
    public bool HasRealmData { get; set; }
    [JsonIgnore]
    public uint[] RealmData { get; set; } = new uint[3];
    public ushort Armor { get; set; }
    public ushort MaxDurability { get; set; }
    public ushort Durability { get; set; }
    public ushort Quantity { get; set; }
    public ushort QuestDifficulty { get; set; }
    public byte SetItemMask { get; set; }
    public List<ItemStatList> MagicAttributes { get; } = [];
    public List<ItemStatList> SetAttributes { get; } = [];
    public List<ItemStatList> RunewordAttributes { get; } = [];


    /// <summary>
    /// <para>Bit Position: 20</para>
    /// Item has been identified
    /// </summary>
    public bool IsIdentified { get => _flags[4]; set => _flags[4] = value; }

    /// <summary>
    /// <para>Bit Position: 27</para>
    /// Item is Socketed
    /// </summary>
    public bool IsSocketed { get => _flags[11]; set => _flags[11] = value; }

    /// <summary>
    /// <para>Bit Position: 29</para>
    /// This bit is set on items which you have picked up since the last time the game was saved.
    /// </summary>
    public bool IsNew { get => _flags[13]; set => _flags[13] = value; }

    /// <summary>
    /// <para>Bit Position: 32</para>
    /// Player Ear
    /// Item contains information about the Ear's former owner (Class, Level, Name)
    /// </summary>
    public bool IsEar { get => _flags[16]; set => _flags[16] = value; }

    /// <summary>
    /// <para>Bit Position: 33</para>
    /// "Newbie" item.
    /// This bit is set on the weapon and shield your character is given when you start the game.
    /// Apparently, this gives the item the property of having a repair cost of 1gp, as well as a sell value of 1gp.
    /// </summary>
    public bool IsStarterItem { get => _flags[17]; set => _flags[17] = value; }

    /// <summary>
    /// <para>Bit Position: 37</para>
    /// Item is simple (only 111 bits {14 bytes} of item data)
    /// </summary>
    public bool IsCompact { get => _flags[21]; set => _flags[21] = value; }

    /// <summary>
    /// <para>Bit Position: 38</para>
    /// Item is Ethereal (Cannot be Repaired)
    /// </summary>
    public bool IsEthereal { get => _flags[22]; set => _flags[22] = value; }

    /// <summary>
    /// <para>Bit Position: 40</para>
    /// Item has been personalized (by Anya in Act V)
    /// </summary>
    public bool IsPersonalized { get => _flags[24]; set => _flags[24] = value; }

    /// <summary>
    /// <para>Bit Position: 42</para>
    /// </summary>
    public bool IsRuneword { get => _flags[26]; set => _flags[26] = value; }
    #endregion Properties

    public void Write(IBitWriter writer, uint version)
        => Write(writer, (SaveVersion)version);

    public void Write(IBitWriter writer, SaveVersion version)
    {
        if (version <= SaveVersion.v11x) //0x60
        {
            writer.WriteUInt16(Header ?? 0x4D4A);
        }
        WriteCompact(writer, this, version);
        if (!IsCompact)
        {
            WriteComplete(writer, this, version);
        }
        writer.Align();
        for (int i = 0; i < NumberOfSocketedItems; i++)
        {
            SocketedItems[i].Write(writer, version);
        }
    }

    public static Item Read(ReadOnlySpan<byte> bytes, uint version)
        => Read(bytes, (SaveVersion)version);

    public static Item Read(ReadOnlySpan<byte> bytes, SaveVersion version)
    {
        using var reader = new BitReader(bytes);
        return Read(reader, version);
    }

    public static Item Read(IBitReader reader, uint version)
        => Read(reader, (SaveVersion)version);

    public static Item Read(IBitReader reader, SaveVersion version)
    {
        var item = new Item();
        if (version <= SaveVersion.v11x) // 0x60
        {
            item.Header = reader.ReadUInt16();
        }
        ReadCompact(reader, item, version);
        if (!item.IsCompact) 
        {
            ReadComplete(reader, item, version);
        }
        reader.Align();
        for (int i = 0; i < item.NumberOfSocketedItems; i++)
        {
            item.SocketedItems.Add(Read(reader, version));
        }
        return item;
    }

    public static byte[] Write(Item writer, uint version)
        => Write(writer, (SaveVersion)version);

    public static byte[] Write(Item item, SaveVersion version)
    {
        using var writer = new BitWriter();
        item.Write(writer, version);
        return writer.ToArray();
    }

    private static string ReadPlayerName(IBitReader reader, SaveVersion version)
    {
        Span<char> name = stackalloc char[16];
        int bitsToRead = 7;
        if (version > SaveVersion.v200)
        {
            bitsToRead = 8;
        }
        for (int i = 0; i < name.Length; i++)
        {
            name[i] = (char)reader.ReadByte(bitsToRead);
            if (name[i] == '\0')
            {
                break;
            }
        }
        var result = new string(name);
        return result.TrimEnd('\0');
    }

    private static void WritePlayerName(IBitWriter writer, string name, SaveVersion version)
    {
        int bitsToWrite = 7;
        if (version > SaveVersion.v200)
        {
            bitsToWrite = 8;
        }
        var nameChars = name.AsSpan().TrimEnd('\0');
        Span<byte> bytes = stackalloc byte[nameChars.Length];
        int byteCount = Encoding.ASCII.GetBytes(nameChars, bytes);
        bytes = bytes[..byteCount];
        for (int i = 0; i < bytes.Length; i++)
        {
            writer.WriteByte(bytes[i], bitsToWrite);
        }
        writer.WriteByte((byte)'\0', bitsToWrite);
    }

    private static void ReadCompact(IBitReader reader, Item item, SaveVersion version)
    {
        Span<byte> bytes = stackalloc byte[4];
        reader.ReadBytes(bytes);
        //1.10-1.14d
        //[flags:32][version:10][mode:3]([invloc:4][x:4][y:4][page:3])([itemcode:32])([sockets:3])
        //1.15
        //[flags:32][version:3][mode:3]([invloc:4][x:4][y:4][page:3])([itemcode:variable])([sockets:3])
        item.Flags = new InternalBitArray(bytes);
        if (version <= SaveVersion.v11x) // 0x60
        {
            item.Version = Convert.ToString(reader.ReadUInt16(10) ?? 0, 10);
        }
        else if (version >= SaveVersion.v200) //0x61
        {
            item.Version = Convert.ToString(reader.ReadUInt16(3) ?? 0, 2);
        }
        item.ParentLocation = (ParentLocation)(reader.ReadByte(3) ?? 0);
        item.SlotLocation = (EquippedSlot)(reader.ReadByte(4) ?? 0);
        item.X = reader.ReadByte(4) ?? 0;
        item.Y = reader.ReadByte(4) ?? 0;
        item.StoredLocation = (StorageLocation)(reader.ReadByte(3) ?? 0);
        if (item.IsEar)
        {
            item.EarClass = (CharacterClass)(reader.ReadByte(3) ?? 0xff);
            item.EarLevel = reader.ReadByte(7) ?? 0;
            item.PlayerName = ReadPlayerName(reader, SaveVersion.Any);
        }
        else
        {
            item.Code = string.Empty;
            if (version <= SaveVersion.v11x) //0x60
            {
                item.Code = reader.ReadString(4);
                item.Code = item.Code.Trim('\0');
            }
            else if (version >= SaveVersion.v200) // 0x61
            {
                for (int i = 0; i < 4; i++)
                {
                    item.Code += IImporter.ItemCodeTree.DecodeChar(reader);
                }
                item.Code = item.Code.Trim('\0');
            }

            int bits = item.IsCompact ? 1 : 3;
            var equipment = Core.Importer.GetByCode(item.Code);
            if (equipment is not null
                && equipment.Type.Categories.Any(x => x == "ques"))
            {
                ItemStatCost qd = Core.Importer.ItemStatCosts["questitemdifficulty"];
                var saveBits = reader.ReadUInt16(qd.SaveBits ?? 0);
                item.QuestDifficulty = (ushort)((saveBits ?? 0) - (qd.SaveAdd ?? 0));
                bits = 1;
            }

            item.NumberOfSocketedItems = reader.ReadByte(bits) ?? 0;
        }
    }

    private static void WriteCompact(IBitWriter writer, Item item, SaveVersion version)
    {
        /*
        if (item.Flags is not InternalBitArray flags)
        {
            flags = new InternalBitArray(32)
            {
                [04] = item.IsIdentified,
                [11] = item.IsSocketed,
                [13] = item.IsNew,
                [16] = item.IsEar,
                [17] = item.IsStarterItem,
                [21] = item.IsCompact,
                [22] = item.IsEthereal,
                [24] = item.IsPersonalized,
                [26] = item.IsRuneword
            };
        }
        writer.WriteBits(flags);
        */
        writer.WriteBits(item._flags);
        if (version <= SaveVersion.v11x) //0x60
        {
            //todo. how do we handle 1.15 version to 1.14. maybe this should be a string
            writer.WriteUInt16(Convert.ToUInt16(item.Version, 10), 10);
        }
        else if (version >= SaveVersion.v200) //0x61
        {
            writer.WriteUInt16(Convert.ToUInt16(item.Version, 2), 3);
        }
        writer.WriteByte((byte)item.ParentLocation, 3);
        writer.WriteByte((byte)item.SlotLocation, 4);
        writer.WriteByte(item.X, 4);
        writer.WriteByte(item.Y, 4);
        writer.WriteByte((byte)item.StoredLocation, 3);
        if (item.IsEar)
        {
            writer.WriteUInt32(item.FileIndex, 3);
            writer.WriteByte(item.EarLevel, 7);
            WritePlayerName(writer, item.PlayerName, SaveVersion.Any);
        }
        else
        {
            var itemCode = item.Code.PadRight(4, ' ');
            Span<byte> code = stackalloc byte[itemCode.Length];
            Encoding.ASCII.GetBytes(itemCode, code);
            if (version <= SaveVersion.v11x) // 0x60
            {
                writer.WriteBytes(code);
            }
            else if (version >= SaveVersion.v200) // 0x61
            {
                var codeTree = IImporter.ItemCodeTree;
                for (int i = 0; i < 4; i++)
                {
                    using var bits = codeTree.EncodeChar((char)code[i]);
                    foreach (bool bit in bits)
                    {
                        writer.WriteBit(bit);
                    }
                }
            }
            writer.WriteByte(item.NumberOfSocketedItems, item.IsCompact ? 1 : 3);
        }
    }

    private static void ReadComplete(IBitReader reader, Item item, SaveVersion version)
    {
        item.Id = reader.ReadUInt32() ?? 0;
        item.ItemLevel = reader.ReadByte(7) ?? 0;
        item.Quality = (ItemQuality)(reader.ReadByte(4) ?? 0);
        item.HasMultipleGraphics = reader.ReadBit();
        if (item.HasMultipleGraphics)
        {
            item.GraphicId = reader.ReadByte(3) ?? 0;
        }
        item.IsClassSpecific = reader.ReadBit();
        if (item.IsClassSpecific)
        {
            item.AutoAffixId = reader.ReadUInt16(11) ?? 0;
        }
        switch (item.Quality)
        {
            case ItemQuality.Normal:
                break;
            case ItemQuality.Inferior:
            case ItemQuality.Superior:
                item.FileIndex = reader.ReadUInt16(3) ?? 0;
                break;
            case ItemQuality.Magic:
                item.MagicPrefixIds[0] = reader.ReadUInt16(11) ?? 0;
                item.MagicSuffixIds[0] = reader.ReadUInt16(11) ?? 0;
                break;
            case ItemQuality.Rare:
            case ItemQuality.Craft:
                item.RarePrefixId = reader.ReadUInt16(8) ?? 0;
                item.RareSuffixId = reader.ReadUInt16(8) ?? 0;
                for (int i = 0; i < 3; i++)
                {
                    if (reader.ReadBit())
                    {
                        item.MagicPrefixIds[i] = reader.ReadUInt16(11) ?? 0;
                    }
                    if (reader.ReadBit())
                    {
                        item.MagicSuffixIds[i] = reader.ReadUInt16(11) ?? 0;
                    }
                }
                break;
            case ItemQuality.Set:
            case ItemQuality.Unique:
                item.FileIndex = reader.ReadUInt16(12) ?? 0;
                break;
        }

        ushort propertyLists = 0;
        if (item.IsRuneword)
        {
            item.RunewordId = reader.ReadUInt32(12) ?? 0;
            if (item.RunewordId == 0x0a9e)
            {
                item.RunewordId = 0x30;
            }
            //propertyLists |= (ushort)(1 << (reader.ReadUInt16(4) ?? 0 + 1));
            reader.AdvanceBits(4);
        }
        if (item.IsPersonalized)
        {
            item.PlayerName = ReadPlayerName(reader, version);
        }
        var trimmedCode = item.Code.AsSpan().TrimEnd();
        if (trimmedCode.SequenceEqual("tbk") || trimmedCode.SequenceEqual("ibk"))
        {
            item.MagicSuffixIds[0] = reader.ReadByte(5) ?? 0;
        }
        item.HasRealmData = reader.ReadBit();
        if (item.HasRealmData)
        {
            //reader.ReadBits(96);
            reader.AdvanceBits(96);
        }

        var row = Core.Importer.GetByCode(item.Code);
        if (row is null)
        {
            return;
        }
        if (row.EquipmentType == EquipmentType.Armor)
        {
            item.Armor = (ushort)(reader.ReadUInt16(11) + ((ushort?)Core.Importer.ItemStatCosts["armorclass"].SaveAdd) ?? 0);
        }
        if (row.EquipmentType == EquipmentType.Armor || row.EquipmentType == EquipmentType.Weapon)
        {
            var maxDurabilityStat = Core.Importer.ItemStatCosts["maxdurability"];
            var durabilityStat = Core.Importer.ItemStatCosts["maxdurability"];
            item.MaxDurability = (ushort)(reader.ReadUInt16(maxDurabilityStat?.SaveBits ?? 0) + maxDurabilityStat?.SaveAdd ?? 0);
            if (item.MaxDurability > 0)
            {
                item.Durability = (ushort)(reader.ReadUInt16(durabilityStat?.SaveBits ?? 0) + durabilityStat?.SaveAdd ?? 0);
                //what is this?
                reader.ReadBit();
            }
        }
        if (row.Stackable)
        {
            item.Quantity = reader.ReadUInt16(9) ?? 0;
        }
        if (item.IsSocketed)
        {
            item.TotalNumberOfSockets = reader.ReadByte(4) ?? 0;
        }
        item.SetItemMask = 0;
        if (item.Quality == ItemQuality.Set)
        {
            item.SetItemMask = reader.ReadByte(5) ?? 0;
            propertyLists |= item.SetItemMask;
        }

        item.MagicAttributes.Add(ItemStatList.Read(reader));

        while (propertyLists > 0)
        {
            if ((propertyLists & 1) > 0)
            {
                item.SetAttributes.Add(ItemStatList.Read(reader));
            }
            propertyLists >>= 1;
        }
        /*
        for (int i = 1; i <= 64; i <<= 1)
        {
            if ((propertyLists & i) != 0)
            {
                item.SetAttributes.Add(ItemStatList.Read(reader));
            }
        }
        */
        if (item.IsRuneword)
        {
            item.RunewordAttributes.Add(ItemStatList.Read(reader));
        }
    }

    private static void WriteComplete(IBitWriter writer, Item item, SaveVersion version)
    {
        writer.WriteUInt32(item.Id);
        writer.WriteByte(item.ItemLevel, 7);
        writer.WriteByte((byte)item.Quality, 4);
        writer.WriteBit(item.HasMultipleGraphics);
        if (item.HasMultipleGraphics)
        {
            writer.WriteByte(item.GraphicId, 3);
        }
        writer.WriteBit(item.IsClassSpecific);
        if (item.IsClassSpecific)
        {
            writer.WriteUInt16(item.AutoAffixId, 11);
        }
        switch (item.Quality)
        {
            case ItemQuality.Normal:
                break;
            case ItemQuality.Inferior:
            case ItemQuality.Superior:
                writer.WriteUInt32(item.FileIndex, 3);
                break;
            case ItemQuality.Magic:
                writer.WriteUInt16(item.MagicPrefixIds[0], 11);
                writer.WriteUInt16(item.MagicSuffixIds[0], 11);
                break;
            case ItemQuality.Rare:
            case ItemQuality.Craft:
                writer.WriteUInt16(item.RarePrefixId, 8);
                writer.WriteUInt16(item.RareSuffixId, 8);
                for (int i = 0; i < 3; i++)
                {
                    bool hasPrefix = item.MagicPrefixIds[i] > 0;
                    bool hasSuffix = item.MagicSuffixIds[i] > 0;
                    writer.WriteBit(hasPrefix);
                    if (hasPrefix)
                    {
                        writer.WriteUInt16(item.MagicPrefixIds[i], 11);
                    }
                    writer.WriteBit(hasSuffix);
                    if (hasSuffix)
                    {
                        writer.WriteUInt16(item.MagicSuffixIds[i], 11);
                    }
                }
                break;
            case ItemQuality.Set:
            case ItemQuality.Unique:
                writer.WriteUInt32(item.FileIndex, 12);
                break;
        }
        ushort propertyLists = 0;
        if (item.IsRuneword)
        {
            writer.WriteUInt32(item.RunewordId, 12);
            propertyLists |= 1 << 6;
            writer.WriteUInt16(5, 4);
        }
        if (item.IsPersonalized)
        {
            WritePlayerName(writer, item.PlayerName, version);
        }
        var trimmedCode = item.Code.AsSpan().Trim();
        if (trimmedCode.SequenceEqual("tbk") || trimmedCode.SequenceEqual("ibk"))
        {
            writer.WriteUInt16(item.MagicSuffixIds[0], 5);
        }
        writer.WriteBit(item.HasRealmData);
        if (item.HasRealmData)
        {
            //todo 96 bits
        }
        var row = Core.Importer.GetByCode(item.Code);
        if (row is null)
        {
            return;
        }
        if (row.EquipmentType == EquipmentType.Armor)
        {
            //writer.WriteUInt16((ushort)(item.Armor - itemStatCost.GetByStat("armorclass")?["Save Add"].ToUInt16() ?? 0), 11);
            writer.WriteUInt16((ushort)(item.Armor - Core.Importer.ItemStatCosts["armorclass"]?.SaveAdd ?? 0), 11);
        }
        if (row.EquipmentType == EquipmentType.Armor || row.EquipmentType == EquipmentType.Weapon)
        {
            var maxDurabilityStat = Core.Importer.ItemStatCosts["maxdurability"];
            var durabilityStat = Core.Importer.ItemStatCosts["maxdurability"];
            writer.WriteUInt16((ushort)(item.MaxDurability - maxDurabilityStat?.SaveAdd ?? 0), maxDurabilityStat?.SaveBits ?? 0);
            if (item.MaxDurability > 0)
            {
                writer.WriteUInt16((ushort)(item.Durability - durabilityStat?.SaveAdd ?? 0), durabilityStat?.SaveBits ?? 0);
                ////what is this?
                writer.WriteBit(false);
            }
        }
        if (row.Stackable)
        {
            writer.WriteUInt16(item.Quantity, 9);
        }
        if (item.IsSocketed)
        {
            writer.WriteByte(item.TotalNumberOfSockets, 4);
        }
        if (item.Quality == ItemQuality.Set)
        {
            writer.WriteByte(item.SetItemMask, 5);
            propertyLists |= item.SetItemMask;
        }
        ItemStatList.Write(writer, item.MagicAttributes[0]);

        if (item.SetAttributes.Count > 0)
        {
            foreach (var setItem in item.SetAttributes)
            {
                ItemStatList.Write(writer, setItem);
            }
        }

        if (item.IsRuneword && item.RunewordAttributes.Count > 0)
        {
            foreach (var runeItem in item.RunewordAttributes)
            {
                ItemStatList.Write(writer, runeItem);
            }
        }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _flags!, null)?.Dispose();
        foreach (var item in SocketedItems)
        {
            item?.Dispose();
        }
        SocketedItems.Clear();
    }
}

/// <summary>
/// Magic Properties
/// </summary>
public class ItemStatList
{
    private const ushort item_maxdamage_percent = 17;
    private const ushort firemindam = 48;
    private const ushort lightmindam = 50;
    private const ushort magicmindam = 52;
    private const ushort coldmindam = 54;
    private const ushort poisonmindam = 57;

    public List<ItemStat> Stats { get; set; } = new();

    public static ItemStatList Read(IBitReader reader)
    {
        var itemStatList = new ItemStatList();
        ushort id = reader.ReadUInt16(9) ?? 0;
        while (id != 0x1ff)
        {
            var property = Core.Importer.ItemStatCosts.Values.FirstOrDefault(x => x.Id == id);
            if (property is null)
            {
                throw new Exception($"Cannot find Magical Property for id: {id} at position {reader.Position}");
            }

            for (var i = 0; i < property.NumberOfProperties; i++)
            {
                itemStatList.Stats.Add(ItemStat.Read(reader, (ushort)(id + i)));
            }
            /*
            // https://github.com/ThePhrozenKeep/D2MOO/blob/master/source/D2Common/src/Items/Items.cpp#L7332
            if (id is magicmindam or
                item_maxdamage_percent or
                firemindam or
                lightmindam)
            {
                itemStatList.Stats.Add(ItemStat.Read(reader, (ushort)(id + 1)));
            }
            else if (id is coldmindam or poisonmindam)
            {
                itemStatList.Stats.Add(ItemStat.Read(reader, (ushort)(id + 1)));
                itemStatList.Stats.Add(ItemStat.Read(reader, (ushort)(id + 2)));
            }
            */
            id = reader.ReadUInt16(9) ?? 0;
        }
        return itemStatList;
    }

    public static void Write(IBitWriter writer, ItemStatList itemStatList)
    {
        for (int i = 0; i < itemStatList.Stats.Count; i++)
        {
            var stat = itemStatList.Stats[i];
            var property = ItemStat.GetStatRow(stat);
            ushort id = (ushort)property.Id;
            writer.WriteUInt16(id, 9);
            ItemStat.Write(writer, stat);

            //assume these stats are in order...
            //https://github.com/ThePhrozenKeep/D2MOO/blob/master/source/D2Common/src/Items/Items.cpp#L7332
            if (id is magicmindam or item_maxdamage_percent or firemindam or lightmindam)
            {
                ItemStat.Write(writer, itemStatList.Stats[++i]);
            }
            else if (id is coldmindam or poisonmindam)
            {
                ItemStat.Write(writer, itemStatList.Stats[++i]);
                ItemStat.Write(writer, itemStatList.Stats[++i]);
            }
        }
        writer.WriteUInt16(0x1ff, 9);
    }

}

/// <summary>
/// Magic Property
/// </summary>
public class ItemStat
{
    public ushort? Id { get; set; }
    public string Stat { get; set; } = string.Empty;
    public int? SkillTab { get; set; }
    public int? SkillId { get; set; }
    public int? SkillLevel { get; set; }
    public int? MaxCharges { get; set; }
    public int? Param { get; set; }
    public int Value { get; set; }
    // 0x039D
    public static ItemStat Read(IBitReader reader, ushort id)
    {
        var itemStat = new ItemStat();
        if (id > Core.Importer.ItemStatCosts.Count)
        {
            throw new Exception($"Invalid ItemStatCost identifier: {id}");
        }
        var property = Core.Importer.ItemStatCosts.Values.FirstOrDefault(x => x.Id == id);

        //var property = Core.Importer.ItemStatCosts[id.ToString()];
        //var property = Core.MetaData.ItemStatCostData.GetById(id);
        if (property == null)
        {
            throw new Exception($"No ItemStatCost record found for id: {id} at bit {reader.Position - 9}");
        }
        itemStat.Id = id;
        itemStat.Stat = property.Stat;

        if (property.SaveParamBits != 0)
        {
            int saveParam = reader.ReadInt32(property.SaveParamBits) ?? 0;
            //todo is there a better way to identify skill tab stats.
            switch (property.DescriptionFunction)
            {
                case 14: //+[value] to [skilltab] Skill Levels ([class] Only) : stat id 188
                    itemStat.SkillTab = saveParam & 0x7;
                    itemStat.SkillLevel = (saveParam >> 3) & 0x1fff;
                    break;
                default:
                    break;
            }
            switch (property.Encode)
            {
                case 2: //chance to cast skill
                case 3: //skill charges
                    itemStat.SkillLevel = saveParam & 0x3f;
                    itemStat.SkillId = (saveParam >> 6) & 0x3ff;
                    break;
                case 1:
                case 4: //by times
                default:
                    itemStat.Param = saveParam;
                    break;
            }
        }
        int saveBits = reader.ReadInt32(property.SaveBits ?? 0) ?? 0;
        saveBits -= property.SaveAdd ?? 0;
        switch (property.Encode)
        {
            case 3: //skill charges
                itemStat.MaxCharges = (saveBits >> 8) & 0xff;
                itemStat.Value = saveBits & 0xff;
                break;
            default:
                itemStat.Value = saveBits;
                break;
        }
        return itemStat;
    }

    public static void Write(IBitWriter writer, ItemStat stat)
    {
        var property = GetStatRow(stat);
        if (property is null)
        {
            throw new ArgumentException($"No ItemStatCost record found for id: {stat.Id}", nameof(stat));
        }
        if (property.SaveParamBits != 0)
        {
            if (stat.Param != null)
            {
                writer.WriteInt32((int)stat.Param, property.SaveParamBits);
            }
            else
            {
                int saveParamBits = 0;
                switch (property.DescriptionFunction)
                {
                    case 14: //+[value] to [skilltab] Skill Levels ([class] Only) : stat id 188
                        saveParamBits |= (stat.SkillTab ?? 0 & 0x7);
                        saveParamBits |= ((stat.SkillLevel ?? 0 & 0x1fff) << 3);
                        break;
                    default:
                        break;
                }
                switch (property.Encode)
                {
                    case 2: //chance to cast skill
                    case 3: //skill charges
                        saveParamBits |= (stat.SkillLevel ?? 0 & 0x3f);
                        saveParamBits |= ((stat.SkillId ?? 0 & 0x3ff) << 6);
                        break;
                    case 4: //by times
                    case 1:
                    default:
                        break;
                }
                //always use param if it is there.
                if (stat.Param != null)
                {
                    saveParamBits = (int)stat.Param;
                }
                writer.WriteInt32(saveParamBits, property.SaveParamBits);
            }
        }
        int saveBits = stat.Value;
        saveBits += property.SaveAdd ?? 0;
        switch (property.Encode)
        {
            case 3: //skill charges
                saveBits &= 0xff;
                saveBits |= ((stat.MaxCharges ?? 0 & 0xff) << 8);
                break;
            default:
                break;
        }
        writer.WriteInt32(saveBits, property.SaveBits ?? 0);
    }

    public static ItemStatCost GetStatRow(ItemStat stat)
    {
        return stat.Id is ushort statId
            ? Core.Importer.ItemStatCosts.Values.FirstOrDefault(x => x.Id == statId)
            : Core.Importer.ItemStatCosts[stat.Stat];
    }
}
