using AutoMapper;
using ProductInventory.Models;
using ProductInventory.Contracts;

namespace ProductInventory.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Entity → DTO
            CreateMap<Product, ProductDto>();

            // DTO → Entity
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}