using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using td_revision.DTO;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository<Image> _dataRepository;

        public ImageController(IMapper mapper, IDataRepository<Image> dataRepository)
        {
            _mapper = mapper;
            _dataRepository = dataRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<ImageDTO>> GetById(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ImageDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetAll()
        {
            var entities = await _dataRepository.GetAllAsync();
            if (entities.Value == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<ImageDTO>>(entities.Value);
            return Ok(dtos);
        }

        [HttpGet]
        [ActionName("GetByName")]
        public async Task<ActionResult<ImageDTO>> GetByName([FromQuery] string name)
        {
            var entity = await _dataRepository.GetByStringAsync(name);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ImageDTO>(entity.Value);
            return Ok(dto);
        }

        [HttpGet("{produitId}")]
        [ActionName("GetByProduitId")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetByProduitId(int produitId)
        {
            try
            {
                var allImages = await _dataRepository.GetAllAsync();
                if (allImages.Value == null)
                {
                    return NotFound();
                }

                var produitImages = allImages.Value.Where(img => img.IdProduit == produitId);
                var imageDtos = _mapper.Map<IEnumerable<ImageDTO>>(produitImages);

                return Ok(imageDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des images : {ex.Message}");
            }
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<ActionResult<ImageDTO>> Add([FromBody] ImageDTO dto)
        {
            try
            {
                var entity = _mapper.Map<Image>(dto);
                await _dataRepository.AddAsync(entity);
                var resultDto = _mapper.Map<ImageDTO>(entity);
                return CreatedAtAction("GetById", new { id = entity.IdImage }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<ActionResult> Update(int id, [FromBody] ImageDTO dto)
        {
            try
            {
                var entityToUpdate = await _dataRepository.GetByIdAsync(id);
                if (entityToUpdate.Value == null)
                {
                    return NotFound();
                }

                _mapper.Map(dto, entityToUpdate.Value);
                await _dataRepository.UpdateAsync(entityToUpdate.Value, entityToUpdate.Value);
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
                var entity = await _dataRepository.GetByIdAsync(id);
                if (entity.Value == null)
                {
                    return NotFound();
                }

                await _dataRepository.DeleteAsync(entity.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }
    }
}