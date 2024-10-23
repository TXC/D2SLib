using D2Shared;
using D2Shared.Enums;
using D2SImporter;
using D2SImporter.Model;
using D2Shared.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2SLib.Model.Save;

//variable size. depends on # of attributes
public class Attributes
{
    public ushort? Header { get; set; }
    public Dictionary<string, int> Stats { get; } = [];

    public static Attributes Read(IBitReader reader)
    {
        //var itemStatCost = Core.MetaData.ItemStatCostData;
        var attributes = new Attributes
        {
            Header = reader.ReadUInt16()
        };
        if (!attributes.Header.HasValue
            || (attributes.Header.HasValue && attributes.Header.Value != 0x6667)
        )
        {
            byte level = reader.GetByte(0x2b) ?? 0;
            if (level == 0x01)
            {
                CharacterClass charClass = (CharacterClass)(reader.GetByte(0x28) ?? 0xff);
                var shortClass = Utility.ToEnumString(charClass);
                if (!Core.Importer.CharStats.TryGetValue(shortClass, out CharStat charStat))
                {
                    throw ItemStatCostException.Create($"Could not find character skill tab '{charClass}' property");
                }

                attributes.Stats.TryAdd("strength", charStat.Strength ?? 0);
                attributes.Stats.TryAdd("energy", charStat.Energy ?? 0);
                attributes.Stats.TryAdd("dexterity", charStat.Dexterity ?? 0);
                attributes.Stats.TryAdd("vitality", charStat.Vitality ?? 0);
                attributes.Stats.TryAdd("statpts", 0);
                attributes.Stats.TryAdd("newskills", 0);
                attributes.Stats.TryAdd("hitpoints", (charStat.Vitality ?? 0) + (charStat.HealthBonus ?? 0));
                attributes.Stats.TryAdd("maxhp", (charStat.Vitality ?? 0) + (charStat.HealthBonus ?? 0));
                attributes.Stats.TryAdd("mana", charStat.Energy ?? 0);
                attributes.Stats.TryAdd("maxmana", charStat.Energy ?? 0);
                attributes.Stats.TryAdd("stamina", charStat.Stamina ?? 0);
                attributes.Stats.TryAdd("maxstamina", charStat.Stamina ?? 0);
                attributes.Stats.TryAdd("level", 1);
                attributes.Stats.TryAdd("experience", 0);
                attributes.Stats.TryAdd("gold", 0);
                attributes.Stats.TryAdd("goldbank", 0);

                return attributes;
            }
            throw new Exception($"Attribute header 'gf' not found at position ${(reader.Position - 2) * 8}");
        }

        int bitoffset = 0;
        ushort id = reader.ReadUInt16(9) ?? 0x1ff;
        while (id != 0x1ff) // if (0x69 & 66)
        {
            var property = Core.Importer.ItemStatCosts.Values.FirstOrDefault(x => x.Id == id);

            if (property is null)
            {
                throw new NullReferenceException($"Could not locate {id} in ItemStatCost.txt");
            }

            //var property = Core.Importer.ItemStatCosts[id.ToString()]; 
            //var property = itemStatCost.GetById(id);
            int attribute = reader.ReadInt32(property?.CSvBits ?? 0) ?? 0;
            int valShift = property?.ValShift ?? 0;
            if (valShift > 0)
            {
                attribute >>= valShift;
            }

            attributes.Stats.TryAdd(property?.Stat ?? string.Empty, attribute);
            //attributes.Stats.Add(property?.Stat ?? string.Empty, attribute);
            bitoffset += property?.CSvBits ?? 0;
            id = reader.ReadUInt16(9) ?? 0x1ff;
        }
        reader.Align();
        return attributes;
    }

    public void Write(IBitWriter writer)
    {
        //var itemStatCost = Core.MetaData.ItemStatCostData;
        writer.WriteUInt16(Header ?? 0x6667); // gf
        foreach (var entry in Stats)
        {
            var property = Core.Importer.ItemStatCosts[entry.Key];
            if (property?.Saved != true)
            {
                continue;
            }
            //var property = itemStatCost.GetByStat(entry.Key);
            writer.WriteUInt16((ushort)property.Id, 9);
            int attribute = entry.Value;
            int valShift = property.ValShift ?? 0;
            if (valShift > 0)
            {
                attribute <<= valShift;
            }
            writer.WriteInt32(attribute, property.CSvBits ?? 0);
        }
        writer.WriteUInt16(0x1ff, 9);
        writer.Align();
    }

    [Obsolete("Try the non-allocating overload!")]
    public static byte[] Write(Attributes attributes)
    {
        using var writer = new BitWriter();
        attributes.Write(writer);
        return writer.ToArray();
    }
}
