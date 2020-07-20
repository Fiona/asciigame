using System;
using System.Diagnostics;
using System.IO;
using asciigame.Core;

namespace asciigame.Data
{
    public abstract class Data<T> where T : Data<T>, new()
    {
        private static Lazy<T> _instance = new Lazy<T>(Get);
        private static T Instance => _instance.Value;

        protected abstract string GetDataFilePath();

        public static T Get()
        {
            if(_instance.IsValueCreated)
                return _instance.Value;

            var pathData = new T();
            var path = pathData.GetDataFilePath();

            if(!File.Exists(path))
            {
                Debug.Print($"Data file {path} not found");
                return pathData;
            }

            T data = null;

            using(var fh = File.OpenText(path))
            {
                var fileConents = fh.ReadToEnd();
                data = JsonHandler.Deserialize<T>(fileConents);
            }

            return data;
        }

        public static T Reload()
        {
            _instance = new Lazy<T>(Get);
            return Instance;
        }
    }
}