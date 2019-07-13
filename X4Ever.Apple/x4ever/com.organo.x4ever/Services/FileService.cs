﻿using com.organo.x4ever.Services;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using com.organo.x4ever.Extensions;

[assembly: Dependency(typeof(FileService))]

namespace com.organo.x4ever.Services
{
    public class FileService : IFileService
    {
        public string ControllerName => "files";

        public async Task<string> GetFileAsync(string fileIdentity)
        {
            //param1
            var model = "";
            var response = await ClientService.GetDataAsync(ControllerName, "get?param1=" + fileIdentity);
            if (response != null)
            {
                var jsonTask = response.Content.ReadAsStringAsync();
                jsonTask.Wait();
                model = JsonConvert.DeserializeObject<string>(jsonTask.Result);
            }

            return model;
        }

        public async Task<string> UploadFileAsync(MediaFile _mediaFile)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");
                var uploadAddress = App.Configuration.AppConfig.BaseUrl + "api/" + ControllerName + "/upload";
                var httpResponseMessage = await ClientService.PostAsync(uploadAddress, content);
                if (httpResponseMessage != null)
                {
                    var message = await httpResponseMessage.Content.ReadAsStringAsync();
                    return message;
                }
            }
            catch (Exception)
            {
                return TextResources.MessageErrorOccurred;
            }

            return TextResources.MessageFileUploadFailed;
        }

        public async Task<HttpResponseMessage> UploadFileResponseAsync(MediaFile _mediaFile)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path.Clean()}\"");
            var uploadAddress = App.Configuration.AppConfig.BaseUrl + "api/" + ControllerName + "/uploadasync";
            return await ClientService.PostAsync(uploadAddress, content);
        }
    }
}