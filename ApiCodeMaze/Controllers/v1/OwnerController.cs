using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using ApiCodeMaze.Dominio.Entidades.Extensions;
using Newtonsoft.Json;
using ApiCodeMaze.Aplicacion.Servicios;

namespace ApiCodeMaze.Controllers.v1
{
    [ApiVersion("1.0", Deprecated = false)]
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
            return Ok(owners);
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
