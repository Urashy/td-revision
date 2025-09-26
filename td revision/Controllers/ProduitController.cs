using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.DTO;
using td_revision.Models.Repository;
using AutoMapper;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProduitController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository<Produit> _dataRepository;
        private readonly IDataRepository<Marque> _marqueRepository;
        private readonly IDataRepository<TypeProduit> _typeProduitRepository;

        public ProduitController(
            IMapper mapper,
            IDataRepository<Produit> dataRepository,
            IDataRepository<Marque> marqueRepository,
            IDataRepository<TypeProduit> typeProduitRepository)
        {
            _mapper = mapper;
            _dataRepository = dataRepository;
            _marqueRepository = marqueRepository;
            _typeProduitRepository = typeProduitRepository;
        }

        // GetAll retourne ProduitDTO (version simple pour la liste)
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAll()
        {
            var entities = await _dataRepository.GetAllAsync();
            if (entities.Value == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<ProduitDTO>>(entities.Value);
            return Ok(dtos);
        }

        // GetById retourne ProduitDetailDTO (version complète)
        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<ProduitDetailDTO>> GetById(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ProduitDetailDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<ActionResult<ProduitDetailDTO>> Post([FromBody] ProduitDetailDTO dto)
        {
            try
            {
                // 1. Mapper le DTO vers l'entité (sans les IDs de FK)
                var entity = _mapper.Map<Produit>(dto);

                // 2. Résoudre l'IdMarque à partir du nom
                if (!string.IsNullOrEmpty(dto.Marque))
                {
                    var marqueResult = await _marqueRepository.GetByStringAsync(dto.Marque);
                    if (marqueResult.Value != null)
                    {
                        entity.IdMarque = marqueResult.Value.IdMarque;
                    }
                    else
                    {
                        return BadRequest($"Marque '{dto.Marque}' introuvable");
                    }
                }

                // 3. Résoudre l'IdTypeProduit à partir du nom
                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var typeResult = await _typeProduitRepository.GetByStringAsync(dto.Type);
                    if (typeResult.Value != null)
                    {
                        entity.IdTypeProduit = typeResult.Value.IdTypeProduit;
                    }
                    else
                    {
                        return BadRequest($"Type de produit '{dto.Type}' introuvable");
                    }
                }

                // 4. Ajouter l'entité
                await _dataRepository.AddAsync(entity);

                // 5. Retourner le DTO avec l'ID généré
                var resultDto = _mapper.Map<ProduitDetailDTO>(entity);
                return CreatedAtAction("GetById", new { id = entity.IdProduit }, resultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du produit : {ex.Message}");
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<ActionResult> Put(int id, [FromBody] ProduitDetailDTO dto)
        {
            try
            {
                var entityToUpdate = await _dataRepository.GetByIdAsync(id);
                if (entityToUpdate.Value == null)
                {
                    return NotFound();
                }

                // Mapper les propriétés basiques
                entityToUpdate.Value.Nom = dto.Nom;
                entityToUpdate.Value.Description = dto.Description;
                entityToUpdate.Value.StockReel = dto.Stock;

                // Résoudre l'IdMarque à partir du nom
                if (!string.IsNullOrEmpty(dto.Marque))
                {
                    var marqueResult = await _marqueRepository.GetByStringAsync(dto.Marque);
                    if (marqueResult.Value != null)
                    {
                        entityToUpdate.Value.IdMarque = marqueResult.Value.IdMarque;
                    }
                    else
                    {
                        return BadRequest($"Marque '{dto.Marque}' introuvable");
                    }
                }

                // Résoudre l'IdTypeProduit à partir du nom
                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var typeResult = await _typeProduitRepository.GetByStringAsync(dto.Type);
                    if (typeResult.Value != null)
                    {
                        entityToUpdate.Value.IdTypeProduit = typeResult.Value.IdTypeProduit;
                    }
                    else
                    {
                        return BadRequest($"Type de produit '{dto.Type}' introuvable");
                    }
                }

                await _dataRepository.UpdateAsync(entityToUpdate.Value, entityToUpdate.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du produit : {ex.Message}");
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteAsync(entity.Value);
            return NoContent();
        }

        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<ProduitDetailDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _dataRepository.GetByStringAsync(name);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ProduitDetailDTO>(entity.Value);
            return Ok(dto);
        }
    }
}