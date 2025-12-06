using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallGeometry.Models;
using Autodesk.Revit.DB;
using System.ComponentModel.Design;
using Autodesk.Revit.UI;
using WallGeometry.Services;


namespace WallGeometry.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ExternalCommandData _commandData;
        public MainWindowViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            CalcWall = new RelayCommand(OnCalcWallExecute);
            Limit = 200;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private WallInfo _wallInfo;
        public WallInfo WallInfo
        {
            get { return _wallInfo; }
            set { _wallInfo = value;
                OnPropertyChanged();
            }
        }
        private string _statusmessage;
        public string Statusmessage
        {
            get { return _statusmessage; }
            set {_statusmessage = value;
                OnPropertyChanged();
            }
        }
        private double _limit;
        public double Limit
        {
            get { return _limit; }
            set {_limit = value;
                OnPropertyChanged();
            }
        }

        public ICommand CalcWall {  get;  }
        private void OnCalcWallExecute(object parameter)
        {
            SelectionService selectionService = new SelectionService(_commandData);
            Wall wall =selectionService.PickWall();
            if (wall != null)
            {
                GeometryService geometryService = new GeometryService();
                WallInfo = geometryService.GetWallInfo(wall, Limit);

                // Обновляем статус
                Statusmessage = WallInfo.IsCorrect
                    ? "Норма"
                    : "Превышение";
            }
            else
            {
                // Если пользователь отменил выбор
                WallInfo = null;
                Statusmessage = "Стена не выбрана";
            }
                       
        }

    }
}
