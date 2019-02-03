using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace brid_emitter.Connectors
{
    public interface IBridEmitterConnector
    {
        Task<bool?> GetIsUnique(string uuid);
    }
    
    public class BridEmitterConnector : IBridEmitterConnector
    {
        private static string _emitterUrl = null;
        private static string IsUniquePath(string uuid) 
            => _emitterUrl + $"/{uuid}/unique";

        public static void Initialize(string url)
        {
            _emitterUrl = url;
        }

        public async Task<bool?> GetIsUnique(string uuid)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(IsUniquePath(uuid));
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using(HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                var parsed = bool.TryParse(await reader.ReadToEndAsync(), out var result);
                return parsed ? result : (bool?) null;
            }
        }
    }
}