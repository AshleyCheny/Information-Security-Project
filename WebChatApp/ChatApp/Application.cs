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

namespace ChatApp
{
    // Use built-in android theme: Theme.Holo.Light
    [Application(Theme = "@android: style / Theme.Holo.Light")]
    //Implement main application class where we can register everything in our ServiceContainer.
    public class Application : Android.App.Application
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //ViewModels
            ServiceContainer.Register<LoginViewModel>(() => new LoginViewModel());
            ServiceContainer.Register<FriendViewModel>(() => new FriendViewModel());
            ServiceContainer.Register<MessageViewModel>(() => new MessageViewModel());
            ServiceContainer.Register<RegisterViewModel>(() => new RegisterViewModel());

            //Models
            ServiceContainer.Register<ISettings>(() => new FakeSettings());
            ServiceContainer.Register<IWebService>(() => new FakeWebService());

        }
    }
}