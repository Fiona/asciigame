using System;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Color = Microsoft.Xna.Framework.Color;

namespace asciigame.Core
{

    public class XNAColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            var colors = new JArray();
            colors.Add(color.R);
            colors.Add(color.G);
            colors.Add(color.B);
            colors.Add(color.A);
            colors.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray obj = (JArray) serializer.Deserialize(reader);
            if(obj == null)
                return Color.Magenta;
            return new Color((int)obj[0], (int)obj[1], (int)obj[2], (int)obj[3]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }

    public class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            var output = $"<{vector.X}, {vector.Y}, {vector.Z}>";
            new JValue(output).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var vector = new Vector3();

            var obj = (string)serializer.Deserialize(reader);
            if(obj == null)
                return vector;
            foreach(var c in new []{"<", ">", " "})
                obj = obj.Replace(c, string.Empty);
            var parts = obj.Split(',');
            vector.X = float.Parse(parts[0]);
            vector.Y = float.Parse(parts[1]);
            vector.Z = float.Parse(parts[2]);
            return vector;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }

    public static class JsonHandler
    {
        public static T Deserialize<T>(string jsonContents)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonContents, GetSerializerSettings());
            }
            catch(JsonException e)
            {
                throw new JsonErrorException(e.Message, e);
            }
        }

        public static string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, GetSerializerSettings());
            }
            catch(JsonException e)
            {
                throw new JsonErrorException(e.Message, e);
            }
        }

        public static void SerializeToFile(object obj, string filePath)
        {
            File.WriteAllText(filePath, Serialize(obj));
        }

        public static void PopulateObject(string json, object obj)
        {
            JsonConvert.PopulateObject(json, obj, GetSerializerSettings());
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            serializerSettings.Converters.Add(new XNAColorConverter());
            serializerSettings.Converters.Add(new Vector3Converter());
            return serializerSettings;
        }
    }

}