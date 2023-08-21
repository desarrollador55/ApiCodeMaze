using ApiCodeMaze.Aplicacion.Servicios;
using ApiCodeMaze.Dominio.Entidades.Extensions;
using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCodeMaze.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/owners/{ownerId}/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAccountsForOwner(Guid ownerId, [FromQuery] AccountParameters parameters)
        {
            var accounts = _accountService.ConsultarAccountsByOwnerId(ownerId, parameters);

            var metadata = new
            {
                accounts.TotalCount,
                accounts.PageSize,
                accounts.CurrentPage,
                accounts.TotalPages,
                accounts.HasNext,
                accounts.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public IActionResult GetAccountForOwner(Guid ownerId, Guid id)
        {
            var account = _accountService.ConsultarAccountByOwnerId(ownerId, id);

            if (account.IsEmptyObject())
            {
                return NotFound();
            }

            return Ok(account);
        }
    }
}
