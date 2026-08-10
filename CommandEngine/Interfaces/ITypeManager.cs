using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandEngine.Interfaces
{
    public interface ITypeManager<T>
    {
        public static abstract bool IsLoaded { get; set; }
        public static abstract string DataPath { get; set; }

        public static abstract void LoadTypes();
        public static abstract void SaveDefaultTypes();
        public static abstract List<T> GetDefaultTypes();
    }

    public abstract class TypeManager<TData, TManager> : ITypeManager<TData>
        where TManager : TypeManager<TData, TManager>, new()
    {
        static TManager _instance = new TManager();

        protected static bool _IsLoaded { get; private set; } = false;
        static bool ITypeManager<TData>.IsLoaded
        {
            get => _instance is null ? false : _IsLoaded;
            set => throw new InvalidOperationException("Cannot set IsLoaded externally.");
        }

        protected static string _DataPath { get; private set; } = _instance.GetDataPath();
        static string ITypeManager<TData>.DataPath
        {
            get
            {
                if (_instance is null)
                    return string.Empty;
                else if (string.IsNullOrEmpty(_DataPath))
                    _DataPath = _instance.GetDataPath();
                return _DataPath;
            }
            set => throw new InvalidOperationException("Cannot set DataPath externally.");
        }

        public static void LoadTypes()
        {
            if(!File.Exists(_DataPath))
            {
                ConsoleExt.WriteWarningLine($"Data file not found at {_DataPath}. Regenerating from defaults.");

                List<TData> data = GetDefaultTypes();
                foreach(var item in data)
                    _instance.AddTypeEntry(item);

                SaveDefaultTypes();

                _IsLoaded = true;
                return;
            }

            StreamReader reader = new StreamReader(_DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            if (jArray is null)
            {
                ConsoleExt.WriteErrorLine("Failed to load data types");
                return;
            }

            foreach (var obj in jArray)
            {
                var data = (TData?)serializer.Deserialize(new JTokenReader(obj), typeof(TData));

                if (data is not null)
                    _instance.AddTypeEntry(data);
            }

            _IsLoaded = true;
        }
        public static void SaveDefaultTypes()
        {
            if (!File.Exists(_DataPath))
            {
                string fileName = _DataPath.Split('\\').Last();
                string dir = _DataPath.Substring(0, _DataPath.Length - fileName.Length);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            using StreamWriter writer = new StreamWriter(_DataPath);
            writer.Write(JsonConvert.SerializeObject(GetDefaultTypes()));
            writer.Close();
        }
        public static List<TData> GetDefaultTypes()
        {
            return _instance.GetDefaultTypesInternal();
        }

        protected abstract string GetDataPath();
        protected abstract void AddTypeEntry(TData item);
        protected abstract List<TData> GetDefaultTypesInternal();
    }
}
