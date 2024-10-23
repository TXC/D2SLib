/*
using D2Shared;
using System.Collections.Generic;

namespace D2SImporter
{
    public abstract class BaseListModel<TValue> : BaseModel<TValue>
        where TValue : IListImport<TValue>, new()
    {
        protected abstract List<TValue> ImportTable(List<Dictionary<string, string>> tableList);
    }

    public abstract class BaseDictModel<TKey, TValue> : BaseModel<TValue>
        where TValue : IDictImport<TKey, TValue>, new()
    {
        protected abstract Dictionary<TKey, TValue> ImportTable(List<Dictionary<string, string>> tableList);
    }

    public abstract class BaseModel<TValue>
        where TValue : IModelImport<TValue>, new()
    {
        public static TValue Import(string excelFolder, string FileName)
        {
            var table = Utility.ReadTxtFileToDictionaryList(excelFolder + $"/{FileName}");
            TValue res = new();
            res.ImportTable(table);
            return res;
        }

        public static TValue ImportFromAssembly(string FileName, string Version)
        {
            var table = Utility.ReadAssemblyFileToDictionaryList($"Excel.{FileName}", Version);
            TValue res = new();
            res.ImportTable(table);
            return res;
        }
    }
}
*/
