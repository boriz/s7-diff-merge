using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DMC.UI.Auth
{
    /// <summary>
    /// Interaction logic for UserLoginDialog.xaml
    /// </summary>
    public partial class UserLoginDialog : Window
    {
        public UserLoginDialog()
        {
            InitializeComponent();
        }

        //private void UserLogin_LoginSuccessful(object sender, DMCBase.EventArgs<DMC.Auth.User> e)
        //{
        //    this.DialogResult = true;
        //}

        //private void UserLogin_LoginCanceled(object sender, EventArgs e)
        //{
        //    this.DialogResult = false;
        //}
    }
}
