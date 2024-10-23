using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    public interface IProductReporsitory : IReporsitory<Product>
    {
        void Update(Product product);
    }
}
