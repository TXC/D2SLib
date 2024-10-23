using D2Shared.IO;
using D2Shared.Enums;
using CommunityToolkit.HighPerformance.Buffers;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Linq;

namespace D2SLib.Model.Save;

public sealed class D2S : IDisposable
{
    private bool disposedValue;

    /// <summary>
    /// 0x0000
    /// </summary>
    public Header Header { get; set; }

    /// <summary>
    /// 0x0010
    /// </summary>
    //public uint ActiveWeapon { get; set; }

    /// <summary>
    /// 0x0014 sizeof(16)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 0x0024
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// 0x0025
    /// </summary>
    [JsonIgnore]
    public byte Progression { get; set; }

    /// <summary>
    /// 0x0026 [unk = 0x0, 0x0]
    /// </summary>
    [JsonIgnore]
    //public byte[]? Unk0x0026 { get; set; }
    public ushort ActiveWeapon { get; set; }

    /// <summary>
    /// 0x0028
    /// </summary>
    public CharacterClass ClassId { get; set; }

    /// <summary>
    /// 0x0029 [unk = 0x10, 0x1E]
    /// </summary>
    [JsonIgnore]
    public byte[]? Unk0x0029 { get; set; }

    /// <summary>
    /// 0x002b
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    /// 0x002c
    /// </summary>
    public uint Created { get; set; }

    /// <summary>
    /// 0x0030
    /// </summary>
    public uint LastPlayed { get; set; }

    /// <summary>
    /// 0x0034 [unk = 0xff, 0xff, 0xff, 0xff]
    /// </summary>
    [JsonIgnore]
    public byte[]? Unk0x0034 { get; set; }

    /// <summary>
    /// 0x0038
    /// </summary>
    public Skill[] AssignedSkills { get; set; }

    /// <summary>
    /// 0x0078
    /// </summary>
    public Skill LeftSkill { get; set; }

    /// <summary>
    /// 0x007c
    /// </summary>
    public Skill RightSkill { get; set; }

    /// <summary>
    /// 0x0080
    /// </summary>
    public Skill LeftSwapSkill { get; set; }

    /// <summary>
    /// 0x0084
    /// </summary>
    public Skill RightSwapSkill { get; set; }

    /// <summary>
    /// 0x0088 [char menu appearance]
    /// </summary>
    public Appearances Appearances { get; set; }

    /// <summary>
    /// 0x00a8
    /// </summary>
    public Difficulties Difficulties { get; set; }

    /// <summary>
    /// 0x00ab
    /// </summary>
    public uint MapId { get; set; }

    /// <summary>
    /// 0x00af [unk = 0x0, 0x0]
    /// </summary>
    [JsonIgnore]
    public byte[]? Unk0x00af { get; set; }

    /// <summary>
    /// 0x00b1
    /// </summary>
    public Mercenary Mercenary { get; set; }

    /// <summary>
    /// 0x00bf [unk = 0x0] (server related data)
    /// </summary>
    [JsonIgnore]
    public byte[]? RealmData { get; set; }

    /// <summary>
    /// 0x014b
    /// </summary>
    public QuestsSection Quests { get; set; }

    /// <summary>
    /// 0x0279
    /// </summary>
    public WaypointsSection Waypoints { get; set; }

    /// <summary>
    /// 0x02c9
    /// </summary>
    public NPCDialogSection NPCDialog { get; set; }

    /// <summary>
    /// 0x02fc
    /// </summary>
    public Attributes Attributes { get; set; }

    public ClassSkills ClassSkills { get; set; }

    public ItemList PlayerItemList { get; set; }
    public CorpseList PlayerCorpses { get; set; }
    public MercenaryItemList? MercenaryItemList { get; set; }
    public Golem? Golem { get; set; }

    private D2S(IBitReader reader)
    {
        //0x0000 [signature = 0xaa, 0x55, 0xaa, 0x55]
        //0x0004 [version = 0x??, 0x00, 0x00, 0x00]
        //0x0008 [filesize]
        //0x000c [checksum]
        Header = Header.Read(reader);
        reader.Advance(4);
        if (Header.Version > SaveVersion.v200)
        {
            reader.Seek(267);
        }
        Name = reader.ReadString(16) ?? "";
        if (Header.Version > SaveVersion.v200)
        {
            reader.Seek(36);
        }
        Status = Status.Read(reader.ReadByte() ?? 0);   // 0x0024
        Progression = reader.ReadByte() ?? 0;           // 0x0025
        //Unk0x0026 = reader.ReadBytes(2);              // 0x0026
        ActiveWeapon = reader.ReadUInt16() ?? 0;        // 0x0026
        ClassId = (CharacterClass)(reader.ReadByte() ?? 0xff);    // 0x0028
        Unk0x0029 = reader.ReadBytes(2);    // 0x0029
        Level = reader.ReadByte() ?? 0;                 // 0x002b
        Created = reader.ReadUInt32() ?? 0;             // 0x002c
        LastPlayed = reader.ReadUInt32() ?? 0;          // 0x0030
        Unk0x0034 = reader.ReadBytes(4);    // 0x0034
        AssignedSkills = Enumerable                     // 0x0038
                           .Range(0, 16)
                           .Select(_ => Skill.Read(reader))
                           .ToArray();
        LeftSkill = Skill.Read(reader);                 // 0x0078
        RightSkill = Skill.Read(reader);                // 0x007c
        LeftSwapSkill = Skill.Read(reader);             // 0x0080
        RightSwapSkill = Skill.Read(reader);            // 0x0084
        Appearances = Appearances.Read(reader);         // 0x0088 [char menu appearance]
        Difficulties = Difficulties.Read(reader);       // 0x00a8
        MapId = reader.ReadUInt32() ?? 0;               // 0x00ab
        Unk0x00af = reader.ReadBytes(2);    // 0x00af [unk = 0x0, 0x0]
        Mercenary = Mercenary.Read(reader);             // 0x00b1
        RealmData = reader.ReadBytes(140);  // 0x00bf
        reader.Advance(4);                        // 0x014b [unk = 0x1, 0x0, 0x0, 0x0]
        Quests = QuestsSection.Read(reader);            // 0x014f [quests = 0x57, 0x6f, 0x6f, 0x21 "Woo!"]
        Waypoints = WaypointsSection.Read(reader);      // 0x0279 [waypoint data = 0x57, 0x53 "WS"]
        NPCDialog = NPCDialogSection.Read(reader);      // 0x02c9 [npc introduction data... unk]
        Attributes = Attributes.Read(reader);           // 0x02fd [attributes]

        ClassSkills = ClassSkills.Read(reader, ClassId);
        PlayerItemList = ItemList.Read(reader, Header.Version);
        PlayerCorpses = CorpseList.Read(reader, Header.Version);

        if (Status.IsExpansion)
        {
            MercenaryItemList = MercenaryItemList.Read(reader, Mercenary, Header.Version);
            Golem = Golem.Read(reader, Header.Version);
        }
    }

