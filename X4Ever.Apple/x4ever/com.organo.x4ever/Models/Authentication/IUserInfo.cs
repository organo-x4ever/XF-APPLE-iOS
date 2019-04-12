﻿using System;

namespace com.organo.x4ever.Models.Authentication
{
    public interface IUserInfo
    {
        long ID { get; set; }
        string UserLogin { get; set; }
        string UserFirstName { get; set; }
        string UserLastName { get; set; }
        string UserEmail { get; set; }
        string ProfileImage { get; set; }
        DateTime UserRegistered { get; set; }
        string DisplayName { get; }
        string FullName { get; }
        bool IsMetaExists { get; set; }
        bool IsTrackerExists { get; set; }
    }
}