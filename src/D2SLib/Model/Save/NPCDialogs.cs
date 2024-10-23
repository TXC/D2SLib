using D2Shared.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace D2SLib.Model.Save;

public sealed class NPCDialogSection
{
    private readonly NPCDialogDifficulty[] _difficulties = new NPCDialogDifficulty[3];

    //0x02c9 [npc header identifier  = 0x01, 0x77 ".w"]
    public ushort? Header { get; set; }
    //0x02ca [npc header length = 0x34]
    public ushort? Length { get; set; }
    public NPCDialogDifficulty Normal => _difficulties[0];
    public NPCDialogDifficulty Nightmare => _difficulties[1];
    public NPCDialogDifficulty Hell => _difficulties[2];

    public void Write(IBitWriter writer)
    {
        writer.WriteUInt16(Header ?? 0x7701);
        writer.WriteUInt16(Length ?? 0x34);

        int start = writer.Position;

        for (int i = 0; i < _difficulties.Length; i++)
        {
            _difficulties[i].Write(writer);
        }

        writer.SeekBits(start + (0x30 * 8));
    }

    public static NPCDialogSection Read(IBitReader reader)
    {
        var npcDialogSection = new NPCDialogSection
        {
            Header = reader.ReadUInt16(),
            Length = reader.ReadUInt16()
        };

        Span<byte> bytes = stackalloc byte[0x30];
        //Span<byte> bytes = stackalloc byte[0x2F];
        reader.ReadBytes(bytes);

        using var bits = new InternalBitArray(bytes);

        for (int i = 0; i < npcDialogSection._difficulties.Length; i++)
        {
            int j = i * 64;
            int k = j + 64;
            var introbits = bits[j..k];
            var congratbits = bits[(j + 0xc0)..(k + 0xc0)];
            npcDialogSection._difficulties[i] = NPCDialogDifficulty.Read(introbits, congratbits);
            npcDialogSection._difficulties[i].Intro = introbits;
            npcDialogSection._difficulties[i].Congrat = congratbits;
        }

        return npcDialogSection;
    }

    [Obsolete("Try the direct-read overload!")]
    public static NPCDialogSection Read(ReadOnlySpan<byte> bytes)
    {
        using var reader = new BitReader(bytes);
        return Read(reader);
    }

    [Obsolete("Try the non-allocating overload!")]
    public static byte[] Write(NPCDialogSection npcDialogSection)
    {
        using var writer = new BitWriter();
        npcDialogSection.Write(writer);
        return writer.ToArray();
    }

    /*
    private static List<string> ToBase2(InternalBitArray bits)
    {
        List<string> output = [];
        for (int i = 0; i < bits.Count / 8; i++)
        {
            int z = i * 8;

            var bytes = new byte[1];
            bits.CopyTo(bytes, z);
            output.Add($"{bytes[0]:b8}");
        }
        return output;
    }
    */
}

// 8 bytes per difficulty for Intro for each Difficulty
// followed by 8 bytes per difficulty for Congrats for each difficulty
public sealed class NPCDialogDifficulty
{
    private readonly NPCDialogData[] _dialogs = new NPCDialogData[40];

    private NPCDialogDifficulty() { }

    #region Act I NPCs
    /// <summary>
    /// $00 offset 7
    /// </summary>
    public NPCDialogData WarrivActII => _dialogs[0];
    //public NPCDialogData Unk0x0001 => _dialogs[1];

    /// <summary>
    /// $00 offset 5
    /// </summary>
    public NPCDialogData Charsi => _dialogs[2];

    /// <summary>
    /// $00 offset 4
    /// </summary>
    public NPCDialogData WarrivActI => _dialogs[3];

    /// <summary>
    /// $00 offset 3
    /// </summary>
    public NPCDialogData Kashya => _dialogs[4];

    /// <summary>
    /// $00 offset 2
    /// </summary>
    public NPCDialogData Akara => _dialogs[5];

    /// <summary>
    /// $00 offset 1
    /// </summary>
    public NPCDialogData Gheed => _dialogs[6];
    //public NPCDialogData Unk0x0007 => _dialogs[7];
    #endregion Act I NPCs

    #region Act II NPCs
    /// <summary>
    /// $01 offset 7
    /// </summary>
    public NPCDialogData Greiz => _dialogs[8];

    /// <summary>
    /// $01 offset 6
    /// </summary>
    public NPCDialogData Jerhyn => _dialogs[9];

    /// <summary>
    /// $01 offset 5
    /// </summary>
    public NPCDialogData MeshifActII => _dialogs[10];

    /// <summary>
    /// $01 offset 4
    /// </summary>
    public NPCDialogData Geglash => _dialogs[11];

