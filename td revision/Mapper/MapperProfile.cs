using td_revision.DTO;
using td_revision.Models;
using AutoMapper;

namespace td_revision.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // ========== TYPEPRODUIT ==========
            CreateMap<TypeProduit, TypeProduitDTO>()
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits != null ? src.Produits.Count : 0));

            CreateMap<TypeProduitDTO, TypeProduit>()
                .ForMember(dest => dest.IdTypeProduit, opt => opt.Ignore())
                .ForMember(dest => dest.Produits, opt => opt.Ignore());

            // ========== MARQUE ==========
            CreateMap<Marque, MarqueDTO>()
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits != null ? src.Produits.Count : 0));

            CreateMap<MarqueDTO, Marque>()
                .ForMember(dest => dest.IdMarque, opt => opt.Ignore())
                .ForMember(dest => dest.Produits, opt => opt.Ignore());

            // ========== PRODUIT -> PRODUITDETAILDTO (lecture) ==========
            CreateMap<Produit, ProduitDetailDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.Nom : null))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation != null ? src.MarqueProduitNavigation.Nom : null))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockReel))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.StockReel < src.StockMini));

            // ========== PRODUITDETAILDTO -> PRODUIT (création) ==========
            CreateMap<ProduitDetailDTO, Produit>()
                .ForMember(dest => dest.IdProduit, opt => opt.Ignore())
                .ForMember(dest => dest.StockReel, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.StockMini, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StockMaxi, opt => opt.MapFrom(src => 100))
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.MarqueProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.TypeProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdMarque, opt => opt.Ignore())
                .ForMember(dest => dest.IdTypeProduit, opt => opt.Ignore());

            // ========== PRODUIT -> PRODUITDTO (lecture simple) ==========
            CreateMap<Produit, ProduitDTO>()
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation.Nom))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation.Nom));

            CreateMap<ProduitDTO, Produit>()
                .ForMember(dest => dest.IdProduit, opt => opt.Ignore())
                .ForMember(dest => dest.IdMarque, opt => opt.Ignore())
                .ForMember(dest => dest.IdTypeProduit, opt => opt.Ignore())
                .ForMember(dest => dest.StockMini, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StockMaxi, opt => opt.MapFrom(src => 100))
                .ForMember(dest => dest.MarqueProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.TypeProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            // ========== IMAGE ==========
            CreateMap<Image, ImageDTO>()
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomImage))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UrlPhoto));

            CreateMap<ImageDTO, Image>()
                .ForMember(dest => dest.IdImage, opt => opt.Ignore())
                .ForMember(dest => dest.NomImage, opt => opt.MapFrom(src => src.Nom))
                .ForMember(dest => dest.UrlPhoto, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.ProduitNavigation, opt => opt.Ignore());
        }
    }
}