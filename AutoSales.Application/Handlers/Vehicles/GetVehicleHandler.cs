using Microsoft.Extensions.Logging;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Responses;
using AutoSales.Application.Results;
using AutoSales.Application.Repositories;

namespace AutoSales.Application.Handlers.Vehicles;

public sealed class GetVehicleHandler : IRequestHandler<GetVehicleQuery, Result<VehicleResponse>>
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<GetVehicleHandler> _logger;

    public GetVehicleHandler(
        IVehicleRepository repository,
        ILogger<GetVehicleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<VehicleResponse>> Handle(GetVehicleQuery message, CancellationToken cancellationToken)
    {
        try
        {
            var vehicle = await _repository.FindAsync(message.Id, cancellationToken);

            if (vehicle is null)
            {
                _logger.LogWarning("Veículo {VehicleId} não encontrado para consulta.", message.Id);
                return Result<VehicleResponse>.Failure("Veículo não encontrado.", errorType: ErrorType.NotFound);
            }

            return Result<VehicleResponse>.Success(new VehicleResponse(vehicle.Id, vehicle.Plate, vehicle.ManufactureYear,
                vehicle.Brand, vehicle.Model, vehicle.PersonId));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao executar {Handler}", nameof(GetVehicleHandler));
            throw;
        }
    }
}
