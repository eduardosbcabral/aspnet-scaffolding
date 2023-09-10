using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scaffolding.Utilities.Converters;

namespace Scaffolding.Utilities;

public class JsonUtility
{
    private static readonly object Lock = new object();

    public static List<JsonConverter> DefaultConverters = new List<JsonConverter>
    {
        new EnumWithContractJsonConverter(),
        new IsoDateTimeConverter
        {
          DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffff"
        }
    };

    private static JsonSerializerSettings _snakeCaseJsonSerializerSettings;

    public static JsonSerializerSettings _camelCaseJsonSerializerSettings;

    private static JsonSerializerSettings _lowerCaseJsonSerializerSettings;

    private static JsonSerializerSettings _originalCaseJsonSerializerSettings;

    private static JsonSerializer _camelCaseJsonSerializer;

    private static JsonSerializer _snakeCaseJsonSerializer;

    private static JsonSerializer _lowerCaseJsonSerializer;

    private static JsonSerializer _originalCaseJsonSerializer;

    public static JsonSerializerSettings SnakeCaseJsonSerializerSettings
    {
        get
        {
            if (_snakeCaseJsonSerializerSettings == null)
            {
                lock (Lock)
                {
                    if (_snakeCaseJsonSerializerSettings == null)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.ContractResolver = new SnakeCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            settings.Converters.Add(c);
                        });
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        _snakeCaseJsonSerializerSettings = settings;
                    }
                }
            }

            return _snakeCaseJsonSerializerSettings;
        }
    }

    public static JsonSerializerSettings CamelCaseJsonSerializerSettings
    {
        get
        {
            if (_camelCaseJsonSerializerSettings == null)
            {
                lock (Lock)
                {
                    if (_camelCaseJsonSerializerSettings == null)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.ContractResolver = new CustomCamelCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            settings.Converters.Add(c);
                        });
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        _camelCaseJsonSerializerSettings = settings;
                    }
                }
            }

            return _camelCaseJsonSerializerSettings;
        }
    }

    public static JsonSerializerSettings LowerCaseJsonSerializerSettings
    {
        get
        {
            if (_lowerCaseJsonSerializerSettings == null)
            {
                lock (Lock)
                {
                    if (_lowerCaseJsonSerializerSettings == null)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.ContractResolver = new LowerCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            settings.Converters.Add(c);
                        });
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        _lowerCaseJsonSerializerSettings = settings;
                    }
                }
            }

            return _lowerCaseJsonSerializerSettings;
        }
    }

    public static JsonSerializerSettings OriginalCaseJsonSerializerSettings
    {
        get
        {
            if (_originalCaseJsonSerializerSettings == null)
            {
                lock (Lock)
                {
                    if (_originalCaseJsonSerializerSettings == null)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.ContractResolver = new OriginalCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            settings.Converters.Add(c);
                        });
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        _originalCaseJsonSerializerSettings = settings;
                    }
                }
            }

            return _originalCaseJsonSerializerSettings;
        }
    }

    public static JsonSerializer CamelCaseJsonSerializer
    {
        get
        {
            if (_camelCaseJsonSerializer == null)
            {
                lock (Lock)
                {
                    if (_camelCaseJsonSerializer == null)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.NullValueHandling = NullValueHandling.Ignore;
                        serializer.ContractResolver = new CustomCamelCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            serializer.Converters.Add(c);
                        });
                        _camelCaseJsonSerializer = serializer;
                    }
                }
            }

            return _camelCaseJsonSerializer;
        }
    }

    public static JsonSerializer SnakeCaseJsonSerializer
    {
        get
        {
            if (_snakeCaseJsonSerializer == null)
            {
                lock (Lock)
                {
                    if (_snakeCaseJsonSerializer == null)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.NullValueHandling = NullValueHandling.Ignore;
                        serializer.ContractResolver = new SnakeCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            serializer.Converters.Add(c);
                        });
                        _snakeCaseJsonSerializer = serializer;
                    }
                }
            }

            return _snakeCaseJsonSerializer;
        }
    }

    public static JsonSerializer LowerCaseJsonSerializer
    {
        get
        {
            if (_lowerCaseJsonSerializer == null)
            {
                lock (Lock)
                {
                    if (_lowerCaseJsonSerializer == null)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.NullValueHandling = NullValueHandling.Ignore;
                        serializer.ContractResolver = new LowerCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            serializer.Converters.Add(c);
                        });
                        _lowerCaseJsonSerializer = serializer;
                    }
                }
            }

            return _lowerCaseJsonSerializer;
        }
    }

    public static JsonSerializer OriginalCaseJsonSerializer
    {
        get
        {
            if (_originalCaseJsonSerializer == null)
            {
                lock (Lock)
                {
                    if (_originalCaseJsonSerializer == null)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.NullValueHandling = NullValueHandling.Ignore;
                        serializer.ContractResolver = new OriginalCasePropertyNamesContractResolver();
                        DefaultConverters.ForEach(delegate (JsonConverter c)
                        {
                            serializer.Converters.Add(c);
                        });
                        _originalCaseJsonSerializer = serializer;
                    }
                }
            }

            return _originalCaseJsonSerializer;
        }
    }

    /// <summary>
    /// Resolve property names to lowercase only
    /// </summary>
    public class LowerCaseNamingResolver : NamingStrategy
    {
        public LowerCaseNamingResolver()
        {
            this.ProcessDictionaryKeys = true;
            this.OverrideSpecifiedNames = true;
        }

        protected override string ResolvePropertyName(string name)
        {
            return name.ToLowerInvariant();
        }
    }

    /// <summary>
    /// Lowercase contract resolver
    /// </summary>
    public class LowerCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public LowerCasePropertyNamesContractResolver()
        {
            this.NamingStrategy = new LowerCaseNamingResolver
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }
    }

    /// <summary>
    /// Resolve property names original name
    /// </summary>
    public class OriginalCaseNamingResolver : NamingStrategy
    {
        public OriginalCaseNamingResolver()
        {
            this.ProcessDictionaryKeys = true;
            this.OverrideSpecifiedNames = true;
        }

        protected override string ResolvePropertyName(string name)
        {
            return name;
        }
    }

    /// <summary>
    /// Original contract resolver
    /// </summary>
    public class OriginalCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public OriginalCasePropertyNamesContractResolver()
        {
            this.NamingStrategy = new OriginalCaseNamingResolver
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }
    }

    /// <summary>
    /// Snake case contract resolver
    /// </summary>
    public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public SnakeCasePropertyNamesContractResolver()
        {
            this.NamingStrategy = new SnakeCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }
    }

    /// <summary>
    /// Camel case contract resolver
    /// </summary>
    public class CustomCamelCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public CustomCamelCasePropertyNamesContractResolver()
        {
            this.NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }
    }
}
