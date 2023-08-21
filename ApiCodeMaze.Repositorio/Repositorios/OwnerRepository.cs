using ApiCodeMaze.Dominio.Entidades.Helpers;
using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Repositorios;
using ApiCodeMaze.Repositorio.Contexts;
using ApiCodeMaze.Dominio.Entidades.Extensions;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
namespace ApiCodeMaze.Repositorio.Repositorios
{
    public class OwnerRepository : Repository<Owner>, IOwnerRepository
    {
        public OwnerRepository(Context context): base(context){}

        public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
        {
            var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                        o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

            SearchByName(ref owners, ownerParameters.Name);

            ApplySort(ref owners, ownerParameters.OrderBy);

            return PagedList<Owner>.ToPagedList(owners.OrderBy(on => on.Name),
                ownerParameters.PageNumber,
                ownerParameters.PageSize);
        }

        private void ApplySort(ref IQueryable<Owner> owners, string orderByQueryString)
        {
            if (!owners.Any())
                return;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                owners = owners.OrderBy(x => x.Name);
                return;
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Owner).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                owners = owners.OrderBy(x => x.Name);
                return;
            }
            owners = owners.OrderBy(orderQuery);
        }

        private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
        {
            if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
                return;

            owners = owners.Where(o => o.Name.ToLower().Contains(ownerName.Trim().ToLower()));
        }

        public Owner GetOwnerById(string ownerId)
        {
            return FindByCondition(owner => owner.Id.Equals(ownerId))
                .DefaultIfEmpty(new Owner())
                .FirstOrDefault();
        }

        public void CreateOwner(Owner owner)
        {
            Create(owner);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateOwner(Owner dbOwner, Owner owner)
        {
            dbOwner.Map(owner);
            Update(dbOwner);
        }

        public void DeleteOwner(Owner owner)
        {
            Delete(owner);
        }
    }
}
