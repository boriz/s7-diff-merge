using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMC.Auth
{
    public class User
    {
        public string Name { get; private set; }
        public string Password { get; private set; }

        private List<string> roles;
        public List<string> Roles
        {
            get
            {
                if (roles == null)
                {
                    roles = new List<string>();
                }

                return roles;
            }
        }

        public bool IsAnonymous
        {
            get
            {
                return this.Name == UserAdminMgr.AnonUser;
            }
        }

        public bool IsDMC
        {
            get
            {
                return this.Name == UserAdminMgr.DmcUser;
            }
        }

        public User(string name, string password)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            Name = name;
            Password = password;
        }
    }
}
