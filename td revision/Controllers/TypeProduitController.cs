using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.DTO;
using td_revision.Models.Repository;
using AutoMapper;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TypeProduitController : ControllerGenerique<TypeProduit, TypeProduitDTO>
    {
        public TypeProduitController(IMapper mapper, IDataRepository<TypeProduit> dataRepository)
            : base(mapper, dataRepository)
        {
        }

        protected override object GetEntityId(TypeProduit entity)
        {
            return entity.IdTypeProduit;
        }
    }
}