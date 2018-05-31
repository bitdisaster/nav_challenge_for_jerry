using NavTest.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavTest
{
    sealed partial class App : Application
    {
        public static NavigationService NavigationService { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            NavigationService = new NavigationService((Window.Current.Content = new Frame()) as Frame);
            Window.Current.Activate();

            NavigationService.GotoA("Launch");

            var gesture = Services.Template10.Services.KeyboardService.KeyboardService.Instance;
            gesture.AfterBackGesture += () => NavigationService.GoBack();
            gesture.AfterForwardGesture += () => NavigationService.GoForward();
        }
    }
}
