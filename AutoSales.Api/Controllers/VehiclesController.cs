using AutoSales.Api.Responses;
using MediatR;
using AutoSales.Application.Requests;
using AutoSales.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoSales.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController : ControllerBase
{
    private readonly ISender _mediator;

    public VehiclesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehicleResponse>>> List(
        [FromQuery] string? search, [FromQuery] string? plate, [FromQuery] int? personId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListVehicleQuery(search, plate, personId), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleResponse>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVehicleQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(VehicleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateVehicleCommand(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVehicleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateVehicleCommand(id, request), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteVehicleCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return ErrorResponse.Create(result, Request.Path);
        }

        return NoContent();
    }
}
