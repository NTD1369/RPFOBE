using Newtonsoft.Json;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using XUnit.PRFO.API;

namespace Test.PRFO.API.FNB_Table
{
    public class CreateOrUpdateTableTest : IClassFixture<DependencyInjectionFixture>
    {
        private readonly string CompanyCode = "CP001";
        private readonly string StoreId = "FM01";
        private readonly string TableTest = "TableTest01";
        protected DependencyInjectionFixture _fixture { get; set; }

        public CreateOrUpdateTableTest(DependencyInjectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void RunAllTableAnyc()
        {
            try
            {
                var alltables = await _fixture.TableInforService.GetAll(CompanyCode, StoreId, string.Empty);
            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

        [Fact]
        public async void UpdateTableTestAsync()
        {
            try
            {
                var mTableInfor = await _fixture.TableInforService.GetMTableInfor(CompanyCode, StoreId, TableTest);
                if (mTableInfor != null)
                {
                    mTableInfor.Slot = 4;
                    mTableInfor.Width = "300x400";

                    await _fixture.TableInforService.Update(mTableInfor);
                }

            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

        [Fact]
        public async void CreateTableTestAsync()
        {
            var table = new MTableInfor
            {
                CompanyCode = CompanyCode,
                StoreId = StoreId,
                TableName = TableTest,
                Remark = TableTest,
                Description = TableTest,
                Status = "A",
                Width = "200x300"
            };

            if (table != null)
            {
                try
                {
                    await _fixture.TableInforService.Create(table);
                }
                catch (System.Exception ex)
                {
                    Assert.IsType<DivideByZeroException>(ex.Message);
                }
            }
        }

        [Fact]
        public async void DeleteTableAync()
        {
            try
            {
                var mTableInfor = await _fixture.TableInforService.GetMTableInfor(CompanyCode, StoreId, TableTest);
                if (mTableInfor != null)
                    await _fixture.TableInforService.Delete(mTableInfor);

            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }
    }
}
