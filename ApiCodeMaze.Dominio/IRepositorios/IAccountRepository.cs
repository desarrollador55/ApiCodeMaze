using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCodeMaze.Dominio.Repositorios
{
    public interface IAccountRepository : IRepository<Account>
    {
        PagedList<Account> GetAccountsByOwner(Guid ownerId, AccountParameters parameters);
        Account GetAccountByOwner(Guid ownerId, Guid id);
        void Save();
    }
}
