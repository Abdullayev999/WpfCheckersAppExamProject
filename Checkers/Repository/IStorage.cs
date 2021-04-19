using System.Collections.Generic;

namespace Checkers.Repository
{
    public interface IStorage
    {
        void Read(ref List<int> map, string name, string name2, ref int currentPlayyer, string playerOneColorChecker, string playerTwoColorChecker);
        void Save(List<int> map, string name, string name2, int currentPlayer, string playerOneColorChecker, string playerTwoColorChecker);
    }
}