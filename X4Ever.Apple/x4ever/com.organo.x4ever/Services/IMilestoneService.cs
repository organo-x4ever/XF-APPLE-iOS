using com.organo.x4ever.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IMilestoneService
    {
        Task<List<Milestone>> GetMilestoneAsync();
    }
}