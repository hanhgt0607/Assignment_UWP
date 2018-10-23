using Assignment.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Assignment.view
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        private static string API_LOGIN = "http://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
     
        public Login()
        {
            this.InitializeComponent();
        }

        private async void  Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<String, String> Login= new Dictionary<string, string>();
            Login.Add("email", this.Email.Text);
            Login.Add("password", this.Password.Password);

            // Lay token
            HttpClient httpClient = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(Login), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_LOGIN, content).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
          
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
               
                Debug.WriteLine(responseContent);//luu file
               
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);//doc token
               
                // Luu token
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, responseContent);

                // Lay thong tin ca nhan bang token
                HttpClient Hclient = new HttpClient();

                Hclient.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
                var rps = Hclient.GetAsync("http://2-dot-backup-server-002.appspot.com/_api/v2/members/information").Result;
                Debug.WriteLine(await rps.Content.ReadAsStringAsync());
            }
            else
            {

                ErrorResponse errorObject = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                if (errorObject != null && errorObject.error.Count > 0)
                {
                    foreach (var key in errorObject.error.Keys)
                    {
                        var textMessage = this.FindName(key);
                        if (textMessage == null)
                        {
                            continue;
                        }
                        TextBlock textBlock = textMessage as TextBlock;
                        textBlock.Text = errorObject.error[key];
                        textBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            rootFrame.Navigate(typeof(view.Register));
        }
    }
}
