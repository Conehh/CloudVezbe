using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface ITransaction
    {
        Task<bool> Prepare();

        Task Commit();

        Task Rollback();

    }
}
