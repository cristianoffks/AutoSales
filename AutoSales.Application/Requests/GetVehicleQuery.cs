using MediatR;
using AutoSales.Application.Responses;
using AutoSales.Application.Results;

namespace AutoSales.Application.Requests;

public sealed record GetVehicleQuery(int Id) : IRequest<Result<VehicleResponse>>;
