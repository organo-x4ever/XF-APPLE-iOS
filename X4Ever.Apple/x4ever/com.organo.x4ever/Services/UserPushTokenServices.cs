
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models.Notifications;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserPushTokenServices))]

namespace com.organo.x4ever.Services
{
    public class UserPushTokenServices : IUserPushTokenServices
    {
        public string ControllerName => "pushnotifications";
        public string ControllerName_UnAuthorized => "actions";
        public async Task<UserPushTokenModel> Get()
        {
            var model = new UserPushTokenModel();
            var response = await ClientService.GetDataAsync(ControllerName, "get");
            if (response != null)
            {
                var jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                model = JsonConvert.DeserializeObject<UserPushTokenModel>(jsonTask.Result);
            }

            return model;
        }

        public async Task<string> Insert(UserPushTokenModel model)
        {
            try
            {
                var response = await ClientService.PostDataAsync(model, ControllerName, "post");
                if (response != null)
                {
                    Task<string> jsonTask = response.Content.ReadAsStringAsync();
                    if (jsonTask.Result.Contains(HttpConstants.SUCCESS))
                        return HttpConstants.SUCCESS;
                    else if (response.ToString().Contains(HttpConstants.UNAUTHORIZED))
                        return response.ToString();
                    return jsonTask.Result;
                }
                else return TextResources.MessageSomethingWentWrong;
            }
            catch (Exception)
            {
                return TextResources.MessageSomethingWentWrong;
            }
        }

        public async Task<string> SaveDeviceToken()
        {
            var deviceToken = DependencyService.Get<ISecureStorage>()
                .RetrieveStringFromBytes(Keys.DEVICE_TOKEN_IDENTITY);
            if (!string.IsNullOrEmpty(deviceToken))
            {
                return await Insert(new UserPushTokenModel()
                {
                    DeviceToken = deviceToken,
                    IssuedOn = DateTime.Now,
                    DeviceIdentity = string.Format(TextResources.AppVersion,
                        App.Configuration.AppConfig.ApplicationVersion),
                    DeviceIdiom = Device.Idiom.ToString(),
                });
            }

            return "";
        }
        
        public async Task<string> SaveDeviceTokenUnauthorized()
        {
            var deviceToken = DependencyService.Get<ISecureStorage>().RetrieveStringFromBytes(Keys.DEVICE_TOKEN_IDENTITY);
            if (deviceToken != null)
            {
                if (!string.IsNullOrEmpty(deviceToken))
                {
                    return await InsertUnauthorized(new UserPushTokenModel()
                    {
                        DeviceToken = deviceToken,
                        IssuedOn = DateTime.Now,
                        DeviceIdentity = string.Format(TextResources.AppVersion,
                            App.Configuration.AppConfig.ApplicationVersion),
                        DeviceIdiom = Device.Idiom.ToString(),
                        UserKey = App.Configuration?.GetUserKey()
                    });
                }
            }

            return "";

            async Task<string> InsertUnauthorized(UserPushTokenModel model)
            {
                try
                {
                    var response =
                        await ClientService.PostDataNoHeaderAsync(model, ControllerName_UnAuthorized,
                            "postnotification");
                    if (response != null && response.IsSuccessStatusCode &&
                        response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var jsonTask = response.Content.ReadAsStringAsync();
                        return jsonTask.Result.Contains(HttpConstants.SUCCESS)
                            ? HttpConstants.SUCCESS
                            : jsonTask.Result;
                    }
                    else return TextResources.MessageSomethingWentWrong;
                }
                catch (Exception)
                {
                    return TextResources.MessageSomethingWentWrong;
                }
            }
        }
    }
}