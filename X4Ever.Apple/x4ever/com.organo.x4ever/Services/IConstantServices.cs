
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IConstantServices
    {
        Task<string> Blogs();
        Task<string> MoreWebLinks();
        Task<string> WeightLoseWarningPercentile();
    }
}