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
        public string Username;
        public string Password;

        public user()
        {
            FirstName = "";
            LastName = "";
            Username = "";
            Password = "";
        }
        public user(string fName, string lName, string uName, string pswdd)
        {
            FirstName = fName;
            LastName = lName;
            Username = uName;
            Password = pswdd;
        }
    }
}
