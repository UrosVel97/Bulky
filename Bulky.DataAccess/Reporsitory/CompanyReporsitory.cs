using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    public class CompanyReporsitory : Reporsitory<Company>, ICompanyReporsitory
    {

        private readonly ApplicationDbContext _db;

        public CompanyReporsitory(ApplicationDbContext db) :base(db) 
        {
            this._db=db;
        }
        
            
        
        public void Update(Company obj)
        {
            
            _db.Companies.Update(obj);
        }
    }
}
