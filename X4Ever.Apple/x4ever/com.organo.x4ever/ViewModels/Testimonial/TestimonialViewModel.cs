﻿using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Models;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Testimonial
{
    public class TestimonialViewModel : BaseViewModel
    {
        private ITestimonialService _testimonialService;
        private readonly IHelper _helper;

        public TestimonialViewModel(INavigation navigation = null) : base(navigation)
        {
            _testimonialService = DependencyService.Get<ITestimonialService>();
            _helper = DependencyService.Get<IHelper>();
            this.Testimonials = new List<Models.Testimonial>();
            this.TestimonialView = null;
        }

        public async Task<List<Models.Testimonial>> GetTestimonialAsync()
        {
            var imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.TESTIMONIAL_PERSON_IMAGE);
            if (imageSize == null)
                imageSize = new ImageSize();
            var testimonials = await _testimonialService.GetAsync(true);
            foreach (var testimonial in testimonials)
            {
                testimonial.PersonPhotoSource =
                    _helper.GetFileUri(_helper.GetFilePath(testimonial.PersonPhoto, FileType.TestimonialPhoto),
                        FileType.None);
                testimonial.PersonImageHeight = imageSize.Height;
                testimonial.PersonImageWidth = imageSize.Width;
            }

            this.Testimonials = testimonials.OrderBy(t => t.DisplaySequence).ToList();
            return this.Testimonials;
        }

        private View _testimonialView;
        public const string TestimonialViewPropertyName = "TestimonialView";

        public View TestimonialView
        {
            get { return _testimonialView; }
            set { SetProperty(ref _testimonialView, value, TestimonialViewPropertyName); }
        }

        private List<Models.Testimonial> _testimonials;
        public const string TestimonialsPropertyName = "Testimonials";

        public List<Models.Testimonial> Testimonials
        {
            get { return _testimonials; }
            set { SetProperty(ref _testimonials, value, TestimonialsPropertyName); }
        }
        
        private ICommand readCommand;

        public ICommand ReadCommand
        {
            get { return readCommand ?? (readCommand = new Command((obj) => { Navigation.PopModalAsync(); })); }
        }
    }

    public enum LayoutSide
    {
        Left = 0,
        Right = 1
    }
}