    /// <summary>
    /// $01 offset 3
    /// </summary>
    public NPCDialogData Lysander => _dialogs[12];

    /// <summary>
    /// $01 offset 2
    /// </summary>
    public NPCDialogData Fara => _dialogs[13];

    /// <summary>
    /// $01 offset 1
    /// </summary>
    public NPCDialogData Drogan => _dialogs[14];
    //public NPCDialogData Unk0x000F => _dialogs[15];
    #endregion Act II NPCs

    #region Act III NPCs
    /// <summary>
    /// $01 offset 7
    /// </summary>
    public NPCDialogData Alkor => _dialogs[16];

    /// <summary>
    /// $01 offset 6
    /// </summary>
    public NPCDialogData Hratli => _dialogs[17];

    /// <summary>
    /// $01 offset 5
    /// </summary>
    public NPCDialogData Ashera => _dialogs[18];
    //public NPCDialogData Unk0x0013 => _dialogs[19];
    //public NPCDialogData Unk0x0014 => _dialogs[20];

    /// <summary>
    /// $01 offset 2
    /// </summary>
    public NPCDialogData CainActIII => _dialogs[21];
    //public NPCDialogData Unk0x0016 => _dialogs[22];

    /// <summary>
    /// $01 offset 0
    /// </summary>
    public NPCDialogData Elzix => _dialogs[23];

    #endregion Act III NPCs

    #region Act IV NPCs
    /// <summary>
    /// $01 offset 7
    /// </summary>
    public NPCDialogData Malah => _dialogs[24];

    /// <summary>
    /// $01 offset 6
    /// </summary>
    public NPCDialogData Anya => _dialogs[25];
    //public NPCDialogData Unk0x001A => _dialogs[26];

    /// <summary>
    /// $01 offset 4
    /// </summary>
    public NPCDialogData Natalya => _dialogs[27];

    /// <summary>
    /// $01 offset 3
    /// </summary>
    public NPCDialogData MeshifActIII => _dialogs[28];
    //public NPCDialogData Unk0x001D => _dialogs[29];
    //public NPCDialogData Unk0x001F => _dialogs[30];

    /// <summary>
    /// $01 offset 0
    /// </summary>
    public NPCDialogData Ormus => _dialogs[31];
    #endregion Act IV NPCs

    #region Act V NPCs
    //public NPCDialogData Unk0x0021 => _dialogs[32];
    //public NPCDialogData Unk0x0022 => _dialogs[33];
    //public NPCDialogData Unk0x0023 => _dialogs[34];
    //public NPCDialogData Unk0x0024 => _dialogs[35];
    //public NPCDialogData Unk0x0025 => _dialogs[36];

    /// <summary>
    /// $01 offset 2
    /// </summary>
    public NPCDialogData CainActV => _dialogs[37];

    /// <summary>
    /// $01 offset 1
    /// </summary>
    public NPCDialogData Qualkehk => _dialogs[38];

    /// <summary>
    /// $01 offset 0
    /// </summary>
    public NPCDialogData Nihlathak => _dialogs[39];

    //public NPCDialogData Unk0x0029 => _dialogs[40];
    #endregion Act V NPCs

    // 23 bits here unused
    internal InternalBitArray Intro { get; set; }
    internal InternalBitArray Congrat { get; set; }

    public void Write(IBitWriter writer)
    {
        for (int i = 0; i < _dialogs.Length; i++)
        {
            int position = writer.Position;
            writer.WriteBit(_dialogs[i].Introduction);
            writer.SeekBits(position + 0xC0);
            writer.WriteBit(_dialogs[i].Congratulations);
            writer.SeekBits(position + 1);
        }
        //writer.Align();
        writer.WriteBytes([0x0, 0x0, 0x0]);
    }

    internal static NPCDialogDifficulty Read(InternalBitArray bits)
    {
        var output = new NPCDialogDifficulty();

        for (int i = 0; i < output._dialogs.Length; i++)
        {
            var data = new NPCDialogData
            {
                Introduction = bits[i],
                Congratulations = bits[i + (0x18 * 8)],
            };
            output._dialogs[i] = data;
        }

        return output;
    }
    internal static NPCDialogDifficulty Read(InternalBitArray intro, InternalBitArray congrat)
    {
        var output = new NPCDialogDifficulty();

        for (int i = 0; i < output._dialogs.Length; i++)
        {
            var data = new NPCDialogData
            {
                Introduction = intro[i],
                Congratulations = congrat[i],
            };
            output._dialogs[i] = data;
        }

        return output;
    }
}

public sealed class NPCDialogData
{
    public bool Introduction { get; set; }
    public bool Congratulations { get; set; }

    public override string ToString()
        => $"Introduced: {Introduction}, Congratulated: {Congratulations} ";
}
