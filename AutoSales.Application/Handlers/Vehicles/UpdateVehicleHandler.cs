using Microsoft.Extensions.Logging;
using AutoSales.Application.Extensions;
using AutoSales.Application.Results;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Repositories;
using FluentValidation;

namespace AutoSales.Application.Handlers;

public sealed class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Result<bool>>
{
    private readonly IVehicleRepository _repository;
    private readonly IValidator<VehicleRequest> _validator;
    private readonly ILogger<UpdateVehicleHandler> _logger;

    public UpdateVehicleHandler(
        IVehicleRepository repository,
        IValidator<VehicleRequest> validator,
        ILogger<UpdateVehicleHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateVehicleCommand message, CancellationToken cancellationToken)
    {
        try
        {
            var request = message.Data;
            var id = message.Id;
            var entity = await _repository.FindAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Veículo {VehicleId} não encontrado para atualização.", id);
                return Result<bool>.Failure("Veículo não encontrado.", errorType: ErrorType.NotFound);
            }

            await _validator.ValidateAndThrowAsync(new VehicleRequest(request.Plate, request.ManufactureYear,
                request.Brand, request.Model, entity.PersonId), cancellationToken);

            if (await _repository.ExistsByPlateAsync(request.Plate.NormalizePlate(), id, cancellationToken))
            {
                return Result<bool>.Failure("Já existe um veículo cadastrado com esta placa.", "Plate");
            }

            entity.Brand = request.Brand.Trim();
            entity.Model = request.Model.Trim();
            entity.Plate = request.Plate.NormalizePlate();
            entity.ManufactureYear = request.ManufactureYear;
            await _repository.UpdateAsync(entity, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao executar {Handler}", nameof(UpdateVehicleHandler));
            throw;
        }
    }


}
