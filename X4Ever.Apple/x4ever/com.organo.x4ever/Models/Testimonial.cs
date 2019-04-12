using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace com.organo.x4ever.Models
{
    public class Testimonial : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string PersonName { get; set; }
        public string PersonPhoto { get; set; }

        private ImageSource _personPhotoSource;
        public const string PersonPhotoSourcePropertyName = "PersonPhotoSource";
        public ImageSource PersonPhotoSource
        {
            get { return _personPhotoSource; }
            set { SetProperty(ref _personPhotoSource, value, PersonPhotoSourcePropertyName); }
        }

        public float PersonImageHeight { get; set; }
        public float PersonImageWidth { get; set; }
        public string VideoUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public decimal StarRating { get; set; }
        public short DisplaySequence { get; set; }
        public bool Active { get; set; }
        public bool IsVideoExists => !string.IsNullOrEmpty(VideoUrl);
        public bool IsPhoto => !IsVideoExists;

        protected void SetProperty<U>(
            ref U backingStore, U value,
            string propertyName,
            Action onChanged = null,
            Action<U> onChanging = null)
        {
            if (EqualityComparer<U>.Default.Equals(backingStore, value))
                return;

            if (onChanging != null)
                onChanging(value);

            OnPropertyChanging(propertyName);

            backingStore = value;

            if (onChanged != null)
                onChanged();

            OnPropertyChanged(propertyName);
        }

        #region INotifyPropertyChanging implementation

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion INotifyPropertyChanging implementation

        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging == null)
                return;

            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged implementation

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}