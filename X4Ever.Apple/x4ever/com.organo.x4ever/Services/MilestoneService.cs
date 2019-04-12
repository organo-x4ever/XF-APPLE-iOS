﻿
using com.organo.x4ever.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.organo.x4ever.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(MilestoneService))]

namespace com.organo.x4ever.Services
{
    public class MilestoneService : IMilestoneService, IBaseService
    {
        public string ControllerName => "milestones";

        public async Task<List<Milestone>> GetMilestoneAsync()
        {
            var model = new List<Milestone>();
            var response = await ClientService.GetDataAsync(ControllerName, "get");
            if (response != null)
            {
                var jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                model = JsonConvert.DeserializeObject<List<Milestone>>(jsonTask.Result);
                return model;
            }
            return null;
        }
    }
}