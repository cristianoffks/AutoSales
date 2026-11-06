using AutoSales.Application.Results;
using MediatR;

namespace AutoSales.Application.Requests;

public sealed record UpdateVehicleCommand(int Id, UpdateVehicleRequest Data) : IRequest<Result<bool>>;
