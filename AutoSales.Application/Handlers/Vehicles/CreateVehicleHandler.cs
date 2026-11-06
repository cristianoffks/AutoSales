using Microsoft.Extensions.Logging;
using AutoSales.Application.Extensions;
using AutoSales.Application.Results;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Responses;
using AutoSales.Application.Entities;
using AutoSales.Application.Repositories;
using FluentValidation;

namespace AutoSales.Application.Handlers.Vehicles;

public sealed class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, Result<VehicleResponse>>
{
    private readonly IVehicleRepository _repository;
    private readonly IValidator<VehicleRequest> _validator;
    private readonly IPersonRepository _people;
    private readonly ILogger<CreateVehicleHandler> _logger;

    public CreateVehicleHandler(
        IVehicleRepository repository,
        IValidator<VehicleRequest> validator,
        IPersonRepository people,
        ILogger<CreateVehicleHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _people = people;
        _logger = logger;
    }

    public async Task<Result<VehicleResponse>> Handle(CreateVehicleCommand message, CancellationToken cancellationToken)
    {
        try
        {
            var request = message.Data;
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var owner = await _people.FindAsync(request.PersonId, cancellationToken);
            if (owner == null)
            {
                _logger.LogWarning("Proprietário {PersonId} não encontrado para cadastro do veículo.", request.PersonId);
                return Result<VehicleResponse>.Failure("Proprietário não encontrado.", nameof(request.PersonId), ErrorType.NotFound);
            }

            if (await _repository.ExistsByPlateAsync(request.Plate.NormalizePlate(), null, cancellationToken))
            {
                return Result<VehicleResponse>.Failure("Já existe um veículo cadastrado com esta placa.", "Plate");
            }

            var entity = new Vehicle
            {
                Plate = request.Plate.NormalizePlate(),
                ManufactureYear = request.ManufactureYear,
                Brand = request.Brand.Trim(),
                Model = request.Model.Trim(),
                PersonId = request.PersonId
            };

            await _repository.AddAsync(entity, cancellationToken);
            return Result<VehicleResponse>.Success(ToResponse(entity));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao executar {Handler}", nameof(CreateVehicleHandler));
            throw;
        }
    }

    private static VehicleResponse ToResponse(Vehicle vehicle)
    {
        return new VehicleResponse(vehicle.Id, vehicle.Plate, vehicle.ManufactureYear,
            vehicle.Brand, vehicle.Model, vehicle.PersonId);
    }
}
