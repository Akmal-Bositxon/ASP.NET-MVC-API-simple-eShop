using eShopDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopDAL.Repositories
{
  public  class UserRepository
    {
        private UserDBEntities entities;
        public UserRepository()
        {
            entities = new UserDBEntities();
        }
        public IQueryable<User> GetAll()
        {
            return entities.Users;
        }
        public void Create(User user)
        {
            entities.Users.Add(user);
            entities.SaveChanges();
        }
    }
}
