using Microsoft.AspNetCore.Mvc;
using TravelAgencyApi.Models;
using TravelAgencyApi.Services;

namespace TravelAgencyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _service;

    public TripsController(ITripsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips() => Ok(await _service.GetTripsAsync());

    [HttpGet("clients/{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        var trips = await _service.GetTripsByClientIdAsync(id);
        return trips.Any() ? Ok(trips) : NotFound();
    }

    [HttpPost("clients")]
    public async Task<IActionResult> AddClient([FromBody] NewClientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var id = await _service.AddClientAsync(dto);
        return CreatedAtAction(nameof(GetClientTrips), new { id }, new { id });
    }

    [HttpPut("clients/{id}/trips/{tripId}")]
    public async Task<IActionResult> Register(int id, int tripId)
    {
        var success = await _service.RegisterClientToTripAsync(id, tripId);
        return success ? Ok() : Conflict();
    }

    [HttpDelete("clients/{id}/trips/{tripId}")]
    public async Task<IActionResult> Unregister(int id, int tripId)
    {
        var success = await _service.UnregisterClientFromTripAsync(id, tripId);
        return success ? Ok() : NotFound();
    }
}