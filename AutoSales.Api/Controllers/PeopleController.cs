using AutoSales.Api.Responses;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AutoSales.Api.Controllers;

[ApiController]
[Route("api/people")]
public sealed class PeopleController : ControllerBase
{
    private readonly ISender _mediator;

    public PeopleController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Person>>> List(
        [FromQuery] string? name, [FromQuery] string? cpf, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListPersonQuery(name, cpf), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Dados inválidos",
                Detail = result.Message,
                Instance = Request.Path
            });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Person>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<Person>> Create(PersonRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePersonCommand(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PersonRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdatePersonCommand(id, request), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePersonCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return NoContent();
    }
}
