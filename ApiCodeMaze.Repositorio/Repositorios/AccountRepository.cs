using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Repositorios;
using ApiCodeMaze.Repositorio.Contexts;

namespace ApiCodeMaze.Repositorio.Repositorios
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(Context context): base(context){}

        public PagedList<Account> GetAccountsByOwner(Guid ownerId, AccountParameters parameters)
        {
            var accounts = FindByCondition(a => a.OwnerId.Equals(ownerId));

            return PagedList<Account>.ToPagedList(accounts,
                parameters.PageNumber,
                parameters.PageSize);
        }

        public Account GetAccountByOwner(Guid ownerId, Guid id)
        {
            return FindByCondition(a => a.OwnerId.Equals(ownerId) && a.Id.Equals(id)).SingleOrDefault();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
