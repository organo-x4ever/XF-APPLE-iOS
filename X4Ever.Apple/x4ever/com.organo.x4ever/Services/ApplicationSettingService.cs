using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Models;
using com.organo.x4ever.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly:Dependency(typeof(ApplicationSettingService))]

namespace com.organo.x4ever.Services
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        public string ControllerName => "applicationsettings";

        public async Task<ApplicationSetting> GetAsync()
        {
            var model = new ApplicationSetting();
            var response = await ClientService.SendAsync(HttpMethod.Get, ControllerName, "getasync");
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                var jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                model = JsonConvert.DeserializeObject<ApplicationSetting>(jsonTask.Result);
            }

            return model;
        }
    }
}