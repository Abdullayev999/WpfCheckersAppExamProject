using Checkers.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Checkers.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private readonly IMessenger messenger;
        private string secondPlayer;
        private string firstPlayer;
        private bool firstMove;
        public string FirstCheckerColor { get; set; }
        public string SecondCheckerColor { get; set; }
        private bool selectOne;
        private bool selectTwo;
        private bool selectThree;
        private bool selectFour;
        private bool selectFive;
        private bool selectSix;

        public SettingViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
            SelectOne = true;
            FirstPlayer = "Player 1";
            SecondPlayer = "Player 2";
            FirstMove = true;
        }

        public bool SelectOne { get => selectOne; set => Set(ref selectOne, value); }
        public bool SelectTwo { get => selectTwo; set => Set(ref selectTwo, value); }
        public bool SelectThree { get => selectThree; set => Set(ref selectThree, value); }
        public bool SelectFour { get => selectFour; set => Set(ref selectFour, value); }
        public bool SelectFive { get => selectFive; set => Set(ref selectFive, value); }
        public bool SelectSix { get => selectSix; set => Set(ref selectSix, value); }
        public string SecondPlayer { get => secondPlayer; set => Set(ref secondPlayer, value); }
        public string FirstPlayer { get => firstPlayer; set => Set(ref firstPlayer, value); }
        public bool FirstMove { get => firstMove; set => Set(ref firstMove, value); }

               
        private RelayCommand backCommand = null;
        public RelayCommand BackCommand => backCommand ??= new RelayCommand(
        () =>
        {
            SwitchColor();
            messenger.Send(new NavigationMessage() { ViewModelType = typeof(HomeViewModel) });
            messenger.Send(new SettingInfoMessage()
            {
                FirstName = FirstPlayer,
                SecondName = SecondPlayer,
                FirstMove = FirstMove == true ? 1 : 2
            });

            var gameViewModel = App.Services.GetInstance<GameViewModel>();
          
            gameViewModel.PlayerOneColorChecker = FirstCheckerColor;
            gameViewModel.PlayerTwoColorChecker = SecondCheckerColor;
            gameViewModel.CreatMap();
        });     
        public void SwitchColor()
        {
            if (SelectOne)
            {
                FirstCheckerColor = "Resources/white.png";
                SecondCheckerColor = "Resources/black.png";
            }
            else if (SelectTwo)
            {
                FirstCheckerColor = "Resources/black.png";
                SecondCheckerColor = "Resources/white.png";  
            }
            else if (SelectThree)
            {
                FirstCheckerColor = "Resources/red.png";
                SecondCheckerColor = "Resources/black.png";
            }
            else if (SelectFour)
            {
                FirstCheckerColor = "Resources/black.png";
                SecondCheckerColor = "Resources/red.png";
            }
            else if (SelectFive)
            {
                FirstCheckerColor = "Resources/red.png";
                SecondCheckerColor = "Resources/blackCLasik.png";
            }
            else if (SelectSix)
            {
                FirstCheckerColor = "Resources/blackCLasik.png";
                SecondCheckerColor = "Resources/red.png";
            }
        }
    }
}
