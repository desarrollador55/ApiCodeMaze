using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCodeMaze.Dominio.Entidades.Helpers
{
    public class AccountParameters : QueryStringParameters
    {
        public AccountParameters()
        {
            OrderBy = "DateCreated";
        }
    }
}
