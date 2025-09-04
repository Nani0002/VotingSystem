using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace VotingSystem.WPF.Services
{
    public class NavigationService
    {
        private Frame? _frame;
        private readonly IServiceProvider _provider;

        private readonly Dictionary<Type, object?> _navigationParameters = new();

        public NavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate<TPage>() where TPage : Page
        {
            if (_frame is null)
                throw new InvalidOperationException("Frame not initialized");

            var page = _provider.GetRequiredService<TPage>();
            _frame.Navigate(page);
        }

        public void Navigate<TPage, TParam>(TParam parameter) where TPage : Page
        {
            _navigationParameters[typeof(TPage)] = parameter;
            Navigate<TPage>();
        }

        public TParam? GetParameter<TPage, TParam>()
        {
            if (_navigationParameters.TryGetValue(typeof(TPage), out var value) && value is TParam param)
            {
                return param;
            }

            return default;
        }

        public void ClearParameter<TPage>()
        {
            _navigationParameters.Remove(typeof(TPage));
        }
    }
}
