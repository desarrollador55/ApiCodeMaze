using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Entidades.Extensions;
using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Repositorios;

namespace ApiCodeMaze.Aplicacion.Servicios
{
    public class OwnerService
    {
        private IOwnerRepository _repo;
        public OwnerService(IOwnerRepository repo) => _repo = repo;

        public PagedList<Owner> ConsultarOwners (OwnerParameters ownerParameters)
        {
            return _repo.GetOwners (ownerParameters);
        }

        public Owner ConsultarOwnerById(string id)
        {
            return _repo.GetOwnerById(id);
        }

        public void AlmacenarOwner(Owner owner)
        {
            _repo.CreateOwner(owner);
            _repo.Save();
        }

        public void ActualizarOwner(string id, Owner owner)
        {
            var dbOwner = _repo.GetOwnerById(id);
            if (dbOwner.IsEmptyObject())
            {
                throw new Exception("No existe el objeto");
            }

            _repo.UpdateOwner(dbOwner, owner);
            _repo.Save();
        }

        public void EliminarOwner(string id)
        {
            var owner = _repo.GetOwnerById(id);
            if (owner.IsEmptyObject())
            {
                throw new Exception("No existe el objeto");
            }

            _repo.DeleteOwner(owner);
            _repo.Save();
        }

    }
}
