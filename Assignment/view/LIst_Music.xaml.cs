using Assignment.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class LIst_Music : Page
    {
        private ObservableCollection<Song> listSong;
        public static string tokenKey = null;
        //private int _currentIndex;
        internal ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }
        private Song currentSong;

        public LIst_Music()
        {
            this.InitializeComponent();
            ReadToken();
            this.currentSong = new Song();
            this.ListSong = new ObservableCollection<Song>();
            this.GetSong();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic iBi1orSXvZisyF48jdnnOYmZURdkyz3rv1Jg0KBhFLsJOa90Tlu6DxxBKPYjm4b6");
            HttpResponseMessage responseMessage = httpClient.GetAsync(Service.ServiceUrl.GET_SONG).Result;
            string content = responseMessage.Content.ReadAsStringAsync().Result;
            Debug.WriteLine(content);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                ObservableCollection<Song> songResponse = JsonConvert.DeserializeObject<ObservableCollection<Song>>(content);
                foreach (var song in songResponse)
                {
                    this.ListSong.Add(song);
                }
                Debug.WriteLine("Oke, đã tạo thành công.");
            }
            else
            {
                entity.ErrorResponse errorResponse = JsonConvert.DeserializeObject<entity.ErrorResponse>(content);
                foreach (var key in errorResponse.error.Keys)
                {
                    if (this.FindName(key) is TextBlock textBlock)
                    {
                        textBlock.Text = errorResponse.error[key];
                    }
                }
            }
        }
        public static async void ReadToken()
        {
            if (tokenKey == null)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("token.txt");
                string content = await FileIO.ReadTextAsync(file);
                TokenResponse account_token = JsonConvert.DeserializeObject<TokenResponse>(content);
                Debug.WriteLine("token la: " + account_token.token);
                tokenKey = account_token.token;
            }
        }
        public async void GetSong()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization" ,"Basic " +tokenKey);
            var response =  httpClient.GetAsync(Service.ServiceUrl.GET_SONG);
            var result = await response.Result.Content.ReadAsStringAsync();
            Debug.WriteLine(result);
            ObservableCollection<Song> list = JsonConvert.DeserializeObject<ObservableCollection<Song>>(result);
            foreach(var songs in list)
            {
                this.ListSong.Add(songs);
            }
            Debug.WriteLine(result);


            
        }
         private async void btn_add(object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            this.currentSong.name = this.txt_name.Text;
            this.currentSong.thumbnail = this.txt_thumbnail.Text;
            this.currentSong.description = this.txt_description.Text;
            this.currentSong.singer = this.txt_singer.Text;
            this.currentSong.author = this.txt_author.Text;
            this.currentSong.link = this.txt_link.Text;

            var jsonMusic = JsonConvert.SerializeObject(this.currentSong);
            StringContent content = new StringContent(jsonMusic, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + tokenKey);
            var response = httpClient.PostAsync(Service.ServiceUrl.REGISTER_SONG, content);
            var result = await response.Result.Content.ReadAsStringAsync();
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Success");
            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(result);
                if (errorResponse.error.Count > 0)
                {
                    foreach (var key in errorResponse.error.Keys)
                    {
                        var objBykey = this.FindName(key);
                        var value = errorResponse.error[key];
                        if (objBykey != null)
                        {
                            TextBlock textBlock = objBykey as TextBlock;
                            textBlock.Text = "* " + value;
                        }
                    }
                }
            }
            this.txt_name.Text = String.Empty;
            this.txt_description.Text = String.Empty;
            this.txt_singer.Text = String.Empty;
            this.txt_author.Text = String.Empty;
            this.txt_thumbnail.Text = String.Empty;
            this.txt_link.Text = String.Empty;
        }
        //private void currentSongs(object sender, TappedRoutedEventArgs e)
        //{
        //    StackPanel panel = sender as StackPanel;
        //    Song chooseSong = panel.Tag as Song;
        //    Debug.WriteLine(chooseSong.link);
        //    _currentIndex = this.MyListSong.SelectedIndex;
        //    Uri mp3Link = new Uri(chooseSong.link);
        //    this.MediaPlayer.Source = mp3Link;
        //    this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
        //    Do_play();
        //}

        //private void Do_play()
        //{
        //    _isPlaying = true;
        //    this.status_song.Text = "Now Playing :";
        //    this.MediaPlayer.Play();
        //    PlayButton.Icon = new SymbolIcon(Symbol.Pause);


        //}
        //private void Do_pause()
        //{
        //    _isPlaying = false;
        //    this.status_song.Text = "pause Playing :";
        //    this.MediaPlayer.Pause();
        //    PlayButton.Icon = new SymbolIcon(Symbol.Play);
        //}



        //private void Player_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_isPlaying)
        //    {
        //        Do_pause();
        //    }
        //    else
        //    {
        //        Do_play();
        //    }
        //}

        //private void btn_Previous(object sender, RoutedEventArgs e)
        //{
        //    MediaPlayer.Stop();
        //    if (_currentIndex >= 0)
        //    {
        //        _currentIndex -= 1;
        //    }
        //    else
        //    {
        //        _currentIndex = listSong.Count - 1;
        //    }
        //    Uri mp3Link = new Uri(ListSong[_currentIndex].link);
        //    this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
        //    this.MediaPlayer.Source = mp3Link;
        //    Debug.WriteLine(mp3Link);
        //    Do_play();

        //}

        //private void btn_Next(object sender, RoutedEventArgs e)
        //{
        //    MediaPlayer.Stop();
        //    if (_currentIndex < ListSong.Count - 1)
        //    {
        //        _currentIndex += 1;
        //    }
        //    else
        //    {
        //        _currentIndex = 0;
        //    }
        //    Uri mp3Link = new Uri(ListSong[_currentIndex].link);
        //    this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
        //    Debug.WriteLine(mp3Link);
        //    this.MediaPlayer.Source = mp3Link;
        //    Do_play();
        //}

        private void btn_reset(object sender, RoutedEventArgs e)
        {
            this.txt_name.Text = String.Empty;
            this.txt_description.Text = String.Empty;
            this.txt_singer.Text = String.Empty;
            this.txt_author.Text = String.Empty;
            this.txt_thumbnail.Text = String.Empty;
            this.txt_link.Text = String.Empty;
        }
    }
}
