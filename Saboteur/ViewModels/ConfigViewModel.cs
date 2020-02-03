using Saboteur.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Saboteur.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        public int MinPlayers { get => ConfigModel.minPlayers; }
        public int MaxPlayers { get => ConfigModel.maxPlayers; }

        private int numberOfPlayers;
        public int NumberOfPlayers
        {
            get => numberOfPlayers;
            set
            {
                numberOfPlayers = value;
                if (_playerInfoList != null)
                {
                    while (_playerInfoList.Count < numberOfPlayers)
                        _playerInfoList.Add(new PlayerEarlyInfomation("Player " + (_playerInfoList.Count+1), 0));
                    while (_playerInfoList.Count > numberOfPlayers)
                        _playerInfoList.RemoveAt(_playerInfoList.Count - 1);
                }
                RaisePropertyChanged("NumberOfPlayers");
                RaisePropertyChanged("PlayerInfoList");
            }
        }

        private List<PlayerEarlyInfomation> _playerInfoList { get; set; }
        public ObservableCollection<PlayerEarlyInfomation> PlayerInfoList
        {
            get => new ObservableCollection<PlayerEarlyInfomation>(_playerInfoList as List<PlayerEarlyInfomation>);
        }

        public ConfigViewModel()
        {
            NumberOfPlayers = MinPlayers;
            _playerInfoList = new List<PlayerEarlyInfomation>();
            for (int i = 1; i <= MinPlayers; i++)
                _playerInfoList.Add(new PlayerEarlyInfomation("Player " + i, 0));
        }
    }

    public class PlayerEarlyInfomation
    {
        public string Name { get; set; }
        public int GameMode { get; set; }

        public PlayerEarlyInfomation(string name, int mode)
        {
            Name = name;
            GameMode = mode;
        }
    }
}
