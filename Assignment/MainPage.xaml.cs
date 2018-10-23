using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Assignment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string CurrentTag = "";

        public static long AccountId = 1538626293330;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void Menu_click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (CurrentTag == radio.Tag.ToString())
            {
                return;
            }
            switch (radio.Tag.ToString())
            {
                case "Home":
                    CurrentTag = "Home";
                    this.MyFrame.Navigate(typeof(view.Home));
                    break;
                case "Register":
                    CurrentTag = "Register";
                    this.MyFrame.Navigate(typeof(view.Register));
                    break;
                case "Login":
                    CurrentTag = "Login";
                    this.MyFrame.Navigate(typeof(view.Login));
                    break;
                case "Ablum":
                    CurrentTag = "Ablum";
                    this.MyFrame.Navigate(typeof(view.LIst_Music));
                    break;
                default:
                    break;

            }
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            this.SplitView.IsPaneOpen = !this.SplitView.IsPaneOpen;
           
        }
    }
}
