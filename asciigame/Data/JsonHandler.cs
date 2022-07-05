using System;
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

    public class FourCharTupleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tuple = ((char, char, char, char))value;
            var jTuple = new JArray();
            jTuple.Add(tuple.Item1);
            jTuple.Add(tuple.Item2);
            jTuple.Add(tuple.Item3);
            jTuple.Add(tuple.Item4);
            jTuple.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray obj = (JArray) serializer.Deserialize(reader);
            if(obj == null)
                return ("", "", "", "");
            return ((char) obj[0], (char) obj[1], (char) obj[2], (char) obj[3]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof((char, char, char, char));
        }
    }

    public class TwoCharTupleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tuple = ((char, char))value;
            var jTuple = new JArray();
            jTuple.Add(tuple.Item1);
            jTuple.Add(tuple.Item2);
            jTuple.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray obj = (JArray) serializer.Deserialize(reader);
            if(obj == null)
                return ("", "");
            return ((char) obj[0], (char) obj[1]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof((char, char));
        }
    }

    public class FourIntTupleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tuple = ((int, int, int, int))value;
            var jTuple = new JArray();
            jTuple.Add(tuple.Item1);
            jTuple.Add(tuple.Item2);
            jTuple.Add(tuple.Item3);
            jTuple.Add(tuple.Item4);
            jTuple.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray obj = (JArray) serializer.Deserialize(reader);
            if(obj == null)
                return ("", "", "", "");
            return ((int) obj[0], (int) obj[1], (int) obj[2], (int) obj[3]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof((int, int, int, int));
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
            serializerSettings.Converters.Add(new TwoCharTupleConverter());
            serializerSettings.Converters.Add(new FourCharTupleConverter());
            serializerSettings.Converters.Add(new FourIntTupleConverter());
            return serializerSettings;
        }
    }

}