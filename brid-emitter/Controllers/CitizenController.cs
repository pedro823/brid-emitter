using System.Threading;
using System.Threading.Tasks;
using brid_emitter.Connectors;
using brid_emitter.Contracts;
using brid_emitter.Util;
using Microsoft.AspNetCore.Mvc;

namespace brid_emitter.Controllers
{
    public class CitizenController : Controller
    {
        private IIdGenerator _idGenerator;

        public CitizenController(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        [HttpGet("/id/{id}")]
        public async Task<IActionResult> FetchCitizen(CancellationToken cancellationToken, string id = null)
        {
            if (id == null)
            {
                return BadRequest("Must include id");
            }

            var citizen = await FoundationDbConnector.Get<Citizen>(id, cancellationToken);
            return Ok(citizen);
        }

        [HttpPost("/id")]
        public async Task<IActionResult> CreateCitizen([FromBody] Citizen citizen, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(citizen.Uuid))
            {
                do
                {
                    citizen.Uuid = _idGenerator.GenerateUuid();
                } while (!await _idGenerator.IsUnique(citizen.Uuid));
            }

            await FoundationDbConnector.Set(citizen.Uuid, citizen, cancellationToken);
            return Ok(new CreateCitizenResponse(citizen.Uuid));
        }
    }

    public class CreateCitizenResponse
    {
        public string Uuid;
        public CreateCitizenResponse(string citizenUuid)
        {
            Uuid = citizenUuid;
        }
    }
}