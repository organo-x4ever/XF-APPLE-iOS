using com.organo.x4ever.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

[assembly:Dependency(typeof(PageImage))]

namespace com.organo.x4ever.Globals
{
    public class PageImage //: IPageImage
    {
        //public List<ImageSize> GetImageSizes()
        //{
        //    var imageSizes = new List<ImageSize>();
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.TOP_BAR_LOGO,
        //        ImageName = TextResources.logo_transparent,
        //        Height = 50,
        //        Width = 50
        //    });
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.TOP_BAR_MENU,
        //        ImageName = TextResources.icon_menu,
        //        Height = 50,
        //        Width = 20
        //    });
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.MAIN_PAGE_LOGO,
        //        ImageName = TextResources.logo,
        //        Height = 400,
        //        Width = 500
        //    });
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.MAIN_PAGE_x4ever_LOGO,
        //        ImageName = TextResources.logo_challenge,
        //        Height = 120,
        //        Width = 800
        //    });
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.MENU_PAGE_USER_IMAGE,
        //        ImageName = null,
        //        Height = 100,
        //        Width = 100,
        //        IsDynamic = true
        //    });
        //    imageSizes.Add(new ImageSize()
        //    {
        //        ImageID = ImageIdentity.USER_BADGE_ICON,
        //        ImageName = null,
        //        Height = 60,
        //        Width = 60
        //    });
        //    return imageSizes;
        //}
    }
}