using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NavTest.Services
{
    public class NavigationService
    {
        private Frame _frame;
        public NavigationService(Frame frame) => _frame = frame;
        public void GoForward() => Iff(_frame.CanGoForward, () => _frame.GoForward());
        public void GoBack() => Iff(_frame.CanGoBack, () => _frame.GoBack());
        public void GotoA(string parameter) => _frame.Navigate(typeof(A), parameter);
        public void GotoB(string parameter) => _frame.Navigate(typeof(B), parameter);
        public IList<PageStackEntry> BackStack => _frame.BackStack;
        public IList<PageStackEntry> ForwardStack => _frame.ForwardStack;

        void Iff(bool predicate, Action operation)
        {
            if (predicate)
            {
                operation();
            }
        }
    }
}
