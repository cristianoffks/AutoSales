using AutoSales.Application.Results;
using MediatR;
using AutoSales.Application.Entities;

namespace AutoSales.Application.Requests;

public sealed record CreatePersonCommand(PersonRequest Data) : IRequest<Result<Person>>;
