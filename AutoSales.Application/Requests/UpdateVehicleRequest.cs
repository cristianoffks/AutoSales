namespace AutoSales.Application.Requests;

public sealed record UpdateVehicleRequest(
    string Plate,
    int ManufactureYear,
    string Brand,
    string Model);
