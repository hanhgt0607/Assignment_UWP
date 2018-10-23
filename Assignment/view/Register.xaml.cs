using Assignment.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Assignment.view
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        private string currentUploadUrl;
        private Account currentAccount;
        private StorageFile photo;

        public object APIHandle { get; private set; }

        public Register()
        {
            this.InitializeComponent();
            this.currentAccount = new Account();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private async void Choose_Image(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            this.photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (this.photo == null)
            {
                
                return;
            }
            HttpClient httpClient = new HttpClient();
            currentUploadUrl = await httpClient.GetStringAsync(Service.ServiceUrl.GET_UPLOAD_URL);
            Debug.WriteLine("Upload url: " + currentUploadUrl);
            HttpUploadFile(currentUploadUrl, "myFile", "image/png");
        }

        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create(url);
            httpWeb.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWeb.Method = "POST";

            Stream rs = await httpWeb.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            
            Stream fileStream = await this.photo.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await httpWeb.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                string imageUrl = reader2.ReadToEnd();
                Avatar.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                AvatarUrl.Text = imageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                httpWeb = null;
            }
        }

        private async void Do_Submit(object sender, RoutedEventArgs e)
        {
            // validate data.
            //this.currentAccount.firstName = this.FirstName.Text;
            //this.currentAccount.lastName = this.LastName.Text;
            //this.currentAccount.avatar = this.AvatarUrl.Text;
            //this.currentAccount.address = this.Address.Text;
            //this.currentAccount.introduction = this.Introduction.Text;
            //this.currentAccount.phone = this.Phone.Text;
            //this.currentAccount.email = this.Email.Text;
            //this.currentAccount.password = this.Password.Password;a

            string jsonAccount = JsonConvert.SerializeObject(this.currentAccount);

            HttpClient httpClient = new HttpClient();
            var content = new StringContent(jsonAccount, Encoding.UTF8, "application/json");
            //var result = httpClient.PostAsync("https://1-dot-backup-server-002.appspot.com/member/register", content).Result.Content.ReadAsStringAsync();
            var response = httpClient.PostAsync(Service.ServiceUrl.MEMBER_REGISTER, content);
            var contents = await response.Result.Content.ReadAsStringAsync();
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Success");
            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contents);
                //if (errorResponse.error.Count > 0)
                //{
                //    foreach (var key in errorResponse.error.Keys)
                //    {
                //        var objectByKey = this.FindName(key);
                //        var value = errorResponse.error[key];
                //        if (objectByKey != null)
                //        {
                //            TextBlock textBlock = objectByKey as TextBlock;
                //            textBlock.Text = "* " + value;
                //            textBlock.Visibility = Visibility.Visible;
                //        }
                //    }
                //}
                foreach (var key in errorResponse.error.Keys)
                {
                    if (this.FindName(key) is TextBlock textBlock)
                    {
                        textBlock.Text = errorResponse.error[key];
                    }
                }
            }

        }
      

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            this.currentAccount.gender = Int32.Parse(radio.Tag.ToString());
        }

        private void BirthdayPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.currentAccount.birthday = sender.Date.Value.ToString("yyyy-MM-dd");
        }

        private void Do_Login(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            rootFrame.Navigate(typeof(view.Login));
        }
    }
}
