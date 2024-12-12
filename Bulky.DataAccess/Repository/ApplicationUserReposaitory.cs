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
    public class ApplicationUserReposaitory : Reposaitory<ApplicationUser>,IApplicationUserReposaitory
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }


        
        public void Update(ApplicationUser applicationUser)
        {
            db.ApplicationUsers.Update(applicationUser);
        }
    }
}
