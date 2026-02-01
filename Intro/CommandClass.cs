using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace Intro
{
    [Transaction(TransactionMode.Manual)]
    public class SolidStatisticsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Выбор любого элемента
                Reference selectedReference = uiDoc.Selection.PickObject(
                    ObjectType.Element,
                    "Выберите любой элемент для анализа геометрии");

                Element element = doc.GetElement(selectedReference);

                if (element == null)
                {
                    TaskDialog.Show("Ошибка", "Не удалось получить элемент.");
                    return Result.Failed;
                }

                // Сбор статистики
                int solidCount = 0;  //количество солидов
                double totalVolume = 0; //суммарный объем
                double totalArea = 0; //суммарная площадь поверхностей
                int faceCount = 0; //количество граней
                int edgeCount = 0; //количество ребер
                double totalEdgeLength = 0; //суммарная длина ребер

                // Получаем геометрию
                Options options = new Options();
                options.DetailLevel = ViewDetailLevel.Fine;

                GeometryElement geometry = element.get_Geometry(options);

                if (geometry != null)
                {
                    foreach (GeometryObject geoObj in geometry)
                    {
                        if (geoObj is Solid solid && solid.Volume > 0)
                        {
                            solidCount++;
                            totalVolume += solid.Volume;

                            foreach (Face face in solid.Faces)
                            {
                                faceCount++;
                                try { totalArea += face.Area; } catch { }

                                foreach (EdgeArray edgeLoop in face.EdgeLoops)
                                {
                                    foreach (Edge edge in edgeLoop)
                                    {
                                        edgeCount++;
                                        try
                                        {
                                            Curve curve = edge.AsCurve();
                                            if (curve != null)
                                                totalEdgeLength += curve.Length;
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }

                // Вывод результатов
                string report = $"ГЕОМЕТРИЯ ЭЛЕМЕНТА:\n\n" +
                               $"Категория: {element.Category?.Name}\n" +
                               $"Тип: {element.GetType().Name}\n\n" +
                               $"Solid-ы: {solidCount}\n" +
                               $"Объем: {totalVolume * 1e9:F0} см³\n" +
                               $"Площадь: {totalArea * 1e6:F0} см²\n" +
                               $"Грани: {faceCount}\n" +
                               $"Ребра: {edgeCount}\n" +
                               $"Длина ребер: {totalEdgeLength * 1000:F0} мм";

                TaskDialog.Show("Геометрия элемента", report);

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                TaskDialog.Show("Отмена", "Выбор отменен.");
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = $"Ошибка: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}


