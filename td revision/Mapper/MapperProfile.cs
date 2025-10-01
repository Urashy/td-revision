using td_revision.DTO;
using td_revision.DTO.Produit; 
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
            CreateMap<TypeProduitDTO, TypeProduit>();

            // ========== MARQUE ==========
            CreateMap<Marque, MarqueDTO>()
                .ForMember(dest => dest.NbProduits, opt => opt.MapFrom(src => src.Produits != null ? src.Produits.Count : 0));
            CreateMap<MarqueDTO, Marque>();

            // ========== PRODUIT ==========
            CreateMap<Models.Produit, ProduitDetailDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.Nom : null))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation != null ? src.MarqueProduitNavigation.Nom : null))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.Stock < src.StockMini));
            CreateMap<ProduitDetailDTO, Models.Produit>();

            CreateMap<ProduitPostDTO, Models.Produit>();

            // ← MODIFIER CE MAPPING pour ajouter EnReappro
            CreateMap<Models.Produit, ProduitDTO>()
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueProduitNavigation.Nom))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation.Nom))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.Stock < src.StockMini));  

            // ========== IMAGE ==========
            CreateMap<Image, ImageDTO>();
            CreateMap<ImageDTO, Image>();
        }
    }
}