using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class FieldSubCategoryRepository : Repository<FieldSubCategory>, IFieldSubcategoryRepository
    {
        public FieldSubCategoryRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
