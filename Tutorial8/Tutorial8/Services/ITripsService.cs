using TravelAgencyApi.Models;

namespace TravelAgencyApi.Services;

public interface ITripsService
{
    Task<IEnumerable<TripDto>> GetTripsAsync();
    Task<IEnumerable<TripDto>> GetTripsByClientIdAsync(int clientId);
    Task<int> AddClientAsync(NewClientDto dto);
    Task<bool> RegisterClientToTripAsync(int clientId, int tripId);
    Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId);
}