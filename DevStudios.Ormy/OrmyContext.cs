using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevStudios.Ormy
{
    public class OrmyContext
    {
        private readonly DbConnection _connection;
        public OrmyContext(DbConnection connection)
        {
            _connection = connection;
        }
    }
}
