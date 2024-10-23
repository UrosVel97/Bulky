using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{

    
    public class ApplicationUserReporsitory : Reporsitory<ApplicationUser>, IApplicationUserReporsitory
    {

        private ApplicationDbContext _db;


        public ApplicationUserReporsitory(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }




    }
}
