using Microsoft.Data.SqlClient;
using TravelAgencyApi.Models;

namespace TravelAgencyApi.Services;

public class TripsService : ITripsService
{
    private readonly IConfiguration _configuration;

    public TripsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<TripDto>> GetTripsAsync()
    {
        var trips = new List<TripDto>();
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name as Country
            FROM Trip t
            JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            JOIN Country c ON c.IdCountry = ct.IdCountry", connection);

        var reader = await command.ExecuteReaderAsync();
        var tripDict = new Dictionary<int, TripDto>();

        while (await reader.ReadAsync())
        {
            var id = (int)reader["IdTrip"];
            if (!tripDict.TryGetValue(id, out var trip))
            {
                trip = new TripDto
                {
                    IdTrip = id,
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    DateFrom = (DateTime)reader["DateFrom"],
                    DateTo = (DateTime)reader["DateTo"],
                    MaxPeople = (int)reader["MaxPeople"]
                };
                tripDict[id] = trip;
            }
            trip.Countries.Add(reader["Country"].ToString());
        }
        return tripDict.Values;
    }

    public async Task<IEnumerable<TripDto>> GetTripsByClientIdAsync(int clientId)
    {
        // Implementacja analogiczna, z JOINami z Client_Trip i Trip itd.
        return new List<TripDto>();
    }

    public async Task<int> AddClientAsync(NewClientDto dto)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            OUTPUT INSERTED.IdClient
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection);

        command.Parameters.AddWithValue("@FirstName", dto.FirstName);
        command.Parameters.AddWithValue("@LastName", dto.LastName);
        command.Parameters.AddWithValue("@Email", dto.Email);
        command.Parameters.AddWithValue("@Telephone", dto.Telephone);
        command.Parameters.AddWithValue("@Pesel", dto.Pesel);

        return (int)await command.ExecuteScalarAsync();
    }

    public async Task<bool> RegisterClientToTripAsync(int clientId, int tripId)
    {
        // Sprawdzenie limitów, obecności itd. + insert do Client_Trip
        return true;
    }

    public async Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId)
    {
        // Delete z Client_Trip
        return true;
    }
}