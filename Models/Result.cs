    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


namespace MOTINFO.Models
{

    public partial class Car
    {
        [JsonProperty("registration")]
        public string Registration { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("firstUsedDate")]
        public string FirstUsedDate { get; set; }

        [JsonProperty("fuelType")]
        public string FuelType { get; set; }

        [JsonProperty("primaryColour")]
        public string PrimaryColour { get; set; }

        [JsonProperty("motTests")]
        public MotTest[] MotTests { get; set; }
    }

    public partial class MotTest
    {
        [JsonProperty("completedDate")]
        public string CompletedDate { get; set; }

        [JsonProperty("testResult")]
        public TestResult TestResult { get; set; }

        [JsonProperty("expiryDate", NullValueHandling = NullValueHandling.Ignore)]
        public string ExpiryDate { get; set; }

        [JsonProperty("odometerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long OdometerValue { get; set; }

        [JsonProperty("odometerUnit")]
        public OdometerUnit OdometerUnit { get; set; }

        [JsonProperty("motTestNumber")]
        public string MotTestNumber { get; set; }

        [JsonProperty("rfrAndComments")]
        public RfrAndComment[] RfrAndComments { get; set; }
    }

    public partial class RfrAndComment
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public enum OdometerUnit { Mi };

    public enum TestResult { Failed, Passed };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                OdometerUnitConverter.Singleton,
                TestResultConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class OdometerUnitConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(OdometerUnit) || t == typeof(OdometerUnit?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "mi")
            {
                return OdometerUnit.Mi;
            }
            throw new Exception("Cannot unmarshal type OdometerUnit");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (OdometerUnit)untypedValue;
            if (value == OdometerUnit.Mi)
            {
                serializer.Serialize(writer, "mi");
                return;
            }
            throw new Exception("Cannot marshal type OdometerUnit");
        }

        public static readonly OdometerUnitConverter Singleton = new OdometerUnitConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class TestResultConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TestResult) || t == typeof(TestResult?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "FAILED":
                    return TestResult.Failed;
                case "PASSED":
                    return TestResult.Passed;
            }
            throw new Exception("Cannot unmarshal type TestResult");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TestResult)untypedValue;
            switch (value)
            {
                case TestResult.Failed:
                    serializer.Serialize(writer, "FAILED");
                    return;
                case TestResult.Passed:
                    serializer.Serialize(writer, "PASSED");
                    return;
            }
            throw new Exception("Cannot marshal type TestResult");
        }

        public static readonly TestResultConverter Singleton = new TestResultConverter();
    }
}
