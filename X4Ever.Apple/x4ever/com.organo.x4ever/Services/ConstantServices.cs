
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using com.organo.x4ever.Services;

[assembly:Dependency(typeof(ConstantServices))]

namespace com.organo.x4ever.Services
{
    public class ConstantServices : IConstantServices
    {
        public async Task<string> Blogs() =>
            await ClientService.GetStringAsync(new Uri(ClientService.GetRequestUri("constants", "blogs")));
    }
}