using AutoMapper;
using Application.DTOs.Branch;
using Application.DTOs.Car;
using Application.DTOs.Customer;
using Application.DTOs.Rental;
using Domain.Entities;
namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
     public InfrastructureProfile()
    {
        CreateMap<Branch, GetBranchDTO>();
        CreateMap<CreateBranchDTO, Branch>();
        CreateMap<UpdateBranchDTO, Branch>();

        CreateMap<Car, GetCarDTO>();
        CreateMap<CreateCarDTO, Car>();
        CreateMap<UpdateCarDTO, Car>();

        CreateMap<Customer, GetCustomerDTO>();
        CreateMap<CreateCustomerDTO, Customer>();
        CreateMap<UpdateCustomerDTO, Customer>();

        CreateMap<Rental, GetRentalDTO>()
          .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.Cars.PricePerDay));
        CreateMap<CreateRentalDTO, Rental>();
        CreateMap<UpdateRentalDTO, Rental>();
    }
}
