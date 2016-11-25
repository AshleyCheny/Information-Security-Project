using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;

namespace AndroidChatApp.Models
{
    public class UserLocalStore
    {
        //**store user information details in local database (on the phone)
        public static string SP_NAME = "userDetails";
        ISharedPreferences userLocalDatabase;
        public UserLocalStore(Context context)
        {
            userLocalDatabase = context.GetSharedPreferences(SP_NAME,0);
        }

        //get the data from local database and store them
        public void StoreUserData(User user)
        {
            ISharedPreferencesEditor spEditor = userLocalDatabase.Edit();
            spEditor.PutString("Username", user.Username);
            spEditor.PutString("Password", user.Password);
            spEditor.PutInt("RegistrationID", user.RegisterationID);
            spEditor.PutString("IdentityKey", user.IdentityKey);
            spEditor.PutString("PreKeys", user.PreKeys);
            spEditor.PutString("SignedPreKeys",user.SignedPreKeys);
            spEditor.PutString("PrivateKey", user.PrivateKey);
            spEditor.Commit();
        }

        //**get the logged in users
        //** needs to connect to the database and get the real value
        public User GetLoggedInUser()
        {
            User StoredUser = new User();

            StoredUser.Username = userLocalDatabase.GetString("Username", "ashley");
            StoredUser.Password = userLocalDatabase.GetString("Password", "");
            StoredUser.RegisterationID = userLocalDatabase.GetInt("RegistrationID", -1);
            StoredUser.IdentityKey = userLocalDatabase.GetString("IdentityKey", "");
            StoredUser.PreKeys = userLocalDatabase.GetString("PreKeys", "");
            StoredUser.SignedPreKeys = userLocalDatabase.GetString("SignedPreKeys", "");
            StoredUser.PrivateKey = userLocalDatabase.GetString("PrivateKey", "");

            return StoredUser;

        }

        //**If the user logged in , set LoggedIn as true. otherwise set it as false
        public void SetUserLoggedIn(Boolean LoggedIn)
        {
            ISharedPreferencesEditor spEditor = userLocalDatabase.Edit();
            spEditor.PutBoolean("LoggedIn", LoggedIn);
            spEditor.Commit();
        }

        //** 
        public void clearUserData()
        {
            ISharedPreferencesEditor spEditor = userLocalDatabase.Edit();
            spEditor.Clear();
            spEditor.Commit();
        }

    }
}