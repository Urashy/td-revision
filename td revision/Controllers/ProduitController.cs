using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.Models.Repository;
using AutoMapper;
using td_revision.DTO.Produit;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProduitController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly INamedRepository<Produit> _dataRepository;
        private readonly INamedRepository<Marque> _marqueRepository;
        private readonly INamedRepository<TypeProduit> _typeProduitRepository;
        private readonly IRepository<Image> _imageRepository;

        public ProduitController(
            IMapper mapper,
            INamedRepository<Produit> dataRepository,
            INamedRepository<Marque> marqueRepository,
            INamedRepository<TypeProduit> typeProduitRepository,
            IRepository<Image> imageRepository)
        {
            _mapper = mapper;
            _dataRepository = dataRepository;
            _marqueRepository = marqueRepository;
            _typeProduitRepository = typeProduitRepository;
            _imageRepository = imageRepository;
        }

        // GetAll retourne ProduitDTO
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAll()
        {
            var entities = await _dataRepository.GetAllAsync();
            if (entities == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<ProduitDTO>>(entities);
            return Ok(dtos);
        }

        // GetById retourne ProduitDetailDTO
        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<ProduitDetailDTO>> GetById(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ProduitDetailDTO>(entity);
            return Ok(dto);
        }

        // GetByName retourne ProduitDetailDTO
        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<ProduitDetailDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _dataRepository.GetByNameAsync(name);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ProduitDetailDTO>(entity);
            return Ok(dto);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<ActionResult<ProduitDetailDTO>> Add([FromBody] ProduitPostDTO dto)
        {
            try
            {
                // Validation des stocks
                if (dto.StockMini > dto.StockMaxi)
                {
                    return BadRequest("Le stock minimum ne peut pas être supérieur au stock maximum.");
                }

                // 1. Mapper le DTO vers l'entité
                var entity = _mapper.Map<Produit>(dto);

                // 2. Résoudre l'IdMarque à partir du nom
                if (!string.IsNullOrEmpty(dto.Marque))
                {
                    var marqueResult = await _marqueRepository.GetByNameAsync(dto.Marque);
                    if (marqueResult != null)
                    {
                        entity.IdMarque = marqueResult.IdMarque;
                    }
                    else
                    {
                        return BadRequest($"Marque '{dto.Marque}' introuvable");
                    }
                }

                // 3. Résoudre l'IdTypeProduit à partir du nom
                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var typeResult = await _typeProduitRepository.GetByNameAsync(dto.Type);
                    if (typeResult != null)
                    {
                        entity.IdTypeProduit = typeResult.IdTypeProduit;
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
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<ActionResult> Update(int id, [FromBody] ProduitDetailDTO dto)
        {
            var entityToUpdate = await _dataRepository.GetByIdAsync(id);
            if (entityToUpdate == null)
            {
                return NotFound();
            }

            // Résoudre les FK comme pour l'ajout
            if (!string.IsNullOrEmpty(dto.Marque))
            {
                var marqueResult = await _marqueRepository.GetByNameAsync(dto.Marque);
                if (marqueResult != null)
                {
                    entityToUpdate.IdMarque = marqueResult.IdMarque;
                }
            }

            if (!string.IsNullOrEmpty(dto.Type))
            {
                var typeResult = await _typeProduitRepository.GetByNameAsync(dto.Type);
                if (typeResult != null)
                {
                    entityToUpdate.IdTypeProduit = typeResult.IdTypeProduit;
                }
            }

            _mapper.Map(dto, entityToUpdate);
            await _dataRepository.UpdateAsync(entityToUpdate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var entity = await _dataRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }

                // 1. D'abord récupérer et supprimer toutes les images liées à ce produit
                var allImagesResult = await _imageRepository.GetAllAsync();
                if (allImagesResult != null)
                {
                    var imagesToDelete = allImagesResult.Where(img => img.IdProduit == id).ToList();
                    foreach (var image in imagesToDelete)
                    {
                        await _imageRepository.DeleteAsync(image);
                    }
                }

                // 2. Ensuite supprimer le produit
                await _dataRepository.DeleteAsync(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression du produit : {ex.Message}");
            }
        }

        [HttpGet]
        [ActionName("GetFiltered")]
        public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetFiltered(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? marque = null,
            [FromQuery] string? type = null)
        {
            try
            {
                var entities = await _dataRepository.GetAllAsync();
                if (entities == null)
                {
                    return Ok(new List<ProduitDTO>());
                }

                var query = entities.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p =>
                        p.Nom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                // Filtre par marque
                if (!string.IsNullOrWhiteSpace(marque) && marque != "all")
                {
                    query = query.Where(p => p.MarqueProduitNavigation != null &&
                                            p.MarqueProduitNavigation.Nom == marque);
                }

                if (!string.IsNullOrWhiteSpace(type) && type != "all")
                {
                    query = query.Where(p => p.TypeProduitNavigation != null &&
                                            p.TypeProduitNavigation.Nom == type);
                }


                var dtos = _mapper.Map<IEnumerable<ProduitDTO>>(query);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du filtrage : {ex.Message}");
            }
        }
    }
}