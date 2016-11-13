using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string number, string message);
    }
}
