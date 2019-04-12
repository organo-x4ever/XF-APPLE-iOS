﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Models.Notifications;

namespace com.organo.x4ever.Services
{
    public interface IUserPushTokenServices : IBaseService
    {
        Task<string> SaveDeviceToken();
        Task<UserPushTokenModel> Get();
        Task<string> Insert(UserPushTokenModel model);
    }
}