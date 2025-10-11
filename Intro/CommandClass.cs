using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Intro
{
    //[Transaction(TransactionMode.Manual)]
    //public class CommandClass : IExternalCommand
    //{

    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        TaskDialog.Show("Внимание", "Привет!");
    //        return Result.Succeeded;
    //    }
    //}

    [Transaction(TransactionMode.Manual)]
    public class ApplicationClass : IExternalApplication
    {
        
            public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("ПИК-Привет");
            var panel = application.CreateRibbonPanel("ПИК-Привет", "Общее");
            var button = new PushButtonData(
                "Hello",
                "Привет",
                "C:\\Users\\solntsevavg\\Downloads\\Software Development Kit\\Samples\\VisibilityControl\\CS\\bin\\Debug\\VisibilityControl.dll",
                "Revit.SDK.Samples.VisibilityControl.CS.Command"
                );
            BitmapImage bitmapImage = new BitmapImage(new Uri("C:\\Users\\solntsevavg\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2019\\Test\\img\\ParameterTransfer32.png", UriKind.Absolute));
            button.LargeImage = bitmapImage;
            panel.AddItem(button);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
