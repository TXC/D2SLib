using D2SImporter.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    [FileName("Misc.txt")]
    public class Misc : Equipment, ID2Data
    {
        [JsonIgnore, ColumnName("autobelt")]
        public bool AutoBelt { get; set; }

        [JsonIgnore, ColumnName("spellicon")]
        public string SpellIcon { get; set; }

        [JsonIgnore, ColumnName("pSpell")]
        public int? PSpell { get; set; }

        [JsonIgnore, ColumnName("state")]
        public string State { get; set; }

        [JsonIgnore, ColumnName("cstate1")]
        public string CState1 { get; set; }

        [JsonIgnore, ColumnName("cstate2")]
        public string CState2 { get; set; }

        [JsonIgnore, ColumnName("len")]
        public int? TempItemTimer { get; set; }

        [JsonIgnore, ColumnName("stat1")]
        public string Stat1 { get; set; }

        [JsonIgnore, ColumnName("calc1")]
        public int? Calc1 { get; set; }

        [JsonIgnore, ColumnName("stat2")]
        public string Stat2 { get; set; }

        [JsonIgnore, ColumnName("calc2")]
        public int? Calc2 { get; set; }

        [JsonIgnore, ColumnName("stat3")]
        public string Stat3 { get; set; }

        [JsonIgnore, ColumnName("calc3")]
        public int? Calc3 { get; set; }

        [JsonIgnore, ColumnName("spelldesc")]
        public string SpellDesc { get; set; }

        [JsonIgnore, ColumnName("spelldescstr")]
        public string SpellDescStr { get; set; }

        [JsonIgnore, ColumnName("spelldescstr2")]
        public string SpellDescStr2 { get; set; }

        [JsonIgnore, ColumnName("spelldesccalc")]
        public string SpellDescCalc { get; set; }

        [JsonIgnore, ColumnName("spelldesccolor")]
        public string SpellDescColor { get; set; }

        [JsonIgnore, ColumnName("BetterGem")]
        public string BetterGem { get; set; }

        [JsonIgnore, ColumnName("multibuy")]
        public bool MultiBuy { get; set; }

        public Misc() : base()
        {
            EquipmentType = EquipmentType.Jewelery;
        }

        public new object Clone()
        {
            Misc eq = base.Clone() as Misc;
            eq.EquipmentType = this.EquipmentType;
            eq.AutoBelt = this.AutoBelt;
            eq.SpellIcon = this.SpellIcon;
            eq.PSpell = this.PSpell;
            eq.State = this.State;
            eq.CState1 = this.CState1;
            eq.CState2 = this.CState2;
            eq.TempItemTimer = this.TempItemTimer;
            eq.Stat1 = this.Stat1;
            eq.Calc1 = this.Calc1;
            eq.Stat2 = this.Stat2;
            eq.Calc2 = this.Calc2;
            eq.Stat3 = this.Stat3;
            eq.Calc3 = this.Calc3;
            eq.SpellDesc = this.SpellDesc;
            eq.SpellDescStr = this.SpellDescStr;
            eq.SpellDescStr2 = this.SpellDescStr2;
            eq.SpellDescCalc = this.SpellDescCalc;
            eq.SpellDescColor = this.SpellDescColor;
            eq.BetterGem = this.BetterGem;
            eq.MultiBuy = this.MultiBuy;

            return eq;
        }
    }
}
