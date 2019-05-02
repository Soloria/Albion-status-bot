namespace AlbionStatusBot
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    public static class Config
    {
        public static IConfigurationRoot root;

        public static void StartUp()
        {
            if (root != null) return;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Assembly.GetExecutingAssembly().Location
                    .Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), ""))
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            root = builder.Build();


        }

        public static T GetValue<T>(string key, T defaultValue = default)
        {
            return root?[key] == null ? defaultValue : ProcessTypes<T>(root[key]);
        }

        internal static T ProcessTypes<T>(string value)
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(Guid):
                    return (T)(Guid.Parse(value) as object);
                case Type t when t == typeof(bool):
                    return (T)(bool.Parse(value) as object);
                case Type t when t == typeof(bool?):
                    return (T)(new bool?(bool.Parse(value)) as object);
                case Type t when t == typeof(DateTime):
                    return (T)(DateTime.Parse(value) as object);
                case Type t when t == typeof(long):
                    return (T)(long.Parse(value) as object);
                case Type t when t == typeof(short):
                    return (T)(short.Parse(value) as object);
                case Type t when t == typeof(int):
                    return (T)(int.Parse(value) as object);
                case Type t when t == typeof(float):
                    return (T)(float.Parse(value) as object);
                case Type t when t == typeof(double):
                    return (T)(double.Parse(value) as object);
                case Type t when t == typeof(decimal):
                    return (T)(decimal.Parse(value) as object);
                case Type t when t == typeof(string):
                    return (T)(object)value;
                case Type t when t == typeof(Uri):
                    return (T)(new Uri(value) as object);
            }
            throw new Exception("something going wrong lol");
        }
    }
}
