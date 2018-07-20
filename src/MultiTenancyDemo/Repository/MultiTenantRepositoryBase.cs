using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;

namespace MultiTenancyDemo.Repository
{
    public class MultiTenantRepositoryBase<TEntity> 
        : MultiTenantRepositoryBase<TEntity, int>,
          IMultiTenantRepositoryBase<TEntity>
    {
        public MultiTenantRepositoryBase(IDbContextProvider<MultiTenancyDbContext> dbContenxtProvider) 
            : base(dbContenxtProvider)
        {
        }
    }
    
    public class MultiTenantRepositoryBase<TEntity, TKey> : Repository.Repository<MultiTenancyDemo.Data.MultiTenancyDbContext, TEntity, TKey>, IMultiTenantRepositoryBase<TEntity, TKey>
    {
        public MultiTenantRepositoryBase(IDbContextProvider<MultiTenancyDbContext> dbContenxtProvider) 
            : base(dbContenxtProvider)
        {
            
        }
    }
}