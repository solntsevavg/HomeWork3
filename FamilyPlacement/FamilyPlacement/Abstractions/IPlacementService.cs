using CSharpFunctionalExtensions;
using FamilyPlacement.Models;

namespace FamalyPlacment.Abstractions
{
    public interface IPlacementService
    {
        Result Place(FurnitureType selectedFurnitureType, int count);
    }
}
