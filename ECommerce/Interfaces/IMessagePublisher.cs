using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message);
    }
}