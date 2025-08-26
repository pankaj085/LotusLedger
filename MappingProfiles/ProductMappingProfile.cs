using AutoMapper;
using ProductInventory.Contracts;
using ProductInventory.Models;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();

        // For update, ignore nulls so only provided fields overwrite
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null
            ));
    }
}
