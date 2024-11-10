using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface ITransaction
    {
        Task<bool> Prepare(object value);

        Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction);

        Task Rollback(Microsoft.ServiceFabric.Data.ITransaction transaction);

    }
}
