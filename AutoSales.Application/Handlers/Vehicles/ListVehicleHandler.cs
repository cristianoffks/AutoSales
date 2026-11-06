using Microsoft.Extensions.Logging;
using AutoSales.Application.Extensions;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Responses;
using AutoSales.Application.Repositories;
using AutoSales.Application.Results;

namespace AutoSales.Application.Handlers;

public sealed class ListVehicleHandler : IRequestHandler<ListVehicleQuery, Result<IReadOnlyList<VehicleResponse>>>
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<ListVehicleHandler> _logger;

    public ListVehicleHandler(
        IVehicleRepository repository,
        ILogger<ListVehicleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<VehicleResponse>>> Handle(ListVehicleQuery message, CancellationToken cancellationToken)
    {
        try
        {
            var search = message.Search;
            var plate = message.Plate;
            var personId = message.PersonId;
            if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(plate) && !personId.HasValue)
            {
                _logger.LogWarning("Pesquisa de veículos sem filtros.");
                return Result<IReadOnlyList<VehicleResponse>>.Failure(
                    "Informe pelo menos um filtro: marca/modelo, placa ou proprietário.", errorType: ErrorType.Validation);
            }


            var normalizedPlate = string.IsNullOrWhiteSpace(plate) ? null : plate.Trim().NormalizePlate();
            var vehicles = await _repository.ListAsync(search?.Trim(), normalizedPlate, personId, cancellationToken);
            var result = vehicles.Select(vehicle => new VehicleResponse(
                vehicle.Id, vehicle.Plate, vehicle.ManufactureYear,
                vehicle.Brand, vehicle.Model, vehicle.PersonId)).ToList();
            return Result<IReadOnlyList<VehicleResponse>>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao executar {Handler}", nameof(ListVehicleHandler));
            throw;
        }
    }
}
