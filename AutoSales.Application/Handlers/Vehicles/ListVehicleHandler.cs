using Microsoft.Extensions.Logging;
using AutoSales.Application.Extensions;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Responses;
using AutoSales.Application.Repositories;
using AutoSales.Application.Results;

namespace AutoSales.Application.Handlers.Vehicles;

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
            var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            var normalizedPlate = string.IsNullOrWhiteSpace(plate) ? null : plate.Trim().NormalizePlate();
            var vehicles = await _repository.ListAsync(normalizedSearch, normalizedPlate, personId, cancellationToken);

            var result = vehicles.Select(VehicleResponse.From).ToList();

            return Result<IReadOnlyList<VehicleResponse>>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao executar {Handler}", nameof(ListVehicleHandler));
            throw;
        }
    }
}
