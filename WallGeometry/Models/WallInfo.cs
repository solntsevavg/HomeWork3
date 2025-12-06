using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallGeometry.Models
{
    public class WallInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Length { get;  set; }
        public double Height { get;  set; }
        public double Thickness { get;  set; }
        public double Volume { get;  set; }
        public double Area { get;  set; }
        public double Distance { get;  set; }
        public bool IsCorrect { get;  set; }
    }

}
