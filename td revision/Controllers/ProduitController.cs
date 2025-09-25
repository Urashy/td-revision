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
        public ProduitController(IMapper mapper, IDataRepository<Produit> dataRepository)
            : base(mapper, dataRepository)
        {
        }

        protected override object GetEntityId(Produit entity)
        {
            return entity.IdProduit;
        }

        // Override si vous voulez un comportement spécifique pour GetById
        // qui retourne ProduitDTO au lieu de ProduitDetailDTO
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