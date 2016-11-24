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
using System.Threading.Tasks;
using Org.Apache.Http.Client.Methods;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

namespace ChatApp.Core.ViewModels
{
    // We subclassed BasicViewModel to get access to IsBusy and the fields containing common services 
    public class LoginViewModel : BasicViewModel
    {
        // We added the Username and Password properties to be set by the View layer 
        public string Username { get; set; }

        public string Password { get; set; }
        public int PhoneNumber { get; set; }

        // We implemented a Login method to be called from View, with validation on Username and Password properties.
        public async Task Login()
        {
            if (string.IsNullOrEmpty(Username))
                throw new Exception("Username is blank.");

            if (string.IsNullOrEmpty(Password))
                throw new Exception("Password is blank.");
            //if (PhoneNumber == 0)
            //    throw new Exception("Phone number is empty");

            // We set IsBusy during the call to the Login method on IWebService 
            //IsBusy = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

                // We set the User property by awaiting the result from Login on the web service 
                //settings.User = await service.Login(Username, Password);
                //settings.Save();
                //string Uri = "http://ycandgap.me/login.php?";
                //WebClient webClient = new WebClient();
                
                // this is where we will send it
                string uri = "https://ycandgap.me/login.php";

                // create a request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        username = Username,
                        password = Password
                    });

                    streamWriter.Write(json);
                }

                // grab te response and print it out to the console along with the status code
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }

        //public async Task RegisterPush(string deviceToken)
        //{
        //    if (settings.User == null)
        //        throw new Exception("User is null");

            //    await service.RegisterPush(settings.User.Id, deviceToken);
            //}
        }
    }