using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    interface ITestTokenCancellation
    {
        void Main();

        Task<string> DoSomeWork(object obj);
    }
}