using D2Shared.IO;
using D2Shared.Enums;
using System;

namespace D2SLib.Model.Save;

public sealed class D2I : IDisposable
{
    private D2I(IBitReader reader, SaveVersion version)
    {
        ItemList = ItemList.Read(reader, version);
    }

    public ItemList ItemList { get; }

    public void Write(IBitWriter writer, uint version)
        => Write(writer, (SaveVersion)version);

    public void Write(IBitWriter writer, SaveVersion version)
        => ItemList.Write(writer, version);

    public static D2I Read(IBitReader reader, uint version)
        => new(reader, (SaveVersion)version);

    public static D2I Read(IBitReader reader, SaveVersion version)
        => new(reader, version);

    public static D2I Read(ReadOnlySpan<byte> bytes, uint version)
    {
        using var reader = new BitReader(bytes);
        return new D2I(reader, (SaveVersion)version);
    }

    public static D2I Read(ReadOnlySpan<byte> bytes, SaveVersion version)
    {
        using var reader = new BitReader(bytes);
        return new D2I(reader, version);
    }

    public static byte[] Write(D2I d2i, uint version)
    {
        using var writer = new BitWriter();
        d2i.Write(writer, (SaveVersion)version);
        return writer.ToArray();
    }

    public static byte[] Write(D2I d2i, SaveVersion version)
    {
        using var writer = new BitWriter();
        d2i.Write(writer, version);
        return writer.ToArray();
    }

    public void Dispose() => ItemList?.Dispose();
}
