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
        private readonly IRepository<Image> _imageRepository;  // Pas INamedRepository

        public ImageController(IMapper mapper, IRepository<Image> imageRepository)
        {
            _mapper = mapper;
            _imageRepository = imageRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<ActionResult<ImageDTO>> GetById(int id)
        {
            var entity = await _imageRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<ImageDTO>(entity);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetAll()
        {
            var entities = await _imageRepository.GetAllAsync();
            if (entities == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<ImageDTO>>(entities);
            return Ok(dtos);
        }

        [HttpGet("{produitId}")]
        [ActionName("GetByProduitId")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetByProduitId(int produitId)
        {
            try
            {
                var allImages = await _imageRepository.GetAllAsync();
                if (allImages == null)
                {
                    return NotFound();
                }

                var produitImages = allImages.Where(img => img.IdProduit == produitId);
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
                await _imageRepository.AddAsync(entity);
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
                var entityToUpdate = await _imageRepository.GetByIdAsync(id);
                if (entityToUpdate == null)
                {
                    return NotFound();
                }

                _mapper.Map(dto, entityToUpdate);
                await _imageRepository.UpdateAsync(entityToUpdate);
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
                var entity = await _imageRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }

                await _imageRepository.DeleteAsync(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }
    }
}