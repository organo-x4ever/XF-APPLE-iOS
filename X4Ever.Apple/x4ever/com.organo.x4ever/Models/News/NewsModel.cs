using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace com.organo.x4ever.Models.News
{
    public class NewsModel : INotifyPropertyChanged
    {
        public NewsModel()
        {
            ID = 0;
            Header = string.Empty;
            Body = string.Empty;
            PostDate = new DateTime();

            PostedBy = string.Empty;
            NewsImage = string.Empty;
            NewsImagePosition = string.Empty;

            Active = false;
            ModifyDate = new DateTime();
            ModifiedBy = string.Empty;

            LanguageCode = string.Empty;
            ApplicationId = 0;
        }

        public Int16 ID { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public DateTime PostDate { get; set; }
        public string PostedBy { get; set; }
        public string NewsImage { get; set; }

        private ImageSource _newsImageSource;
        public const string NewsImageSourcePropertyName = "NewsImageSource";
        public ImageSource NewsImageSource
        {
            get { return _newsImageSource; }
            set { SetProperty(ref _newsImageSource, value, NewsImageSourcePropertyName); }
        }

        public string NewsImagePosition { get; set; }
        public bool Active { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsTop => !string.IsNullOrEmpty(NewsImagePosition) && NewsImagePosition.Contains("top");

        public bool IsBottom => !string.IsNullOrEmpty(NewsImagePosition) && NewsImagePosition.Contains("bottom");

        public string LanguageCode { get; set; }
        public int ApplicationId { get; set; }


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