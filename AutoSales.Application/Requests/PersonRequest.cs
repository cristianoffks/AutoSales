namespace AutoSales.Application.Requests;

public sealed record PersonRequest(
    string FullName,
    string Cpf,
    DateOnly? BirthDate,
    string Email,
    string Phone);
