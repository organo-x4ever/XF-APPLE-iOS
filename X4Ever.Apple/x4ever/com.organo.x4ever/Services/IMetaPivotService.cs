﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Models.User;

namespace com.organo.x4ever.Services
{
    public interface IMetaPivotService : IBaseService
    {
        string Message { get; set; }
        Task<string> SaveMetaAsync(List<Meta> metas);
        Task<string> SaveMetaAsync(Meta metas);
        Task<bool> SaveMetaStep2Async(List<Meta> metas);
        Task<MetaPivot> GetMetaAsync();
        Task<Meta> AddMetaAsync(string metaValue, string description, string key, string type);
        Meta AddMeta(string metaValue, string description, string key, string type);
    }
}