using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnit.PRFO.API;
using Xunit;

namespace Test.PRFO.API.FNB_Table
{
    public class AssignTableInPlace : IClassFixture<DependencyInjectionFixture>
    {
        private readonly string CompanyCode = "CP001 ";
        private readonly string StoreId = "FM01 ";

        private readonly string TableTest = "TableTest01";
        private readonly string PlaceName = "Area0001 ";

        public AssignTableInPlace()
        {

        }

    }
}
