using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CompanyReposaitory : Reposaitory<Company>, ICompanyReposaitory
    {
        private readonly ApplicationDbContext db;

        public CompanyReposaitory(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void update(Company obj)
        {
            db.Companies.Update(obj);
        }


    }
}
