using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DMC.Auth
{
    public class UserAdminMgr : INotifyPropertyChanged
    {
        public const string DmcUser = "dmc";
        internal const string AnonUser = "ANON_USER: 21020008-217C-444D-A7ED-22EF45344F05";
        
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static UserAdminMgr manager = new UserAdminMgr();
        public static UserAdminMgr Manager
        {
            get
            {
                return manager;
            }
        }

        private UserAdminMgr()
        {
        }

        private Dictionary<string, User> users;
        public Dictionary<string, User> Users
        {
            get
            {
                if (users == null)
                {
                    users = new Dictionary<string, User>()
                    {
                        { AnonUser, new User(AnonUser, "") },
                        { DmcUser, new User(DmcUser, "dmc") },
                    };
                }

                return users;
            }
        }

        private List<string> roles;
        public List<string> Roles
        {
            get
            {
                if (roles == null)
                {
                    roles = new List<string>()
                    {
                    };
                }

                return roles;
            }
        }

        private User authenticatedUser;
        public User AuthenticatedUser
        {
            get
            {
                if (authenticatedUser == null)
                {
                    AuthenticatedUser = Users[AnonUser];
                }

                return authenticatedUser;
            }
            private set
            {
                authenticatedUser = value;
                NotifyPropertyChanged("AuthenticatedUser");
            }
        }

        public void AuthenticateUser(string username, string password)
        {
            try
            {
                foreach (KeyValuePair<string, User> kvp in Users)
                {
                    if (kvp.Key == username)
                    {
                        if (kvp.Value.Password == password)
                        {
                            AuthenticatedUser = kvp.Value;
                            logger.Info("Authentication succeeded for user {0}.", username);
                            return;
                        }
                        else
                        {
                            throw new ApplicationException(string.Format("Authentication Failed. Password incorrect for user \"{0}\".", username));
                        }
                    }
                }

                throw new ApplicationException(string.Format("Authentication Failed. User \"{0}\" not found.", username));
            }
            catch (Exception e)
            {
                logger.InfoException("Authentication Failed", e);
                throw e;
            }
        }

        public void Logout()
        {
            AuthenticatedUser = Users[AnonUser];
            logger.Info("User logged out.");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyIn)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyIn));
            }
        }

        #endregion
    }
}
