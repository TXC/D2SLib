using CommunityToolkit.HighPerformance.Buffers;
using D2Shared.IO;
using D2Shared.Enums;
using D2Shared.Huffman;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using D2SImporter;
using D2SLib.Model.Save;

namespace D2SLib;

public class Core
{
    public static IImporter Importer { get => Main.Importer; }

    #region Read Savefile
    public static D2S ReadD2S(string path)
        => D2S.Read(File.ReadAllBytes(path));

    public static async Task<D2S> ReadD2SAsync(string path, CancellationToken ct = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
        return D2S.Read(bytes);
    }
    #endregion Read Savefile

    #region Read Item
    public static D2S ReadD2S(ReadOnlySpan<byte> bytes)
        => D2S.Read(bytes);

    public static Item ReadItem(string path, SaveVersion version)
        => ReadItem(File.ReadAllBytes(path), version);

    public static Item ReadItem(string path, uint version)
        => ReadItem(File.ReadAllBytes(path), (SaveVersion)version);

    public static async Task<Item> ReadItemAsync(string path, SaveVersion version, CancellationToken ct = default)
        => await ReadItemAsync(path, version, ct);

    public static async Task<Item> ReadItemAsync(string path, uint version, CancellationToken ct = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
        return Item.Read(bytes, (SaveVersion)version);
    }

    public static Item ReadItem(ReadOnlySpan<byte> bytes, SaveVersion version)
        => Item.Read(bytes, version);

    public static Item ReadItem(ReadOnlySpan<byte> bytes, uint version)
        => Item.Read(bytes, (SaveVersion)version);
    #endregion Read Item

    #region Read Stash
    public static D2I ReadD2I(string path, uint version)
        => D2I.Read(File.ReadAllBytes(path), (SaveVersion)version);

    public static D2I ReadD2I(string path, SaveVersion version)
        => D2I.Read(File.ReadAllBytes(path), version);

    public static async Task<D2I> ReadD2IAsync(string path, uint version, CancellationToken ct = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
        return D2I.Read(bytes, (SaveVersion)version);
    }

    public static async Task<D2I> ReadD2IAsync(string path, SaveVersion version, CancellationToken ct = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
        return D2I.Read(bytes, version);
    }

    public static D2I ReadD2I(ReadOnlySpan<byte> bytes, uint version)
        => D2I.Read(bytes, (SaveVersion)version);

    public static D2I ReadD2I(ReadOnlySpan<byte> bytes, SaveVersion version)
        => D2I.Read(bytes, version);
    #endregion Read Stash

    #region Write Savefile
    public static MemoryOwner<byte> WriteD2SPooled(D2S d2s)
        => D2S.WritePooled(d2s);

    public static byte[] WriteD2S(D2S d2s)
        => D2S.Write(d2s);

    public static MemoryOwner<byte> WriteItemPooled(Item item, uint version)
    {
        using var writer = new BitWriter();
        item.Write(writer, version);
        return writer.ToPooledArray();
    }
    #endregion Write Savefile

    #region Write Item
    public static byte[] WriteItem(Item item, SaveVersion version)
        => Item.Write(item, version);

    public static byte[] WriteItem(Item item, uint version)
        => Item.Write(item, (SaveVersion)version);
    #endregion Write Item

    #region Write Stash
    public static MemoryOwner<byte> WriteD2IPooled(D2I d2i, SaveVersion version)
    {
        using var writer = new BitWriter();
        d2i.Write(writer, version);
        return writer.ToPooledArray();
    }

    public static MemoryOwner<byte> WriteD2IPooled(D2I d2i, uint version)
    {
        using var writer = new BitWriter();
        d2i.Write(writer, (SaveVersion)version);
        return writer.ToPooledArray();
    }

    public static byte[] WriteD2I(D2I d2i, SaveVersion version)
        => D2I.Write(d2i, version);

    public static byte[] WriteD2I(D2I d2i, uint version)
        => D2I.Write(d2i, (SaveVersion)version);
    #endregion Write Stash

    #region Helpers
    internal static HuffmanTree ItemCodeTree => IImporter.ItemCodeTree;

    #endregion Helpers
}
