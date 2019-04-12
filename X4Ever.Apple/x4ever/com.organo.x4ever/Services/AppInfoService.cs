using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Models;
using com.organo.x4ever.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly:Dependency(typeof(AppInfoService))]

namespace com.organo.x4ever.Services
{

    public class AppInfoService : IAppInfoService
    {
        public string ControllerName => "appinfo";
        IDeviceInfo DeviceInfo = DependencyService.Get<IDeviceInfo>();

        public async Task<AppInfoModel> GetAsync()
        {
            var platform = DeviceInfo.GetPlatform;
            var response =
                await ClientService.SendAsync(HttpMethod.Get, ControllerName, $"get?platform={platform}");
            if (response != null && response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
                Task<string> jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                var model = JsonConvert.DeserializeObject<AppInfoModel>(jsonTask.Result);
                return model;
            }

            return null;
        }
    }
}