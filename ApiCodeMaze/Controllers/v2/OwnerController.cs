using ApiCodeMaze.Aplicacion.Servicios;
using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiCodeMaze.Dominio.Entidades.Extensions;

namespace ApiCodeMaze.Controllers.v2
{
    [ApiVersion("2.0")]
    [ApiVersion("2.1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private OwnerService _ownerService;

        public OwnerController(OwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetOwners([FromQuery] OwnerParameters ownerParameters)
        {
            if (!ownerParameters.ValidYearRange)
            {
                return BadRequest("Max year of birth cannot be less than min year of birth");
            }

            var owners = _ownerService.ConsultarOwners(ownerParameters);

            var metadata = new
            {
                owners.TotalCount,
                owners.PageSize,
                owners.CurrentPage,
                owners.TotalPages,
                owners.HasNext,
                owners.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok("Hola mundo V2");
        }

        [HttpGet]
        [MapToApiVersion("2.1")]
        public IActionResult GetOwnersV3([FromQuery] OwnerParameters ownerParameters)
        {
            if (!ownerParameters.ValidYearRange)
            {
                return BadRequest("Max year of birth cannot be less than min year of birth");
            }

            var owners = _ownerService.ConsultarOwners(ownerParameters);

            var metadata = new
            {
                owners.TotalCount,
                owners.PageSize,
                owners.CurrentPage,
                owners.TotalPages,
                owners.HasNext,
                owners.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok("Hola mundo V2.1");
        }

        [HttpGet("{id}", Name = "OwnerById")]
        public IActionResult GetOwnerById(string id)
        {
            var owner = _ownerService.ConsultarOwnerById(id);

            if (owner.IsEmptyObject())
            {
                return NotFound();
            }
            else
            {
                return Ok(owner);
            }
        }

        [HttpPost]
        public IActionResult CreateOwner([FromBody] Owner owner)
        {
            if (owner.IsObjectNull())
            {
                return BadRequest("Owner object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            _ownerService.AlmacenarOwner(owner);

            return CreatedAtRoute("OwnerById", new { id = owner.Id }, owner);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOwner(string id, [FromBody] Owner owner)
        {
            if (owner.IsObjectNull())
            {
                return BadRequest("Owner object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            _ownerService.ActualizarOwner(id, owner);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(string id)
        {
            _ownerService.EliminarOwner(id);
            return Ok();
        }
    }
}
