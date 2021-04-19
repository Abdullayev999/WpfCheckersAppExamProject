using Checkers.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Checkers.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
        }
        private RelayCommand startCommand = null;
        private readonly IMessenger messenger;

        public RelayCommand StartCommand => startCommand ??= new RelayCommand(
        () =>
        {
            messenger.Send(new NavigationMessage { ViewModelType = typeof(GameViewModel) });
        });


        private RelayCommand loadGameCommand = null;

        public RelayCommand LoadGameCommand => loadGameCommand ??= new RelayCommand(
        () =>
        {
            var gameViewModel = App.Services.GetInstance<GameViewModel>();
            var isLoad = gameViewModel.Load();

            if (isLoad)
            {
                messenger.Send(new NavigationMessage { ViewModelType = typeof(GameViewModel) });
            }
            else
            {
                MessageBox.Show("Not Saving Game!!", "Checker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        });


        private RelayCommand exitCommand = null;

        public RelayCommand ExitCommand => exitCommand ??= new RelayCommand(
        () =>
        {
             Application.Current.Shutdown();
        });


        private RelayCommand newGametCommand = null;

        public RelayCommand NewGametCommand => newGametCommand ??= new RelayCommand(
        () =>
        {
            var GameViewModel = App.Services.GetInstance<GameViewModel>() ;
            GameViewModel.NewGame();
            messenger.Send(new NavigationMessage() { ViewModelType = typeof(GameViewModel) });

        });

        private RelayCommand settingCommand = null;

        public RelayCommand SettingCommand => settingCommand ??= new RelayCommand(
        () =>
        {
            messenger.Send(new NavigationMessage() { ViewModelType = typeof(SettingViewModel) });
        });
    }
    
}
