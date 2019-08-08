using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace YUNXI.EDUS.EntityFramework.Repositories
{
    public abstract class EDUSRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<EDUSDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EDUSRepositoryBase(IDbContextProvider<EDUSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class EDUSRepositoryBase<TEntity> : EDUSRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected EDUSRepositoryBase(IDbContextProvider<EDUSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
