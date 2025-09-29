using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using td_revision.DTO;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        public ImageController(IMapper mapper, IDataRepository<Image> dataRepository)
            : base(mapper, dataRepository)
        {
        }


        // Méthode spécifique pour récupérer les images par IdProduit
        [HttpGet("{produitId}")]
        [ActionName("GetByProduitId")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetByProduitId(int produitId)
        {
            try
            {
                // Supposant que votre repository a une méthode pour filtrer par IdProduit
                var allImages = await _dataRepository.GetAllAsync();
                if (allImages.Value == null)
                {
                    return NotFound();
                }

                // Filtrer les images par IdProduit
                var produitImages = allImages.Value.Where(img => GetProduitId(img) == produitId);
                var imageDtos = _mapper.Map<IEnumerable<ImageDTO>>(produitImages);

                return Ok(imageDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des images : {ex.Message}");
            }
        }

        // Méthode pour obtenir l'IdProduit d'une image (à adapter selon votre modèle Image)
        private int GetProduitId(Image image)
        {
            // Remplacez par la propriété correcte de votre classe Image
            return image.IdProduit; // Assumant que vous avez une propriété IdProduit dans Image
        }
    }
}