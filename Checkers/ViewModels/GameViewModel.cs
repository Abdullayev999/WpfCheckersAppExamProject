using Checkers.Messages;
using Checkers.Models;
using Checkers.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Checkers.ViewModels
{
    class GameViewModel : ViewModelBase
    {
        const int mapSize = 8;
        private readonly IMessenger messenger;
        private readonly IStorage repository;
        int CurrentPlayer;
        List<Checker> simpleSteps = new List<Checker>();
        int countEatSteps = 0;
        Checker prevChecker;
        Checker pressedChecker;
        bool isContinue = false;
        bool isMoving;
        private int[,] map;
        Checker[,] checkers = new Checker[mapSize, mapSize];
        private ObservableCollection<Checker> save;
        private string playerOneColorChecker;
        private string playerTwoColorChecker;
        private bool isNewGame;
        private string secondPlayer;
        private string firstPlayer;
        private string currentGo;

        public string PlayerOneColorChecker { get => playerOneColorChecker; set => Set(ref playerOneColorChecker, value); }    
        public string PlayerTwoColorChecker { get => playerTwoColorChecker; set => Set(ref playerTwoColorChecker, value); }
           
        public bool IsNewGame
        {
            get => isNewGame;
            set
            {
                Set(ref isNewGame, value);

                if (isNewGame)
                {
                    ResetGame();
                }
            }
        }       
        public string SecondPlayer { get => secondPlayer; set => Set(ref secondPlayer, value); }
        public string FirstPlayer { get => firstPlayer; set => Set(ref firstPlayer, value); }      

        public string CurrentGo { get => currentGo; set => Set(ref currentGo, value); }

        public ObservableCollection<Checker> Checkers { get => save; set => Set(ref save, value); }

        public GameViewModel(IMessenger messenger , IStorage repository)
        {
            this.messenger = messenger;
            this.repository = repository;

            PlayerOneColorChecker = "Resources/white.png";
            PlayerTwoColorChecker = "Resources/black.png";
            CurrentPlayer = 1;

            FirstPlayer = "Player 1";
            SecondPlayer = "Player 2";
                      

            Checkers = new ObservableCollection<Checker>();        

            messenger.Register<SettingInfoMessage>(this,
            message =>
            {
                FirstPlayer = message.FirstName;
                SecondPlayer = message.SecondName;
                CurrentPlayer = message.FirstMove;
            });

           
            map = new int[mapSize, mapSize];
            Init();
        }

       
        private RelayCommand saveCommand = null;
        private RelayCommand backCommand = null;
        private RelayCommand exitCommand = null;
        private RelayCommand<Checker> clickCommand = null;

        public RelayCommand SaveCommand => saveCommand ??= new RelayCommand(
        () =>
        {
            List<int> tmp = new List<int>();
            foreach (var item in map)
            {
                tmp.Add(item);
            }

             repository.Save(tmp, FirstPlayer, SecondPlayer,CurrentPlayer,PlayerOneColorChecker,PlayerTwoColorChecker);
        });    
        public RelayCommand BackCommand => backCommand ??= new RelayCommand(
        () =>
        {
            messenger.Send(new NavigationMessage() { ViewModelType = typeof(HomeViewModel) });
        });
        public RelayCommand ExitCommand => exitCommand ??= new RelayCommand(
        () =>
        {
            Application.Current.Shutdown();
        });
        public RelayCommand<Checker> ClickCommand => clickCommand ??= new RelayCommand<Checker>(
        checker =>
        {
            if (checker!=null)
            {
                OnFigurePress(checker);
            }        
        });

        public void Init()
        {
            CurrentPlayer = 1;

            if (CurrentPlayer == 1)
                CurrentGo = PlayerOneColorChecker;
            else
                CurrentGo = PlayerTwoColorChecker;

            isMoving = false;
            prevChecker = null;

            map = new int[mapSize, mapSize] {
                 { 0,1,0,1,0,1,0,1},
                 { 1,0,1,0,1,0,1,0},
                 { 0,1,0,1,0,1,0,1},
                 { 0,0,0,0,0,0,0,0},
                 { 0,0,0,0,0,0,0,0},
                 { 2,0,2,0,2,0,2,0},
                 { 0,2,0,2,0,2,0,2},
                 { 2,0,2,0,2,0,2,0}
            };

            CreatMap();
        }
        public bool Load()
        {
            List<int> tmp = new List<int>();
            repository.Read(ref tmp, FirstPlayer, SecondPlayer, ref CurrentPlayer, PlayerOneColorChecker, PlayerTwoColorChecker);
            map = new int[8, 8];

            if (tmp.Count==64)
            {
                for (int i = 0, c = 0; i < 8; i++)                
                    for (int j = 0; j < 8; c++, j++)                    
                        map[i, j] = tmp[c];                    
                
                Checkers.Clear();
                isMoving = false;
                prevChecker = null;
                CreatMap();
                return true;
            }              
                return false;            
        }

        public void NewGame()
        {
            Checkers.Clear();
            Init();
        }
        public void ResetGame()
        {
            bool player1 = false;
            bool player2 = false;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 1)
                        player1 = true;
                    if (map[i, j] == 2)
                        player2 = true;
                }
            }


            if (player1==false)
            {
                var result =  MessageBox.Show(SecondPlayer + "  WIN!!\r\nDo you want to play a new game ?", "Congratulations" , MessageBoxButton.YesNo);

                NewGame();
                if (result != MessageBoxResult.Yes)
                {
                    messenger.Send(new NavigationMessage() { ViewModelType = typeof(HomeViewModel) });
                }
            }

            if (player2 == false)
            {
                var result = MessageBox.Show(FirstPlayer + "  WIN!!\r\nDo you want to play a new game ?", "Congratulations", MessageBoxButton.YesNo);

                NewGame();
                if (result != MessageBoxResult.Yes)
                {
                    messenger.Send(new NavigationMessage() { ViewModelType = typeof(HomeViewModel) });
                }
            }
        }
        public void CreatMap()
        {
            Checkers.Clear();

            if (CurrentPlayer == 1)
                CurrentGo = PlayerOneColorChecker;            
            else 
                CurrentGo = PlayerTwoColorChecker;             

            Checker checker;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    checker = new Checker();

                    if (map[i, j] == 1)
                        checker.Content = PlayerOneColorChecker;


                    else if (map[i, j] == 2)
                        checker.Content = PlayerTwoColorChecker;

                    checker.Background = GetPrevButtonColor(checker);
                    checker.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    checkers[i, j] = checker;

                    Checkers.Add(checker);
                }
            }
        }
        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;

            if (CurrentPlayer == 1)
            {
                CurrentGo = PlayerOneColorChecker;
            }
            else
            {
                CurrentGo = PlayerTwoColorChecker; ;
            }
            
            ResetGame();
        }
        public SolidColorBrush GetPrevButtonColor(Checker prevChecker)
        {
            int index = Checkers.IndexOf(prevChecker);

            int row = index / 8;
            int col = index % 8;


            if (row % 2 != 0)
                if (col % 2 == 0)
                    return new SolidColorBrush(Color.FromRgb(141, 140, 139));
                
            
            if (row % 2 == 0)
                if (col % 2 != 0)
                    return new SolidColorBrush(Color.FromRgb(141, 140, 139));
                
            
            return new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        private void OnFigurePress(Checker checker)
        {
            if (prevChecker != null)
            {
                prevChecker.Background = GetPrevButtonColor(prevChecker);
            }

            pressedChecker = checker;

            int index = Checkers.IndexOf(pressedChecker);

            int row = index / 8;
            int col = index % 8;

            if (map[row, col] != 0 && map[row, col] == CurrentPlayer)
            {
                CloseSteps();
                pressedChecker.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                DeactivateAllButtons();
                pressedChecker.IsEnabled = true;
                countEatSteps = 0;
                if (pressedChecker.Text.Equals("Damka"))
                    ShowSteps(row, col, false);
                else ShowSteps(row, col);



                if (isMoving)
                {
                    CloseSteps();
                    pressedChecker.Background = GetPrevButtonColor(pressedChecker);
                    ShowPossibleSteps();
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    isContinue = false;

                    int indexP = Checkers.IndexOf(prevChecker);

                    int rowP = indexP / 8;  
                    int colP = indexP % 8; 


                    if (Math.Abs(col - colP) > 1)
                    {
                        isContinue = true;
                        DeleteEaten(pressedChecker, prevChecker);
                    }

                    int tmp = map[row, col];
                    map[row, col] = map[rowP, colP];
                    map[rowP, colP] = tmp;
                    pressedChecker.Content = prevChecker.Content;
                    prevChecker.Content = null;
                    pressedChecker.Text = prevChecker.Text;
                    prevChecker.Text = "Damka";
                    SwitchButtonToCheat(pressedChecker);
                    countEatSteps = 0;
                    isMoving = false;
                    CloseSteps();
                    DeactivateAllButtons();

                    if (pressedChecker.Text.Equals("Damka"))
                        ShowSteps(row, col, false);
                    else ShowSteps(row, col);
                    if (countEatSteps == 0 || !isContinue)
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }
                    else if (isContinue)
                    {
                        pressedChecker.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        pressedChecker.IsEnabled = true;
                        isMoving = true;
                    }
                }
            }

            prevChecker = pressedChecker;
        }
        public void ShowPossibleSteps()
        {
            bool isOneStep = true;
            bool isEatStep = false;
            DeactivateAllButtons();
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == CurrentPlayer)
                    {
                        if (checkers[i, j].Text.Equals("Damka"))
                            isOneStep = false;
                        else isOneStep = true;
                        if (IsButtonHasEatStep(i, j, isOneStep, new int[2] { 0, 0 }))
                        {
                            isEatStep = true;
                            checkers[i, j].IsEnabled = true;
                        }
                    }
                }
            }
            if (!isEatStep)
                ActivateAllButtons();
        }
        public void SwitchButtonToCheat(Checker checker)
        {
            int index = Checkers.IndexOf(checker);

            int row = index / 8;
            int col = index % 8;

            if (map[row, col] == 1 && row == mapSize - 1)
            {
                checker.Text = "Damka";     
                checker.Content = SelectDamka(PlayerOneColorChecker);
            }
            if (map[row, col] == 2 && row == 0)
            {
                checker.Text = "Damka";
                checker.Content = SelectDamka(playerTwoColorChecker);
            }
        }

        public string SelectDamka(string playerChecker)
        {
            if (playerChecker == "Resources/white.png")
            {
                return "Resources/whiteDamka.png";
            }
            else if (playerChecker == "Resources/black.png")
            {
                return "Resources/blackDamka.png";
            }
            else if (playerChecker == "Resources/blackCLasik.png")
            {
                return "Resources/blackClasikDamka.png";
            }
            else 
            {
                return "Resources/redDamka.png";
            }
        }

        public void DeleteEaten(Checker endChecker, Checker startChecker)
        {
            int index = Checkers.IndexOf(endChecker);

            int row = index / 8; 
            int col = index % 8;  

            int indexp = Checkers.IndexOf(startChecker);

            int rowp = indexp / 8; 
            int colp = indexp % 8;  

            int count = Math.Abs(row - rowp);
            int startIndexX = row - rowp;
            int startIndexY = col - colp;
            startIndexX = startIndexX < 0 ? -1 : 1;
            startIndexY = startIndexY < 0 ? -1 : 1;
            int currCount = 0;
            int i = rowp + startIndexX;
            int j = colp + startIndexY;
            while (currCount < count - 1)
            {
                map[i, j] = 0;
                checkers[i, j].Content = null;
                checkers[i, j].Content = "";  
                i += startIndexX;
                j += startIndexY;
                currCount++;
            }

        }
        public void ShowSteps(int iCurrFigure, int jCurrFigure, bool isOnestep = true)
        {
            simpleSteps.Clear();
            ShowDiagonal(iCurrFigure, jCurrFigure, isOnestep);
            if (countEatSteps > 0)
                CloseSimpleSteps(simpleSteps);
        }
        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }

        public bool DeterminePath(int ti, int tj)
        {

            if (map[ti, tj] == 0 && !isContinue)
            {
                checkers[ti, tj].Background = new SolidColorBrush(Color.FromRgb(255, 255, 51));
                checkers[ti, tj].IsEnabled = true;
                simpleSteps.Add(checkers[ti, tj]);
            }
            else
            {
                if (map[ti, tj] != CurrentPlayer)
                {
                    if (pressedChecker.Text.Equals("Damka"))
                        ShowProceduralEat(ti, tj, false);
                    else ShowProceduralEat(ti, tj);
                }
                return false;
            }
            return true;
        }

        public void CloseSimpleSteps(List<Checker> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].Background = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].IsEnabled = false;
                }
            }
        }
        public void ShowProceduralEat(int i, int j, bool isOneStep = true)
        {

            int index = Checkers.IndexOf(pressedChecker);

            int row = index / 8;
            int col = index % 8; 


            int dirX = i - row;
            int dirY = j - col;
            dirX = dirX < 0 ? -1 : 1;
            dirY = dirY < 0 ? -1 : 1;
            int il = i;
            int jl = j;
            bool isEmpty = true;
            while (IsInsideBorders(il, jl))
            {
                if (map[il, jl] != 0 && map[il, jl] != CurrentPlayer)
                {
                    isEmpty = false;
                    break;
                }
                il += dirX;
                jl += dirY;

                if (isOneStep)
                    break;
            }
            if (isEmpty)
                return;
            List<Checker> toClose = new List<Checker>();
            bool closeSimple = false;
            int ik = il + dirX;
            int jk = jl + dirY;
            while (IsInsideBorders(ik, jk))
            {
                if (map[ik, jk] == 0)
                {
                    if (IsButtonHasEatStep(ik, jk, isOneStep, new int[2] { dirX, dirY }))
                    {
                        closeSimple = true;
                    }
                    else
                    {
                        toClose.Add(checkers[ik, jk]);
                    }
                    checkers[ik, jk].Background = new SolidColorBrush(Color.FromRgb(255, 255, 51));
                    checkers[ik, jk].IsEnabled = true;
                    countEatSteps++;
                }
                else break;
                if (isOneStep)
                    break;
                jk += dirY;
                ik += dirX;
            }
            if (closeSimple && toClose.Count > 0)
            {
                CloseSimpleSteps(toClose);
            }

        }

        public bool IsButtonHasEatStep(int IcurrFigure, int JcurrFigure, bool isOneStep, int[] dir)
        {
            bool eatStep = false;
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == -1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != CurrentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, j + 1))
                            eatStep = false;
                        else if (map[i - 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != CurrentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, j - 1))
                            eatStep = false;
                        else if (map[i - 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != CurrentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j - 1))
                            eatStep = false;
                        else if (map[i + 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == -1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != CurrentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j + 1))
                            eatStep = false;
                        else if (map[i + 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
            return eatStep;
        }

        public void CloseSteps()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    checkers[i, j].Background = GetPrevButtonColor(checkers[i, j]);
                }
            }
        }

        public bool IsInsideBorders(int ti, int tj)
        {
            if (ti >= mapSize || tj >= mapSize || ti < 0 || tj < 0)
            {
                return false;
            }
            return true;
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    checkers[i, j].IsEnabled = true;
                }
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    checkers[i, j].IsEnabled = false;
                }
            }
        }
    }
}
