using Microsoft.AspNetCore.Mvc;
using td_revision.Models.Repository;
using AutoMapper;

namespace td_revision.Controllers
{
    [ApiController]
    public abstract class ControllerGenerique<TEntity, TDto> : ControllerBase
        where TEntity : class
        where TDto : class
    {
        protected readonly IMapper _mapper;
        protected readonly IDataRepository<TEntity> _dataRepository;

        protected ControllerGenerique(IMapper mapper, IDataRepository<TEntity> dataRepository)
        {
            _mapper = mapper;
            _dataRepository = dataRepository;
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        public virtual async Task<ActionResult<TDto>> Get(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<TDto>(entity.Value);
            return Ok(dto);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var entities = await _dataRepository.GetAllAsync();
            if (entities.Value == null)
            {
                return NotFound();
            }
            var dtos = _mapper.Map<IEnumerable<TDto>>(entities.Value);
            return Ok(dtos);
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public virtual async Task<ActionResult> Delete(int id)
        {
            var entity = await _dataRepository.GetByIdAsync(id);
            if (entity.Value == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteAsync(entity.Value);
            return NoContent();
        }

        [HttpPost]
        [ActionName("Add")]
        public virtual async Task<ActionResult<TDto>> Post([FromBody] TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _dataRepository.AddAsync(entity);
            var resultDto = _mapper.Map<TDto>(entity);
            return CreatedAtAction("GetById", new { id = GetEntityId(entity) }, resultDto);
        }

        [HttpPut("{id}")]
        [ActionName("Update")]
        public virtual async Task<ActionResult> Put(int id, [FromBody] TDto dto)
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

        [HttpGet]
        [ActionName("GetByName")]
        public virtual async Task<ActionResult<TDto>> GetByName([FromQuery] string name)
        {
            var entity = await _dataRepository.GetByStringAsync(name);
            if (entity.Value == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<TDto>(entity.Value);
            return Ok(dto);
        }

        protected abstract object GetEntityId(TEntity entity);
    }
}