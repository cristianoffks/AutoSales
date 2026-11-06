namespace AutoSales.Application.Responses;

public sealed record VehicleResponse(
    int Id,
    string Plate,
    int ManufactureYear,
    string Brand,
    string Model,
    int PersonId);
