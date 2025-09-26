using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.DTO;
using td_revision.Models.Repository;
using AutoMapper;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProduitController : ControllerGenerique<Produit, ProduitDetailDTO>
    {
        private readonly IDataRepository<Marque> _marqueRepository;
        private readonly IDataRepository<TypeProduit> _typeProduitRepository;

        public ProduitController(
            IMapper mapper,
            IDataRepository<Produit> dataRepository,
            IDataRepository<Marque> marqueRepository,
            IDataRepository<TypeProduit> typeProduitRepository)
            : base(mapper, dataRepository)
        {
            _marqueRepository = marqueRepository;
            _typeProduitRepository = typeProduitRepository;
        }

        protected override object GetEntityId(Produit entity)
        {
            return entity.IdProduit;
        }

        // 🔧 OVERRIDE de la méthode Add pour résoudre les IDs
        [HttpPost]
        [ActionName("Add")]
        public override async Task<ActionResult<ProduitDetailDTO>> Post([FromBody] ProduitDetailDTO dto)
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

        // Override si vous voulez un comportement spécifique pour GetById
        [HttpGet("{id}")]
        [ActionName("GetByIdSimple")]
        public async Task<ActionResult<ProduitDTO>> GetSimple(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ProduitDTO>(entity.Value);
            return Ok(dto);
        }
    }
}