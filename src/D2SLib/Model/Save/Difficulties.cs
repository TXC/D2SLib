using D2Shared.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace D2SLib.Model.Save;

public class Difficulties
{
    private readonly Difficulty[] _locations = new Difficulty[3];

    public Difficulty Normal { get => _locations[0]; set => _locations[0] = value; }
    public Difficulty Nightmare { get => _locations[1]; set => _locations[1] = value; }
    public Difficulty Hell { get => _locations[2]; set => _locations[2] = value; }

    public void Write(IBitWriter writer)
    {
        for (int i = 0; i < _locations.Length; i++)
        {
            _locations[i].Write(writer);
        }
    }

    public static Difficulties Read(IBitReader reader)
    {
        var locations = new Difficulties();
        var places = locations._locations;
        for (int i = 0; i < places.Length; i++)
        {
            places[i] = Difficulty.Read(reader);
        }
        return locations;
    }

    [Obsolete("Try the direct-read overload!")]
    public static Difficulties Read(ReadOnlySpan<byte> bytes)
    {
        using var reader = new BitReader(bytes);
        return Read(reader);
    }

    [Obsolete("Try the non-allocating overload!")]
    public static byte[] Write(Difficulties locations)
    {
        using var writer = new BitWriter();
        locations.Write(writer);
        return writer.ToArray();
    }
}

public readonly struct Difficulty : IEquatable<Difficulty>
{
    private readonly InternalBitArray _flags;
    public Difficulty(byte flags)
    {
        _flags = new InternalBitArray(stackalloc byte[] { flags });
    }
    /*
    public Difficulty(bool active, byte act)
    {
        Active = active;
        Act = act;
    }
    */
    [JsonIgnore]
    public IList<bool> Flags => [.. _flags];

    public readonly bool Active { get => Flags[7]; }
    public readonly byte Act { get => GetAct(); }

    public void Write(IBitWriter writer)
    {
        /*
        byte b = 0x0;
        if (Active)
        {
            b |= 0x7;
        }

        b |= (byte)(Act - 1);

        writer.WriteByte(b);
        */
        writer.WriteBits(Flags);
    }

    public static Difficulty Read(IBitReader reader)
    {
        byte b = reader.ReadByte() ?? 0;
        return new Difficulty(b);
        //return new Difficulty(
        //    active: (b >> 7) == 1,
        //    act: (byte)((b & 0x5) + 1)
        //);
    }

    private byte GetAct()
    {
        int result = 0;
        for (int i = 0; i < 3; i++)
        {
            int bitMask = 1 << i;
            if (Flags[i])
            {
                result |= bitMask;
            }
            else
            {
                result &= ~bitMask;
            }
        }
        return (byte)result;
    }

    public bool Equals(Difficulty other)
    {
        return Active == other.Active
            && Act == other.Act;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Difficulty other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Active, Act);

    public static bool operator ==(Difficulty left, Difficulty right) => left.Equals(right);

    public static bool operator !=(Difficulty left, Difficulty right) => !left.Equals(right);
}
