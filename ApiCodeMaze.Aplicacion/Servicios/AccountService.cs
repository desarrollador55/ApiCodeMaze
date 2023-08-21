using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCodeMaze.Aplicacion.Servicios
{
    public class AccountService
    {
        IAccountRepository _repo;
        public AccountService(IAccountRepository repo) => _repo = repo;

        public PagedList<Account> ConsultarAccountsByOwnerId(Guid ownerId, AccountParameters accountP)
        {
            return _repo.GetAccountsByOwner(ownerId,accountP);
        }

        public Account ConsultarAccountByOwnerId(Guid ownerId, Guid Id)
        {
            return _repo.GetAccountByOwner(ownerId,Id);
        }
    }
}
