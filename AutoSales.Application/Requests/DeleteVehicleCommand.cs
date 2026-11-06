using MediatR;
using AutoSales.Application.Results;

namespace AutoSales.Application.Requests;

public sealed record DeleteVehicleCommand(int Id) : IRequest<Result<bool>>;
