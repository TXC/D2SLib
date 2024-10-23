using CommunityToolkit.HighPerformance.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace D2SImporter.Model
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct TblHeader
    {
        /// <summary>
        /// +0x00 - CRC value for string table
        /// </summary>
        [FieldOffset(0x00)]
        public ushort CRC;

        /// <summary>
        /// +0x02 - size of Indices array
        /// </summary>
        [FieldOffset(0x02)]
        public ushort NodesNumber;

        /// <summary>
        /// +0x04 - size of TblHashNode array
        /// </summary>
        [FieldOffset(0x04)]
        public uint HashTableSize;

        /// <summary>
        /// +0x08 - file version, either 0 or 1, doesn't matter
        /// </summary>
        [FieldOffset(0x08)]
        public byte Version;

        /// <summary>
        /// +0x09 - string table start offset
        /// </summary>
        [FieldOffset(0x09)]
        public uint DataStartOffset;

        /// <summary>
        /// +0x0D - max number of collisions for string key search based on its hash value
        /// </summary>
        [FieldOffset(0x0D)]
        public uint HashMaxTries;

        /// <summary>
        /// +0x11 - size of the file
        /// </summary>
        [FieldOffset(0x011)]
        public uint FileSize;

        public const int size = 0x15;
    };

    /// <summary>
    /// node of the hash table in string *.tbl file
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct TblHashNode
    {
        /// <summary>
        /// <para><b>Offset: </b>+0x00</para>
        /// <para>shows if the entry is used.</para>
        /// <para>if 0, then it has been "deleted" from the table</para>
        /// </summary>
        [FieldOffset(0x00)]
        public byte Active;

        /// <summary>
        /// <para><b>Offset: </b>+0x01</para>
        /// <para>index in Indices array</para>
        /// </summary>
        [FieldOffset(0x01)]
        public ushort Index;

        /// <summary>
        /// <para><b>Offset: </b>+0x03</para>
        /// <para>hash value of the current string key</para>
        /// </summary>
        [FieldOffset(0x03)]
        public uint HashValue;

        /// <summary>
        /// <para><b>Offset: </b>+0x07</para>
        /// <para>offset of the current string key</para>
        /// </summary>
        [FieldOffset(0x07)]
        public uint StringKeyOffset;

        /// <summary>
        /// <para><b>Offset: </b>+0x0B</para>
        /// <para>offset of the current string value</para>
        /// </summary>
        [FieldOffset(0x0B)]
        public uint StringValOffset;

        /// <summary>
        /// <para><b>Offset: </b>+0x0F</para>
        /// <para>length of the current string value</para>
        /// </summary>
        [FieldOffset(0x0F)]
        public ushort StringValLength;

        /// <summary>
        /// <para>Calculated value</para>
        /// <para>length of the current string key</para>
        /// </summary>
        public readonly ushort StringKeyLength
        {
            get => (ushort)(StringValOffset - StringKeyOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        public const int size = 0x11;
    };

    internal struct TableList
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        public override readonly string ToString()
            => $"{Index}: '{Key}' - '{Value}'";
    };

    // Code taken from: https://github.com/kambala-decapitator/QTblEditor
    internal class TableProcessor
    {
        public static List<TableList> ReadTablesFile(string path, int offset = 0)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                return ReadTablesFile(fs, offset);
            }
            catch (Exception e)
            {
                throw new Exception($"Table '{path}' seems to be corrupt", e);
            }
        }

        public static List<TableList> ReadTablesFile(Stream stream, int offset = 0)
        {
            using var br = new BinaryReader(stream, Encoding.UTF8);
            // Read the header
            var header = GetHeader(br);

            // number of bytes to read without header
            var numElem = header.FileSize - TblHeader.size;

            // Check we can read the entire file
            var byteArray = br.ReadBytes((int)numElem);
            if (byteArray.Length != numElem)
            {
                throw new Exception($"Table seems to be corrupt");
            }

            br.BaseStream.Position = TblHeader.size;

            // Read the table
            return GetStringTable(br, header, offset);
        }

        private static List<TableList> GetStringTable(BinaryReader br, TblHeader header, int offset = 0)
        {
            br.BaseStream.Position += header.NodesNumber * sizeof(ushort);
            var hashNodes = new List<TblHashNode>();

            for (uint i = 0; i < header.HashTableSize; i++)
            {
                hashNodes.Add(GetHashNode(br));
            }

            br.BaseStream.Position = 0;

            //var result = BuildTableBuffer(br, hashNodes);
            var tableList = BuildTableStream(br, hashNodes, offset);

            //var result = tableList.ToDictionary(x => x.Key, x => x.Value);

            return tableList;
        }

        private static List<TableList> BuildTableBuffer(BinaryReader br, List<TblHashNode> hashNodes, int offset = 0)
        {
            var result = new Dictionary<string, string>();
            var tableList = new List<TableList>();

            var byteArray = ReadAllBytes(br);
            foreach (var hashNode in hashNodes)
            {

                if (hashNode.Active == 0)
                {
                    continue;
                }
                else if (hashNode.Active != 1)
                {
                    continue;
                }

                string? val = null;
                string key;

                val = Encoding.UTF8.GetString(byteArray, (int)hashNode.StringValOffset, hashNode.StringValLength).Trim('\0');
                key = Encoding.UTF8.GetString(byteArray, (int)hashNode.StringKeyOffset, hashNode.StringKeyLength).Trim('\0');

                tableList.Add(new TableList { Key = key, Value = val ?? "", Index = offset + hashNode.Index });
            }

            tableList = [.. tableList.OrderBy(x => x.Index)];

            return tableList;
        }

        private static List<TableList> BuildTableStream(BinaryReader br, List<TblHashNode> hashNodes, int offset = 0)
        {
            var tableList = new List<TableList>();

            foreach (var hashNode in hashNodes)
            {
                if (hashNode.Active == 0)
                {
                    continue;
                }
                else if (hashNode.Active != 1)
                {
                    continue;
                }

                byte[] valArray = new byte[hashNode.StringValLength];
                byte[] keyArray = new byte[hashNode.StringKeyLength];

                string? val = null;
                string key;

                //br.Read(valArray, (int)hashNode.StringValOffset, (int)hashNode.StringValLength);
                br.BaseStream.Position = hashNode.StringValOffset;
                valArray = br.ReadBytes(hashNode.StringValLength);

                //br.Read(keyArray, (int)hashNode.StringKeyOffset, (int)hashNode.StringKeyLength);
                br.BaseStream.Position = hashNode.StringKeyOffset;
                keyArray = br.ReadBytes(hashNode.StringKeyLength);

                val = Encoding.UTF8.GetString(valArray).Trim('\0');
                key = Encoding.UTF8.GetString(keyArray).Trim('\0');

                tableList.Add(new TableList { Key = key, Value = val ?? "", Index = offset + hashNode.Index });
            }

            tableList = [.. tableList.OrderBy(x => x.Index)];

            return tableList;
        }

        private static TblHeader GetHeader(BinaryReader br)
        {
            byte[] readBuffer = br.ReadBytes(TblHeader.size);

            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var header = (TblHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TblHeader));
            handle.Free();

            return header;
        }

        private static TblHashNode GetHashNode(BinaryReader br)
        {
            byte[] readBuffer = br.ReadBytes(TblHashNode.size);

            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var result = (TblHashNode)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TblHashNode));
            handle.Free();

            return result;
        }

        private static byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 4096;

            using var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, count);
            }
            return ms.ToArray();
        }
    }
}
