using AutoSales.Application.Results;
using MediatR;
using AutoSales.Application.Responses;

namespace AutoSales.Application.Requests;

public sealed record CreateVehicleCommand(VehicleRequest Data) : IRequest<Result<VehicleResponse>>;
