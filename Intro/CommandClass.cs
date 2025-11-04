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
    public class SelectionStatisticsCommand : IExternalCommand
    {

        public class DuctSystemsSelectionFilter : ISelectionFilter
        {
            private readonly List<BuiltInCategory> allowedCategories = new List<BuiltInCategory>
    {
                BuiltInCategory.OST_DuctAccessory,         // Арматура воздуховодов
        BuiltInCategory.OST_DuctTerminal,         // Воздухораспределители
        BuiltInCategory.OST_MechanicalEquipment   // Оборудование
    };

            public bool AllowElement(Element elem)
            {
                // Проверяем, что элемент - FamilyInstance и принадлежит разрешенной категории
                if (elem is FamilyInstance familyInstance)
                {
                    return allowedCategories.Contains((BuiltInCategory)familyInstance.Category.Id.IntegerValue);
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Запрашиваем выбор элементов у пользователя
                IList<Reference> selectedReferences = uiDoc.Selection.PickObjects(
                    ObjectType.Element,
                    new DuctSystemsSelectionFilter(),
                    "Выберите элементы систем вентиляции");

                // Собираем элементы из ссылок
                List<Element> selectedElements = new List<Element>();
                foreach (Reference reference in selectedReferences)
                {
                    Element element = doc.GetElement(reference);
                    if (element != null)
                    {
                        selectedElements.Add(element);
                    }
                }

                // Создаем Dictionary для подсчета по категориям
                Dictionary<string, int> categoryCount = new Dictionary<string, int>();

                // Подсчитываем общее количество и распределение по категориям
                int totalCount = selectedElements.Count;

                foreach (Element element in selectedElements)
                {
                    Category category = element.Category;
                    if (category != null)
                    {
                        string categoryName = category.Name;

                        if (categoryCount.ContainsKey(categoryName))
                        {
                            categoryCount[categoryName]++;
                        }
                        else
                        {
                            categoryCount.Add(categoryName, 1);
                        }
                    }
                }

                // Формируем отчет
                string report = $"СТАТИСТИКА ВЫБРАННЫХ ЭЛЕМЕНТОВ\n\n";
                report += $"Общее количество элементов: {totalCount}\n\n";
                report += "Распределение по категориям:\n";
                foreach (var category in categoryCount.OrderByDescending(x => x.Value))
                {
                    report += $"  {category.Key} → {category.Value} элементов\n";
                }

                // Дополнительная информация о типах элементов
                report += $"\nТипы выбранных элементов:\n";
                var elementTypes = selectedElements
                    .Select(e => e.GetType().Name)
                    .Distinct()
                    .ToList();

                foreach (string typeName in elementTypes)
                {
                    int typeCount = selectedElements.Count(e => e.GetType().Name == typeName);
                    report += $"  {typeName}: {typeCount} элементов\n";
                }

                // Выводим отчет в TaskDialog
                TaskDialog.Show("Статистика выбора", report);

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
    }
}


