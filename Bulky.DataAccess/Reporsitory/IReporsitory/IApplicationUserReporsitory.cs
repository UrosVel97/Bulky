using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    public interface IApplicationUserReporsitory :IReporsitory<ApplicationUser>
    {
        void Update(ApplicationUser user);
    }
}
