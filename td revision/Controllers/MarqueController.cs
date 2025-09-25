using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using td_revision.DTO;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MarqueController : ControllerGenerique<Marque, MarqueDTO>
    {
        public MarqueController(IMapper mapper, IDataRepository<Marque> dataRepository)
            : base(mapper, dataRepository)
        {
        }

        protected override object GetEntityId(Marque entity)
        {
            return entity.IdMarque;
        }
    }
}