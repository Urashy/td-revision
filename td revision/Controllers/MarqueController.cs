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
        private readonly IDataRepository<Marque> _marqueRepository;
        private readonly IDataRepository<Produit> _produitRepository;

        public MarqueController(IMapper mapper, IDataRepository<Marque> marqueRepository, IDataRepository<Produit> produitRepository)
        {
            _mapper = mapper;
            _marqueRepository = marqueRepository;
            _produitRepository = produitRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<MarqueDTO>> GetById(int id)
        {
            var entity = await _marqueRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MarqueDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<MarqueDTO>>> GetAll()
        {
            var entities = await _marqueRepository.GetAllAsync();
            if (entities.Value == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<MarqueDTO>>(entities.Value);
            return Ok(dtos);
        }

        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<MarqueDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _marqueRepository.GetByStringAsync(name);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MarqueDTO>(entity.Value);
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
                if (entityToUpdate.Value == null)
                {
                    return NotFound();
                }

                _mapper.Map(dto, entityToUpdate.Value);
                await _marqueRepository.UpdateAsync(entityToUpdate.Value, entityToUpdate.Value);
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
                if (marque.Value == null)
                {
                    return NotFound();
                }

                // 1. Récupérer tous les produits de cette marque
                var allProducts = await _produitRepository.GetAllAsync();
                if (allProducts.Value != null)
                {
                    var produitsASupprimer = allProducts.Value.Where(p => p.IdMarque == id).ToList();

                    // 2. Supprimer tous les produits liés à cette marque
                    foreach (var produit in produitsASupprimer)
                    {
                        await _produitRepository.DeleteAsync(produit);
                    }
                }

                // 3. Supprimer la marque
                await _marqueRepository.DeleteAsync(marque.Value);
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
                if (allProducts.Value == null)
                {
                    return 0;
                }

                var count = allProducts.Value.Count(p => p.IdMarque == id);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du comptage : {ex.Message}");
            }
        }
    }
}