using System;
using System.Threading.Tasks;

namespace ITHSLab3.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        public event Action SplashCompleted;

        public SplashViewModel()
        {
            _ = RunAsync();
        }

        private async Task RunAsync()
        {
            await Task.Delay(11000);
            SplashCompleted?.Invoke();
        }
    }
}
