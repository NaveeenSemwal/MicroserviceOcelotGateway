using AuthenticationService.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBook.IntegrationTesting
{
    public class Utilities
    {
        public static void InitializeDbForTests(ApplicationDbContext db)
        {
            db.Users.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ApplicationDbContext db)
        {
            db.Users.RemoveRange(db.Users);
            InitializeDbForTests(db);
        }

        public static List<IdentityUser> GetSeedingMessages()
        {
            var users = new List<IdentityUser>();

            users.Add(new IdentityUser { Email= "champ1@gmail.com",UserName= "champ1@gmail.com" });

            return users;
        }
    }
}
