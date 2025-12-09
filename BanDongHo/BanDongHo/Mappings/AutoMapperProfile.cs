using AutoMapper;
using WatchAPI.DTOs;
using WatchAPI.DTOs.Watch;
using WatchAPI.Models.Entities;

namespace WatchAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // get Role from UserManager separately

            // Auth
            CreateMap<User, AuthResponseDTO>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()) 
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());


            // Watch
            CreateMap<Watch, WatchUserDTO>();

            CreateMap<Watch, WatchAdminDTO>();

            CreateMap<WatchCreateDTO, Watch>();

            CreateMap<WatchUpdateDTO, Watch>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // ImageUrl is set after uploading the image


            // CartItem to CartItemDTO
            CreateMap<CartItem, CartItemDTO>()
                .ForMember(dest => dest.WatchName, opt => opt.MapFrom(src => src.Watch.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Watch.ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Watch.Price))
                .ForMember(dest => dest.Total, opt => opt.Ignore()); 

            // Invoice to InvoiceDTO
            CreateMap<Invoice, InvoiceDTO>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.InvoiceDetails))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

            // InvoiceDetail to InvoiceDetailDTO
            CreateMap<InvoiceDetail, InvoiceDetailDTO>()
                .ForMember(dest => dest.WatchName, opt => opt.MapFrom(src => src.Watch.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Watch.ImageUrl))
                .ForMember(dest => dest.Total, opt => opt.Ignore()); 
        }
    }
}
