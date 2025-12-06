using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallGeometry.ViewModels;
using WallGeometry.Views;

namespace WallGeometry
{
    [Transaction(TransactionMode.Manual)]
    internal class Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(commandData);
           

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();


            return Result.Succeeded;
        }
    }
}
