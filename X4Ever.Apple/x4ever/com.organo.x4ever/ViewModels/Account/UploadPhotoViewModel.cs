﻿using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Account
{
    public class UploadPhotoViewModel : BaseViewModel
    {
        private readonly IHelper helper;
        public UploadPhotoViewModel(INavigation navigation = null) : base(navigation)
        {
            helper = DependencyService.Get<IHelper>();
            this.SetPageImageSize();
            this.ImageFront = this.ImageDefault;
            this.ImageSide = this.ImageDefault;
        }

        public string ImageDefault => TextResources.icon_camera;

        private string imageFront;
        public const string ImageFrontPropertyName = "ImageFront";

        public string ImageFront
        {
            get { return imageFront; }
            set { SetProperty(ref imageFront, value, ImageFrontPropertyName, ChangeImageFront); }
        }

        private void ChangeImageFront()
        {
            this.ImageFrontSource = this.ImageFront == this.ImageDefault
                ? ImageResizer.ResizeImage(this.ImageFront, UploadImageSize)
                : helper.GetFileUri(this.ImageFront, FileType.Upload);
        }

        private ImageSource imageFrontSource;
        public const string ImageFrontSourcePropertyName = "ImageFrontSource";

        public ImageSource ImageFrontSource
        {
            get { return imageFrontSource; }
            set { SetProperty(ref imageFrontSource, value, ImageFrontSourcePropertyName); }
        }

        private string imageSide;
        public const string ImageSidePropertyName = "ImageSide";

        public string ImageSide
        {
            get { return imageSide; }
            set { SetProperty(ref imageSide, value, ImageSidePropertyName, ChangeImageSide); }
        }

        private void ChangeImageSide()
        {
            this.ImageSideSource = this.ImageSide == this.ImageDefault
                ? ImageResizer.ResizeImage(this.ImageSide, UploadImageSize)
                : helper.GetFileUri(this.ImageSide, FileType.Upload);
        }

        private ImageSource imageSideSource;
        public const string ImageSideSourcePropertyName = "ImageSideSource";

        public ImageSource ImageSideSource
        {
            get { return imageSideSource; }
            set { SetProperty(ref imageSideSource, value, ImageSideSourcePropertyName); }
        }

        public ImageSize UploadImageSize { get; set; }

        private void SetPageImageSize()
        {
            UploadImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.UPLOAD_CAMERA_IMAGE);
            if (UploadImageSize != null)
            {
                this.CameraImageHeight = UploadImageSize.Height;
                this.CameraImageWidth = UploadImageSize.Width;
            }
        }

        private float cameraImageHeight;
        public const string CameraImageHeightPropertyName = "CameraImageHeight";

        public float CameraImageHeight
        {
            get { return cameraImageHeight; }
            set { SetProperty(ref cameraImageHeight, value, CameraImageHeightPropertyName); }
        }

        private float cameraImageWidth;
        public const string CameraImageWidthPropertyName = "CameraImageWidth";

        public float CameraImageWidth
        {
            get { return cameraImageWidth; }
            set { SetProperty(ref cameraImageWidth, value, CameraImageWidthPropertyName); }
        }
    }
}