using td_revision.DTO;
using td_revision.Models;
using AutoMapper;

namespace td_revision.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {            
            CreateMap<Produit, ProduitDTO>().ReverseMap();
        
            CreateMap<TypeProduit, TypeProduitDTO>().ReverseMap();

            //mapping de  produit vers produitDetailDTO avec la règle pour le champs de rééapro
            CreateMap<Produit, ProduitDetailDTO>()
                .ForMember(dest => dest.IdProduit, opt => opt.MapFrom(src => src.IdProduit))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation.Nom))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation.Nom))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockReel))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.StockReel < src.StockMini));

            CreateMap<ProduitDetailDTO, Produit>()
                .ForMember(dest => dest.IdProduit, opt => opt.MapFrom(src => src.IdProduit))
                .ForMember(dest => dest.TypeProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.MarqueProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.StockMini, opt => opt.Ignore());

            CreateMap<Marque, MarqueDTO>()
                .ForMember(dest => dest.IdMarque, opt => opt.MapFrom(src => src.IdMarque))
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Nom))
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits.Count()));

            CreateMap<Image, ImageDTO>()
                .ForMember(dest => dest.IdImage, opt => opt.MapFrom(src => src.IdImage))
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomImage))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UrlPhoto))
                .ForMember(dest => dest.IdProduit, opt => opt.MapFrom(src => src.IdProduit));

            CreateMap<ImageDTO, Image>()
                .ForMember(dest => dest.IdImage, opt => opt.MapFrom(src => src.IdImage))
                .ForMember(dest => dest.NomImage, opt => opt.MapFrom(src => src.Nom))
                .ForMember(dest => dest.UrlPhoto, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.IdProduit, opt => opt.MapFrom(src => src.IdProduit))
                .ForMember(dest => dest.ProduitNavigation, opt => opt.Ignore());

        }
    }
}
