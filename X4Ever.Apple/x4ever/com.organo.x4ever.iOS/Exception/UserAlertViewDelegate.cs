using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace com.organo.x4ever.ios.Exception
{
    public class UserAlertViewDelegate : UIAlertViewDelegate
    {
        #region Constructors

        public UserAlertViewDelegate()
        {
            UIAlertView alert = new UIAlertView();
            alert.Title = "What is your favourite movie?";
            alert.AddButton("OK");
            alert.AddButton("Cancel");
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            // Add this line.
            alert.Delegate = new MyDelegate();
            alert.Show();
        }

        #endregion

        public class MyDelegate : UIAlertViewDelegate
        {
            public override void Clicked(UIAlertView alertview, nint buttonIndex)
            {
                if (buttonIndex == 0)
                {
                    string userInput = alertview.GetTextField(0).Text;
                }
            }

            //public IntPtr Handle => throw new NotImplementedException();

            //public void Dispose()
            //{
            //    throw new NotImplementedException();
            //}
        }
    }
}