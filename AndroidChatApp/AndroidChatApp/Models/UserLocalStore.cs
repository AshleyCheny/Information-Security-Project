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
            spEditor.PutString("RegistrationID", user.RegisterationID.ToString());
            spEditor.PutString("IdentityKey", user.IdentityKey);
            spEditor.PutString("PreKeys", user.PreKeys);
            spEditor.PutString("SignedPreKeys",user.SignedPreKey);
            spEditor.PutString("PrivateKey", user.PrivateKey);
            spEditor.Commit();
        }

        //**get the logged in users
        //** needs to connect to the database and get the real value
        public User GetLoggedInUser()
        {
            User StoredUser = new User();

            StoredUser.Username = userLocalDatabase.GetString("Username", string.Empty);
            StoredUser.Password = userLocalDatabase.GetString("Password", string.Empty);
            StoredUser.RegisterationID = Convert.ToUInt32(userLocalDatabase.GetString("RegistrationID", string.Empty));
            StoredUser.IdentityKey = userLocalDatabase.GetString("IdentityKey", string.Empty);
            StoredUser.PreKeys = userLocalDatabase.GetString("PreKeys", string.Empty);
            StoredUser.SignedPreKey = userLocalDatabase.GetString("SignedPreKeys", string.Empty);
            StoredUser.PrivateKey = userLocalDatabase.GetString("PrivateKey", string.Empty);

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