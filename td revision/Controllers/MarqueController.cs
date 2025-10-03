using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using td_revision.DTO;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MarqueController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly INamedRepository<Marque> _marqueRepository;
        private readonly IRepository<Produit> _produitRepository;
        private readonly IRepository<Image> _imageRepository;

        public MarqueController(
            IMapper mapper,
            INamedRepository<Marque> marqueRepository,
            IRepository<Produit> produitRepository,
            IRepository<Image> imageRepository)
        {
            _mapper = mapper;
            _marqueRepository = marqueRepository;
            _produitRepository = produitRepository;
            _imageRepository = imageRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<MarqueDTO>> GetById(int id)
        {
            var entity = await _marqueRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MarqueDTO>(entity);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<MarqueDTO>>> GetAll()
        {
            var entities = await _marqueRepository.GetAllAsync();
            if (entities == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<MarqueDTO>>(entities);
            return Ok(dtos);
        }

        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<MarqueDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _marqueRepository.GetByNameAsync(name);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MarqueDTO>(entity);
            return Ok(dto);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<ActionResult<MarqueDTO>> Add([FromBody] MarqueDTO dto)
        {
            try
            {
                var entity = _mapper.Map<Marque>(dto);
                await _marqueRepository.AddAsync(entity);
                var resultDto = _mapper.Map<MarqueDTO>(entity);
                return CreatedAtAction("GetById", new { id = entity.IdMarque }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<ActionResult> Update(int id, [FromBody] MarqueDTO dto)
        {
            try
            {
                var entityToUpdate = await _marqueRepository.GetByIdAsync(id);
                if (entityToUpdate == null)
                {
                    return NotFound();
                }

                _mapper.Map(dto, entityToUpdate);
                await _marqueRepository.UpdateAsync(entityToUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la mise à jour : {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var marque = await _marqueRepository.GetByIdAsync(id);
                if (marque == null)
                {
                    return NotFound();
                }

                // 1. Récupérer tous les produits de cette marque
                var allProducts = await _produitRepository.GetAllAsync();
                if (allProducts != null)
                {
                    var produitsASupprimer = allProducts.Where(p => p.IdMarque == id).ToList();

                    var allimage = await _imageRepository.GetAllAsync();

                    if (allimage != null)
                    {
                        var imagesASupprimer = allimage.Where(i => produitsASupprimer.Any(p => p.IdProduit == i.IdProduit)).ToList();
                        foreach (var image in imagesASupprimer)
                        {
                            await _imageRepository.DeleteAsync(image);
                        }
                    }


                    // 2. Supprimer tous les produits liés à cette marque
                    foreach (var produit in produitsASupprimer)
                    {
                        await _produitRepository.DeleteAsync(produit);
                    }
                }

                // 3. Supprimer la marque
                await _marqueRepository.DeleteAsync(marque);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }

        // Méthode pour obtenir le nombre de produits liés à une marque
        [HttpGet("{id}/produits-count")]
        [ActionName("GetProduitsCount")]
        public async Task<ActionResult<int>> GetProduitsCount(int id)
        {
            try
            {
                var allProducts = await _produitRepository.GetAllAsync();
                if (allProducts == null)
                {
                    return 0;
                }

                var count = allProducts.Count(p => p.IdMarque == id);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du comptage : {ex.Message}");
            }
        }
    }
}