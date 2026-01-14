using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
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
    public class PipeDistanceCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Запрашиваем выбор двух труб
                IList<Reference> selectedReferences = uiDoc.Selection.PickObjects(
                    ObjectType.Element,
                    new PipeSelectionFilter(),
                    "Выберите две трубы для расчета расстояния между ними");

                // Проверка выбора - убедиться, что выбраны именно две трубы
                if (selectedReferences.Count != 2)
                {
                    TaskDialog.Show("Ошибка", "Пожалуйста, выберите две трубы.");
                    return Result.Cancelled;
                }

                // Получение труб из ссылок
                var pipe1 = doc.GetElement(selectedReferences[0]) as Pipe;
                var pipe2 = doc.GetElement(selectedReferences[1]) as Pipe;

                if (pipe1 == null || pipe2 == null)
                {
                    TaskDialog.Show("Ошибка", "Выбранные элементы не являются трубами.");
                    return Result.Failed;
                }

                // Определение нормалей - вычисление векторов нормалей для обеих труб
                var normal1 = GetPipeNormal(pipe1);
                var normal2 = GetPipeNormal(pipe2);

                if (normal1 == null || normal2 == null)
                {
                    TaskDialog.Show("Ошибка", "Не удалось определить направление труб.");
                    return Result.Failed;
                }
                // Проверка параллельности с использованием скалярного произведения
                if (!ArePipesParallel(normal1, normal2))
                {
                    TaskDialog.Show("Ошибка", "Трубы не параллельны. Невозможно вычислить расстояние.");
                    return Result.Failed;
                }

                // Расчет расстояния
                var midpoint1 = GetPipeMidpoint(pipe1);
                var midpoint2 = GetPipeMidpoint(pipe2);

                var distance = CalculateDistanceBetweenPipes(midpoint1, midpoint2, normal1);

                // Конвертация из футов в миллиметры (1 фут = 304.8 мм)
                var distanceInMm = distance * 304.8;

                // Вывод результата
                string resultMessage = $"Расстояние между трубами: {distanceInMm:F2} мм\n\n" +
                                      $"Труба 1 (ID: {pipe1.Id}): {GetPipeInfo(pipe1)}\n" +
                                      $"Труба 2 (ID: {pipe2.Id}): {GetPipeInfo(pipe2)}";

                TaskDialog.Show("Результат расчета", resultMessage);

                return Result.Succeeded;
            }


            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // Пользователь нажал Esc или отменил выбор
                TaskDialog.Show("Статистика выбора", "Выбор отменен пользователем.");
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = $"Ошибка при выполнении команды: {ex.Message}";
                return Result.Failed;
            }
        }

        // Фильтр выбора только труб
        public class PipeSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is Pipe;
            }
            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

        // Определение нормали трубы
        private XYZ GetPipeNormal(Pipe pipe)
        {
            LocationCurve location = pipe.Location as LocationCurve;
            if (location == null) return null;

            Curve curve = location.Curve;
            XYZ start = curve.GetEndPoint(0);
            XYZ end = curve.GetEndPoint(1);

            return (end - start).Normalize();
        }

        // Проверка параллельности труб с использованием скалярного произведения
        private bool ArePipesParallel(XYZ normal1, XYZ normal2)
        {
            // Скалярное произведение для проверки параллельности: |n1·n2| ≈ 1
            double dotProduct = Math.Abs(normal1.DotProduct(normal2));

            // Допуск для учета погрешностей вычислений
            double tolerance = 0.001; // 0.1% отклонение

            return Math.Abs(dotProduct - 1.0) < tolerance;
        }

        // Нахождение середины трубы
        private XYZ GetPipeMidpoint(Pipe pipe)
        {
            if (pipe.Location is LocationCurve locationCurve)
            {
                Curve curve = locationCurve.Curve;
                if (curve != null)
                {
                    return curve.Evaluate(0.5, true); // Середина кривой
                }
            }
            return null;
        }

        // Расчет расстояния между трубами
        private double CalculateDistanceBetweenPipes(XYZ point1, XYZ point2, XYZ normal)
        {
            // Векторное вычитание для нахождения направления между трубами
            XYZ vectorBetween = point2 - point1;  //вектор от середины первой трубы до середины второй трубы


            double parallelLength = vectorBetween.DotProduct(normal); // длина компоненты вектора vectorBetween, которая параллельна направлению труб
            XYZ parallelComponent = normal * parallelLength; //иммет направление как у труб, имеет длину parallelLength
            XYZ perpendicularComponent = vectorBetween - parallelComponent; // perpendicularComponent = vectorBetween - (проекция vectorBetween на normal)

            // Длина перпендикулярной составляющей - это расстояние между осями
            double distance = perpendicularComponent.GetLength();
            return distance;
        }

        // Получение информации о трубе для отчета
        private string GetPipeInfo(Pipe pipe)
        {
            string info = $"Диаметр: {pipe.Diameter * 304.8:F1} мм";

            if (pipe.Location is LocationCurve locationCurve)
            {
                Curve curve = locationCurve.Curve;
                if (curve != null)
                {
                    double length = curve.Length * 304.8; // Конвертация в мм
                    info += $", Длина: {length:F0} мм";
                }
            }
            return info;
        }

    }
}


