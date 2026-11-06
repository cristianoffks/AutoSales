namespace AutoSales.Application.Requests;

public sealed record VehicleRequest(
    string Plate,
    int ManufactureYear,
    string Brand,
    string Model,
    int PersonId);
