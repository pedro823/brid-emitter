using System;
using System.Threading.Tasks;
using brid_emitter.Connectors;
using Microsoft.AspNetCore.Mvc;

namespace brid_emitter.Util
{
    public interface IIdGenerator
    {
        string GenerateUuid();
        Task<bool> IsUnique(string uuid);
    }

    public class IdGenerator : IIdGenerator
    {
        private IBridEmitterConnector _bridConnector;
        private const string Alphabet = "0123456789";

        public IdGenerator(IBridEmitterConnector bridConnector)
        {
            _bridConnector = bridConnector;
        }

        public string GenerateUuid()
        {
            const int length = 14;
            var uuidArray = new char[length];
            var rng = new Random();

            for (var i = 0; i < length; i++)
            {
                uuidArray[i] = i % 3 == 0 && i != 0 
                    ? '.' 
                    : Alphabet[rng.Next(Alphabet.Length)];
            }

            return new string(uuidArray);
        }

        public Task<bool> IsUnique(string uuid)
        {
            return Task.FromResult(true);
        }
    }
}