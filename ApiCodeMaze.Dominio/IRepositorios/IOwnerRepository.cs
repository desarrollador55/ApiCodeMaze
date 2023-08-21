using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Entidades.Helpers;

namespace ApiCodeMaze.Dominio.Repositorios
{
    public interface IOwnerRepository
    {
        PagedList<Owner> GetOwners(OwnerParameters ownerParameters);
        Owner GetOwnerById(string ownerId);
        void CreateOwner(Owner owner);
        void Save();
        void UpdateOwner(Owner dbOwner, Owner owner);
        void DeleteOwner(Owner owner);
    }
}
