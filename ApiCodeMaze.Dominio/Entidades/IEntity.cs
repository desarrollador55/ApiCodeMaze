using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCodeMaze.Dominio.Entidades
{
    public class IEntity
    {
        public string Id { get; set; }

        public IEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
