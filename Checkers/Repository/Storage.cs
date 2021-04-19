using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Checkers.Repository
{
    public class Storage : IStorage
    {
        public string CurrentPlayerPath { get; set; } = "currentPlayer.txt";
        public string MapPath { get; set; } = "map.json";
        public string SecondPlayerPath { get; set; } = "firstplayer.txt";
        public string FirstPlayerPath { get; set; } = "secondplayer.txt";
        public string FirstColor { get; set; } = "firstColor.txt";
        public string SecondColor { get; set; } = "secondColor.txt";
        public void Save(List<int> map, string name, string name2, int currentPlayer, string playerOneColorChecker, string playerTwoColorChecker)
        {
            SaveMap(map);
            SaveNames(name, name2);
            SaveCurrentPlayer(currentPlayer);
            SaveColor(playerOneColorChecker, playerTwoColorChecker);
        }

        public void SaveColor(string playerOneColorChecker, string playerTwoColorChecker)
        {
            File.WriteAllText(FirstColor, playerOneColorChecker);
            File.WriteAllText(SecondColor, playerTwoColorChecker);
        }
        public void ReadCurrentPlayer(ref int currentPlayer)
        {
            if (File.Exists(CurrentPlayerPath))
                currentPlayer = Convert.ToInt32(File.ReadAllText(CurrentPlayerPath));

        }
        public void SaveCurrentPlayer(int currentPlayer)
        {
            File.WriteAllText(CurrentPlayerPath, currentPlayer.ToString());
        }
        public void ReadColor(string playerOneColorChecker, string playerTwoColorChecker)
        {
            if (File.Exists(FirstColor))
                playerTwoColorChecker = File.ReadAllText(FirstColor);


            if (File.Exists(SecondColor))
                playerTwoColorChecker = File.ReadAllText(SecondColor);

        }
        public void SaveNames(string name, string name2)
        {
            File.WriteAllText(FirstPlayerPath, name);
            File.WriteAllText(SecondPlayerPath, name2);
        }
        public void ReadNames(string name, string name2)
        {
            if (File.Exists(FirstPlayerPath))
                name = File.ReadAllText(FirstPlayerPath);


            if (File.Exists(SecondPlayerPath))
                name2 = File.ReadAllText(SecondPlayerPath);

        }
        public void SaveMap(List<int> map)
        {
            var json = JsonSerializer.Serialize(map);
            File.WriteAllText(MapPath, json);
        }
        public void ReadMap(ref List<int> map)
        {
            if (File.Exists(MapPath))
            {
                var json = File.ReadAllText(MapPath);
                map = JsonSerializer.Deserialize<List<int>>(json);
            }
        }

        public void Read(ref List<int> map, string name, string name2, ref int currentPlayyer, string playerOneColorChecker, string playerTwoColorChecker)
        {
            ReadMap(ref map);
            ReadNames(name, name2);
            ReadCurrentPlayer(ref currentPlayyer);
            ReadColor(playerOneColorChecker, playerTwoColorChecker);
        }

    }
}
