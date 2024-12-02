using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Interfaces
{
    public interface ILoggingService
    {
        Task LogAsync(string message);
    }
}