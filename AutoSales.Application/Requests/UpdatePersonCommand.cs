using AutoSales.Application.Results;
using MediatR;

namespace AutoSales.Application.Requests;

public sealed record UpdatePersonCommand(int Id, PersonRequest Data) : IRequest<Result<bool>>;
