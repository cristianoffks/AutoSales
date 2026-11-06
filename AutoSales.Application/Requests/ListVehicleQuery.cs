using MediatR;
using AutoSales.Application.Responses;
using AutoSales.Application.Results;

namespace AutoSales.Application.Requests;

public sealed record ListVehicleQuery(string? Search, string? Plate, int? PersonId) : IRequest<Result<IReadOnlyList<VehicleResponse>>>;
