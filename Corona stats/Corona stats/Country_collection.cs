namespace Corona_stats
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Country_collection
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("cases")]
        public long Cases { get; set; }

        [JsonProperty("todayCases")]
        public long TodayCases { get; set; }

        [JsonProperty("deaths")]
        public long Deaths { get; set; }

        [JsonProperty("todayDeaths")]
        public long TodayDeaths { get; set; }

        [JsonProperty("recovered")]
        public long Recovered { get; set; }

        [JsonProperty("active")]
        public long Active { get; set; }

        [JsonProperty("critical")]
        public long Critical { get; set; }

        [JsonProperty("casesPerOneMillion")]
        public long CasesPerOneMillion { get; set; }
    }

    public partial class Country_collection
    {
        public static Country_collection[] FromJson(string json) => JsonConvert.DeserializeObject<Country_collection[]>(json, Corona_stats.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Country_collection[] self) => JsonConvert.SerializeObject(self, Corona_stats.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

   
    
}