    public void Write(IBitWriter writer)
    {
        //Unk0x0026 ??= new byte[2];
        Unk0x0029 ??= [0x10, 0x1e];
        Unk0x0034 ??= [0xff, 0xff, 0xff, 0xff];
        Unk0x00af ??= new byte[2];
        RealmData ??= new byte[140];

        //0x0000 [signature = 0xaa, 0x55, 0xaa, 0x55]
        //0x0004 [version = 0x??, 0x00, 0x00, 0x00]
        //0x0008 [filesize (needs to be writen after all data)]
        //0x000c [checksum (needs to be calculated after all data writer)]
        Header.Write(writer);

        if (Header.Version > SaveVersion.v200)
        {
            writer.WriteBytes(new byte[20]);            // 0x0010
        }
        else
        {
            writer.WriteUInt32(ActiveWeapon);           // 0x0010
            writer.WriteString(Name, 16);         // 0x0014
        }
        
        Status.Write(writer);                           // 0x0024
        writer.WriteByte(Progression);                  // 0x0025
        //writer.WriteBytes(Unk0x0026);                 // 0x0026
        writer.WriteUInt16(ActiveWeapon);               // 0x0026
        writer.WriteByte((byte)ClassId);                // 0x0028
        writer.WriteBytes(Unk0x0029);                   // 0x0029
        writer.WriteByte(Level);                        // 0x002b
        writer.WriteUInt32(Created);                    // 0x002c
        writer.WriteUInt32(LastPlayed);                 // 0x0030
        writer.WriteBytes(Unk0x0034);                   // 0x0034
        for (int i = 0; i < 16; i++)                    // 0x0038
        {
            AssignedSkills[i].Write(writer);
        }
        LeftSkill.Write(writer);                        // 0x0078
        RightSkill.Write(writer);                       // 0x007c
        LeftSwapSkill.Write(writer);                    // 0x0080
        RightSwapSkill.Write(writer);                   // 0x0084
        Appearances.Write(writer);                      // 0x0088 [char menu appearance]
        Difficulties.Write(writer);                     // 0x00a8
        writer.WriteUInt32(MapId);                      // 0x00ab
        writer.WriteBytes(Unk0x00af);                   // 0x00af [unk = 0x0, 0x0]
        Mercenary.Write(writer);                        // 0x00b1

        if (Header.Version > SaveVersion.v200)
        {
            //writer.WriteBytes(Unk0x00bf);
            writer.WriteBytes(RealmData[..76].AsSpan());    // 0x00bf [unk]
            writer.WriteString(Name, 16);             // 0x010b [name]
            writer.WriteBytes(RealmData[92..].AsSpan());    // 0x011b [unk]
            writer.WriteUInt32(0x0);
        }
        else
        {
            writer.WriteBytes(RealmData[..140].AsSpan());   // 0x00bf [unk = 0x0] (server related data)
            writer.WriteUInt32(0x1);              // 0x014b [unk = 0x1, 0x0, 0x0, 0x0]
        }
        Quests.Write(writer);                           // 0x014f [quests = 0x57, 0x6f, 0x6f, 0x21 "Woo!"]
        Waypoints.Write(writer);                        // 0x0279 [waypoint data = 0x57, 0x53 "WS"]
        NPCDialog.Write(writer);                        // 0x02c9 [npc introduction data... unk]
        Attributes.Write(writer);                       // 0x02fd [attributes]
        ClassSkills.Write(writer);                      
        PlayerItemList.Write(writer, Header.Version);   
        PlayerCorpses.Write(writer, Header.Version);    
        if (Status.IsExpansion)
        {
            MercenaryItemList?.Write(writer, Mercenary, Header.Version);
            Golem?.Write(writer, Header.Version);
        }
    }

    public static D2S Read(ReadOnlySpan<byte> bytes)
    {
        using var reader = new BitReader(bytes);
        var d2s = new D2S(reader);
        Debug.Assert(reader.Position == (bytes.Length * 8));
        return d2s;
    }

    public static MemoryOwner<byte> WritePooled(D2S d2s)
    {
        using var writer = new BitWriter();
        d2s.Write(writer);
        var bytes = writer.ToPooledArray();
        Header.Fix(bytes.Span);
        return bytes;
    }

    public static byte[] Write(D2S d2s)
    {
        using var writer = new BitWriter();
        d2s.Write(writer);
        byte[] bytes = writer.ToArray();
        Header.Fix(bytes);
        return bytes;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                PlayerItemList.Dispose();
                PlayerCorpses.Dispose();
                MercenaryItemList?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
