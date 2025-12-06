using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallGeometry.Models;

namespace WallGeometry.Services
{
    public class GeometryService
    {

        public object UnitTypeId { get; private set; }

        public WallInfo GetWallInfo(Wall wall, double limit)
        {
            if (wall == null) return null;

            var doc = wall.Document;
            var wallType = wall.WallType;

            // Получаем параметры стены
            double thickness = wallType.Width;
            Parameter heightParam = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
            double height = heightParam?.AsDouble() ?? 0;

            // Получаем площадь (вычисленную Revit)
            Parameter areaParam = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
            double area = areaParam?.AsDouble() ?? 0;

            // Получаем объем через геометрию
            double volume = GetWallVolume(wall);

            // Рассчитываем длину (приблизительно)
            double length = (area > 0 && height > 0) ? (area / (2 * height)) : 0;

            // Конвертируем из внутренних единиц Revit
            thickness = UnitUtils.ConvertFromInternalUnits(thickness, DisplayUnitType.DUT_MILLIMETERS);
            height = UnitUtils.ConvertFromInternalUnits(height, DisplayUnitType.DUT_MILLIMETERS);
            length = UnitUtils.ConvertFromInternalUnits(length, DisplayUnitType.DUT_MILLIMETERS);
            area = UnitUtils.ConvertFromInternalUnits(area, DisplayUnitType.DUT_SQUARE_METERS);
            volume = UnitUtils.ConvertFromInternalUnits(volume, DisplayUnitType.DUT_CUBIC_METERS);

            return new WallInfo()
            {
                Name = wall.Name,
                Type = wallType.Name,
                Length = Math.Round(length, 0),
                Height = Math.Round(height, 0),
                Thickness = Math.Round(thickness, 0),
                Volume = Math.Round(volume, 2),
                Area = Math.Round(area, 2),
                Distance = thickness,
                IsCorrect = thickness <= limit
            };
        }
                private double GetWallVolume(Wall wall)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            GeometryElement geomElem = wall.get_Geometry(options);

            if (geomElem == null) return 0;

            double totalVolume = 0;

            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is Solid solid && solid.Volume > 0)
                {
                    totalVolume += Math.Abs(solid.Volume);
                }
            }


            return totalVolume;
        }
    }
}
