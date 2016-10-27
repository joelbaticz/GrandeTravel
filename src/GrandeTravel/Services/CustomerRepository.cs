using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GrandeTravel.Models;

namespace GrandeTravel.Services
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
    }
}
