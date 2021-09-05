using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class LoggedUsersRepository : Repository<LoggedInUser>, ILoggedUsersRepository
    {
        private readonly ApplicationDbContext _db;
        public LoggedUsersRepository( ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(CoverType coverType)
        {
            throw new NotImplementedException();
        }
    }
}
