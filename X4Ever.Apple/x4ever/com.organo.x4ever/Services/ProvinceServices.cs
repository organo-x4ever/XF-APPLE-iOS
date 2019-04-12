using com.organo.x4ever.Models;
using com.organo.x4ever.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ProvinceServices))]

namespace com.organo.x4ever.Services
{
    internal class ProvinceServices : IProvinceServices
    {
        public string ControllerName => "provinces";

        public async Task<List<CountryProvince>> GetAsync()
        {
            var model = new List<CountryProvince>();
            var response = await ClientService.GetByApplicationHeaderDataAsync(ControllerName, "get");
            if (response != null)
            {
                var jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                model = JsonConvert.DeserializeObject<List<CountryProvince>>(jsonTask.Result);
                return model;
            }

            return null;
        }
    }
}