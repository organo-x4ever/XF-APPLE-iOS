using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IInformationMessageServices
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}