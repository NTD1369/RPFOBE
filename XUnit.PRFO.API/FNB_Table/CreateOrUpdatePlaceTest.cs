using RPFO.Data.Entities;
using System;
using Xunit;
using XUnit.PRFO.API;

namespace Test.PRFO.API.FNB_Table
{
    public class CreateOrUpdatePlaceTest : IClassFixture<DependencyInjectionFixture>
    {
        private readonly string CompanyCode = "CP001 ";
        private readonly string StoreId = "FM01 ";
        private readonly string PlaceName = "Area0001 ";


        protected DependencyInjectionFixture _fixture { get; set; }

        public CreateOrUpdatePlaceTest(DependencyInjectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void RunAllPlaceAnyc()
        {
            try
            {
                var alltables = await _fixture.PlaceInforService.GetAll(CompanyCode, StoreId, string.Empty);
            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

        [Fact]
        public async void RunGetPlaceInfoAnyc()
        {
            try
            {
                var placeInfo = await _fixture.PlaceInforService.GetMPlaceInfor(CompanyCode, StoreId, PlaceName);
            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

        [Fact]
        public async void CreatePlaceTestAsync()
        {
            var table = new MPlaceInfor
            {
                CompanyCode = CompanyCode,
                StoreId = StoreId,
                PlaceName = PlaceName,
                Remark = CompanyCode + StoreId + PlaceName,
                Description = CompanyCode + StoreId + PlaceName,
                Slot = 6,
                Status = "A",
                Width = "200x300"
            };

            if (table != null)
            {
                try
                {
                    await _fixture.PlaceInforService.Create(table);
                }
                catch (System.Exception ex)
                {
                    Assert.IsType<DivideByZeroException>(ex.Message);
                }
            }
        }

        [Fact]
        public async void UpdatePlaceTestAsync()
        {
            try
            {
                var placeInfo = await _fixture.PlaceInforService.GetMPlaceInfor(CompanyCode, StoreId, PlaceName);
                if (placeInfo != null)
                {
                    placeInfo.Slot = 4;
                    placeInfo.Width = "300x400";

                    await _fixture.PlaceInforService.Update(placeInfo);
                }

            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

        [Fact]
        public async void DeletePlaceAync()
        {
            try
            {
                var mTableInfor = await _fixture.PlaceInforService.GetMPlaceInfor(CompanyCode, StoreId, PlaceName);
                if (mTableInfor != null)
                    await _fixture.PlaceInforService.Delete(mTableInfor);

            }
            catch (System.Exception ex)
            {
                Assert.IsType<DivideByZeroException>(ex.Message);
            }
        }

    }
}
