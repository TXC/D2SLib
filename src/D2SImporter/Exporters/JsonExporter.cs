using D2SImporter.Model;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace D2SImporter
{
    public class JsonExporter
    {

        private static JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };

        public static void ExportJson(string outputPath, List<Unique> uniques, List<Runeword> runewords, List<CubeRecipe> cubeRecipes, List<Set> sets)
        {
            if (!Directory.Exists(outputPath))
            {
                throw new System.Exception("Could not find output directory");
            }

            var txtOutputDirectory = outputPath + "/json";

            if (!Directory.Exists(txtOutputDirectory))
            {
                Directory.CreateDirectory(txtOutputDirectory);
            }

            Serialize(txtOutputDirectory + "/uniques.json", uniques);
            Serialize(txtOutputDirectory + "/runewords.json", runewords);
            Serialize(txtOutputDirectory + "/cube_recipes.json", cubeRecipes);
            Serialize(txtOutputDirectory + "/sets.json", sets);
        }

        private static void Serialize<T>(string destination, List<T> values)
        {
            var json = JsonSerializer.Serialize(values, jsonOptions)
                                           .Replace("\\ufffd", "'");
            File.WriteAllText(destination, json, System.Text.Encoding.UTF8);
        }
    }
}
