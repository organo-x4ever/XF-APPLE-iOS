using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IXEncryptionService
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}