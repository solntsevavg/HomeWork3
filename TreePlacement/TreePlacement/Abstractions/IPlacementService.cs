using System;
using System.Collections.Generic;
using System.Text;

using CSharpFunctionalExtensions;
using TreePlacement.Models;

namespace FamalyPlacment.Abstractions
{
    public interface IPlacementService
    {
        Result Place(FurnitureType selectedFurnitureType, int count);
    }
}
