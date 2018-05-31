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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NavTest
{
    public sealed partial class B : Page
    {
        public B()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyParameterTextBox.Text = DateTime.Now.ToString("ss");
            MyTextBlock.Text = $"Mode:{e.NavigationMode}";
            MyParameterTextBlock.Text = $"({e.Parameter})";
            MyBackStackTextBlock.Text = string.Join(" > ", App.NavigationService.BackStack.Select(x => $"{x.SourcePageType.Name}:({x.Parameter})"));
            MyForeStackTextBlock.Text = string.Join(" > ", App.NavigationService.ForwardStack.Select(x => $"{x.SourcePageType.Name}:({x.Parameter})"));
        }

        private void MyParameterTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Go_Clicked(null, null);
            }
        }

        private void GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            App.NavigationService.GoBack();
        }

        private void Go_Clicked(object sender, RoutedEventArgs e)
        {
            App.NavigationService.GotoA(MyParameterTextBox.Text);
        }

        private void GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            App.NavigationService.GoForward();
        }
    }
}
