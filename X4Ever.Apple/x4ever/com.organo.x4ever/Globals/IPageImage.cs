using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Models;
using Xamarin.Forms;

namespace com.organo.x4ever.Globals
{
    public interface IPageImage
    {
        List<ImageSize> GetImageSizes();
    }
}