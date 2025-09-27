using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.DTO;
using td_revision.Models.Repository;
using AutoMapper;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TypeProduitController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository<TypeProduit> _typeProduitRepository;
        private readonly IDataRepository<Produit> _produitRepository;

        public TypeProduitController(IMapper mapper, IDataRepository<TypeProduit> typeProduitRepository, IDataRepository<Produit> produitRepository)
        {
            _mapper = mapper;
            _typeProduitRepository = typeProduitRepository;
            _produitRepository = produitRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<TypeProduitDTO>> GetById(int id)
        {
            var entity = await _typeProduitRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<TypeProduitDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<TypeProduitDTO>>> GetAll()
        {
            var entities = await _typeProduitRepository.GetAllAsync();
            if (entities.Value == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<TypeProduitDTO>>(entities.Value);
            return Ok(dtos);
        }

        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<TypeProduitDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _typeProduitRepository.GetByStringAsync(name);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<TypeProduitDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<ActionResult<TypeProduitDTO>> Add([FromBody] TypeProduitDTO dto)
        {
            try
            {
                var entity = _mapper.Map<TypeProduit>(dto);
                await _typeProduitRepository.AddAsync(entity);
                var resultDto = _mapper.Map<TypeProduitDTO>(entity);
                return CreatedAtAction("GetById", new { id = entity.IdTypeProduit }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<ActionResult> Update(int id, [FromBody] TypeProduitDTO dto)
        {
            try
            {
                var entityToUpdate = await _typeProduitRepository.GetByIdAsync(id);
                if (entityToUpdate.Value == null)
                {
                    return NotFound();
                }

                _mapper.Map(dto, entityToUpdate.Value);
                await _typeProduitRepository.UpdateAsync(entityToUpdate.Value, entityToUpdate.Value);
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
                var typeProduit = await _typeProduitRepository.GetByIdAsync(id);
                if (typeProduit.Value == null)
                {
                    return NotFound();
                }

                // 1. Récupérer tous les produits de ce type
                var allProducts = await _produitRepository.GetAllAsync();
                if (allProducts.Value != null)
                {
                    var produitsASupprimer = allProducts.Value.Where(p => p.IdTypeProduit == id).ToList();

                    // 2. Supprimer tous les produits liés à ce type
                    foreach (var produit in produitsASupprimer)
                    {
                        await _produitRepository.DeleteAsync(produit);
                    }
                }

                // 3. Supprimer le type de produit
                await _typeProduitRepository.DeleteAsync(typeProduit.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }

        // Méthode pour obtenir le nombre de produits liés à un type
        [HttpGet("{id}/produits-count")]
        [ActionName("GetProduitsCount")]
        public async Task<ActionResult<int>> GetProduitsCount(int id)
        {
            try
            {
                var allProducts = await _produitRepository.GetAllAsync();
                if (allProducts.Value == null)
                {
                    return 0;
                }

                var count = allProducts.Value.Count(p => p.IdTypeProduit == id);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du comptage : {ex.Message}");
            }
        }
    }
}