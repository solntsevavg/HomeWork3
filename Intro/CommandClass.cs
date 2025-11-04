using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Intro
{
    //КОММЕНТАРИИ. Сделала задачу не для стен, а для воздхуоводов. Родные мне они)
    [Transaction(TransactionMode.Manual)]
    public class DuctStatisticsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Получаем все воздуховоды
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> ductElements = collector
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .ToElements();

            // Обработка случая, когда воздуховодов нет
            if (ductElements.Count == 0)
            {
                TaskDialog.Show("Статистика воздуховодов", "В проекте не найдено воздуховодов.");
                return Result.Succeeded;
            }


            List<Duct> ducts = new List<Duct>(); //Список всех воздуховодов
            List<double> ductLengths = new List<double>(); //Список всех длин воздуховодов

           
                foreach (Element element in ductElements)
                {
                    Duct duct = element as Duct;
                    if (duct != null)
                    {
                        Parameter lengthParam = duct.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH); //Длина
                        if (lengthParam != null && lengthParam.HasValue)
                        {
                            double length = lengthParam.AsDouble();
                            ductLengths.Add(length);
                            ducts.Add(duct);
                        }
                    }
                }
           // Расчет статистики
            int totalDucts = ducts.Count;
            double maxLength = ductLengths.Max();
            double minLength = ductLengths.Min();
            double avgLength = ductLengths.Average();

            // Поиск самого длинного и самого короткого воздуховода
            Duct longestDuct = ducts[ductLengths.IndexOf(maxLength)];
            Duct shortestDuct = ducts[ductLengths.IndexOf(minLength)];
            // Транзакция только для изменения параметров
            using (Transaction t = new Transaction(doc, "Установка комментариев для воздуховодов"))
            {
                t.Start();

                // Установка комментариев
                Parameter longCommentParam = longestDuct.LookupParameter("Комментарии");
                if (longCommentParam != null && !longCommentParam.IsReadOnly)
                {
                    longCommentParam.Set("Самый длинный воздуховод");
                }

                Parameter shortCommentParam = shortestDuct.LookupParameter("Комментарии");
                if (shortCommentParam != null && !shortCommentParam.IsReadOnly)
                {
                    shortCommentParam.Set("Самый короткий воздуховод");
                }

                t.Commit();
            }
            // Форматирование отчета (конвертация из футов в миллиметры)
            string report = $"Статистика воздуховодов:\n\n" +
                           $"Общее количество: {totalDucts}\n" +
                           $"Самая большая длина: {maxLength * 304.8:F2} мм\n" +
                           $"Самая маленькая длина: {minLength * 304.8:F2} мм\n" +
                           $"Средняя длина: {avgLength * 304.8:F2} мм\n\n" +
                           $"Параметры обновлены для:\n" +
                           $"• Самый длинный воздуховод (ID: {longestDuct.Id})\n" +
                           $"• Самый короткий воздуховод (ID: {shortestDuct.Id})";

            
            // Вывод отчета в TaskDialog:cite[10]
            TaskDialog.Show("Отчет по воздуховодам", report);
            return Result.Succeeded;
        }
           public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
