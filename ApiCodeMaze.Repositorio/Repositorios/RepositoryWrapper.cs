using ApiCodeMaze.Dominio.Repositorios;
using ApiCodeMaze.Repositorio.Contexts;

namespace ApiCodeMaze.Repositorio.Repositorios
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private Context _context;
        private IOwnerRepository _owner;
        private IAccountRepository _account;

        public IOwnerRepository Owner
        {
            get
            {
                if (_owner == null)
                {
                    _owner = new OwnerRepository(_context);
                }

                return _owner;
            }
        }

        public IAccountRepository Account
        {
            get
            {
                if (_account == null)
                {
                    _account = new AccountRepository(_context);
                }

                return _account;
            }
        }

        public RepositoryWrapper(Context context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
