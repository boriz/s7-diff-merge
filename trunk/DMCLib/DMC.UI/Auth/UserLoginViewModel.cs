//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using DMCBase;
//using DMC.Auth;
//using System.Windows;

//namespace DMC.UI.Auth
//{
//    public class UserLoginViewModel : NotifyPropertyChangedBase
//    {
//        public event EventHandler<EventArgs<User>> LoginSuccessful;

//        private string _Username;
//        public string Username
//        {
//            get
//            {
//                return _Username;
//            }
//            set
//            {
//                if (value.CompareTo(_Username) != 0)
//                {
//                    _Username = value;
//                    NotifyPropertyChanged("Username");
//                    NotifyPropertyChanged("UsernameRecognized");
//                    NotifyPropertyChanged("PasswordMatchesUser");
//                }
//            }
//        }

//        private string _Password;
//        public string Password
//        {
//            get
//            {
//                return _Password;
//            }
//            set
//            {
//                if (value.CompareTo(_Password) != 0)
//                {
//                    _Password = value;
//                    NotifyPropertyChanged("Password");
//                    NotifyPropertyChanged("PasswordMatchesUser");
//                }
//            }
//        }

//        public bool UsernameRecognized
//        {
//            get
//            {
//                return UserAdminMgr.Manager.Users.ContainsKey(Username);
//            }
//        }

//        public bool PasswordMatchesUser
//        {
//            get
//            {
//                return UserAdminMgr.Manager.Users.ContainsKey(Username) && 
//                    (UserAdminMgr.Manager.Users[Username].Password == Password);
//            }
//        }

//        private void Login()
//        {
//            try
//            {
//                UserAdminMgr.Manager.AuthenticateUser(Username, Password);

//                EventHandler<EventArgs<User>> handler = LoginSuccessful;

//                if (handler != null)
//                {
//                    handler(this, new EventArgs<User>(UserAdminMgr.Manager.AuthenticatedUser));
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(string.Format("Login Failed:{0}{1}", Environment.NewLine, ex.Message));
//            }
//        }
//    }
//}
