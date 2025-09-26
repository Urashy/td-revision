using td_revision.DTO;
using td_revision.Models;
using AutoMapper;

namespace td_revision.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<TypeProduit, TypeProduitDTO>()
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits != null ? src.Produits.Count : 0));

            CreateMap<TypeProduitDTO, TypeProduit>()
                .ForMember(dest => dest.Produits, opt => opt.Ignore())
                .ForMember(dest => dest.IdTypeProduit, opt => opt.MapFrom(src => src.IdTypeProduit))
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Nom));

            // Mapping simple pour Marque avec ReverseMap
            CreateMap<Marque, MarqueDTO>()
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits != null ? src.Produits.Count : 0))
                .ReverseMap()
                .ForMember(dest => dest.Produits, opt => opt.Ignore()); // Ignorer lors de la création

            // Mapping de produit vers produitDetailDTO (lecture seule pour l'instant)
            CreateMap<Produit, ProduitDetailDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.Nom : null))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation != null ? src.MarqueProduitNavigation.Nom : null))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockReel))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.StockReel < src.StockMini));

            // Mapping inverse pour ProduitDetailDTO - NE PAS MAPPER L'ID
            CreateMap<ProduitDetailDTO, Produit>()
                .ForMember(dest => dest.IdProduit, opt => opt.Ignore()) // Important : ignorer l'ID
                .ForMember(dest => dest.StockReel, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.TypeProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.MarqueProduitNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.StockMini, opt => opt.MapFrom(src => 0)) // Valeur par défaut
                .ForMember(dest => dest.StockMaxi, opt => opt.MapFrom(src => 100)) // Valeur par défaut
                .ForMember(dest => dest.IdMarque, opt => opt.Ignore()) // Sera résolu par la logique métier
                .ForMember(dest => dest.IdTypeProduit, opt => opt.Ignore()); // Sera résolu par la logique métier

            // Mapping simple pour Image
            CreateMap<Image, ImageDTO>()
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomImage))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UrlPhoto))
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.IdImage, opt => opt.Ignore()) // Ignorer l'ID pour les nouvelles images
                .ForMember(dest => dest.NomImage, opt => opt.MapFrom(src => src.Nom))
                .ForMember(dest => dest.UrlPhoto, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.ProduitNavigation, opt => opt.Ignore());
        }
    }
}