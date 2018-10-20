using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class user
    {
        public string FirstName;
        public string LastName;
        public string UserName;
        public string Password;

        public user()
        {
            FirstName = "";
            LastName = "";
            UserName = "";
            Password = "";
        }
        public user(string fName, string lName, string uName, string pswdd)
        {
            FirstName = fName;
            LastName = lName;
            UserName = uName;
            Password = pswdd;
        }
    }
}
