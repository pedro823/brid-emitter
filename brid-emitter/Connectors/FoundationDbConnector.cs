using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using brid_emitter.Contracts;
using FoundationDB.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace brid_emitter.Connectors
{
    public class FoundationDbConnector
    {
        private static IFdbDatabase _connection;

        public static void Initialize(IFdbDatabase connection)
        {
            _connection = connection;
        }

        public static async Task Set<T>(string key, T value, CancellationToken ct)
        {
            using (var transaction = _connection.BeginTransaction(ct))
            {
                await transaction.SetAsync(
                    _connection.Keys.Encode(key),
                    Encoder.Encode(value)
                );

                await transaction.CommitAsync();
            }
        }

        public static async Task<CacheResult<T>> Get<T>(string key, CancellationToken ct)
        {
            using (var transaction = _connection.BeginReadOnlyTransaction(ct))
            {
                var value = await transaction.GetAsync(_connection.Keys.Encode(key));
                return value.HasValue ? new CacheResult<T>(Encoder.Decode<T>(value)) : null;
            }
        }

        public class CacheResult<T>
        {
            public T Data { get; set; }

            public CacheResult(T data)
            {
                Data = data;
            }
        }
    }

    public static class Encoder
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static UnicodeEncoding _unicode = new UnicodeEncoding();
        
        public static MemoryStream Encode<T>(T value)
        {
            var serializedValue = _unicode.GetBytes(JsonConvert.SerializeObject(value));
            return new MemoryStream(serializedValue);
        }

        public static T Decode<T>(Slice value)
        {
            var deserializedValue = value.ToUnicode();
            return JsonConvert.DeserializeObject<T>(deserializedValue);
        }
    }
}