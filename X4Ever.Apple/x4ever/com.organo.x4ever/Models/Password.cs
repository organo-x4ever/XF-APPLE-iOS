namespace com.organo.x4ever.Models
{
    public class ForgotPassword
    {
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
    }

    public class PasswordDetail
    {
        public string RequestCode { get; set; }
        public string Password { get; set; }
    }

    public class PasswordChange
    {
        public long UserID { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
    }

    public class AccountDelete
    {
        public long UserID { get; set; }
        public string Password { get; set; }
    }
}