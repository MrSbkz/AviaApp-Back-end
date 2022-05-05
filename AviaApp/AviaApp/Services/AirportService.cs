using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AviaApp.Models.Dto;
using AviaApp.Models.Requests;
using AviaApp.Services.Contracts;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AviaApp.Services;

public class AirportService : IAirportService
{
    private readonly AviaAppDbContext _context;
    private readonly IMapper _mapper;

    public AirportService(AviaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<AirportDto>> GetAirportsAsync(Guid cityId)
    {
        return _mapper.Map<IList<AirportDto>>(await _context.Airports.Where(x => x.CityId == cityId)
            .Include(x => x.City.Country).ToListAsync());
    }

    public async Task<IList<AirportDto>> GetAllAirportsAsync()
    {
        return _mapper.Map<IList<AirportDto>>(await _context.Airports.Include(x => x.City.Country).ToListAsync());
    }

    public async Task<AirportDto> GetAirportByIdAsync(Guid airportId)
    {
        var airport = await GetAirportIfExistsAsync(airportId);
        return _mapper.Map<AirportDto>(airport);
    }

    public async Task<AirportDto> AddAirportAsync(AddAirportRequest request)
    {
        await CheckAirportName(request.AirportName, request.CityId);

        var airport = new Airport
        {
            Id = new Guid(),
            Name = request.AirportName,
            CityId = request.CityId,
        };
        await _context.Airports.AddAsync(airport);
        await _context.SaveChangesAsync();

        return _mapper.Map<AirportDto>(await _context.Airports.Include(x => x.City.Country)
            .FirstAsync(x => x.Id.Equals(airport.Id)));
    }

    public async Task UpdateAirportNameAsync(UpdateAirportRequest request)
    {
        var existingAirport = await GetAirportIfExistsAsync(request.AirportId);
        await CheckAirportName(request.UpdatedAirportName, existingAirport.City.Id);

        existingAirport.Name = request.UpdatedAirportName;

        _context.Airports.Update(existingAirport);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAirportAsync(Guid airportId)
    {
        var airport = await GetAirportIfExistsAsync(airportId);
        _context.Airports.Remove(airport);
        await _context.SaveChangesAsync();
    }

    private async Task<Airport> GetAirportIfExistsAsync(Guid airportId)
    {
        var airport = await _context.Airports.Include(x => x.City.Country).FirstOrDefaultAsync(x => x.Id == airportId);
        if (airport == null)
            throw new Exception("This airport has not been found");

        return airport;
    }

    private async Task CheckAirportName(string airportName, Guid cityId)
    {
        var airport = await _context.Airports.FirstOrDefaultAsync(x =>
            x.Name == airportName && x.City.Id == cityId);

        if (airport != null)
            throw new Exception($"The airport {airportName} already exists");
    }
}