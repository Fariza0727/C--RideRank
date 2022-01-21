using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Repo
{
     public interface IRepository<TEntity, TContext>
         where TEntity : class
         where TContext : DbContext, new()
     {
          TEntity FindById(object id);
          Task<TEntity> FindByIdAsync(object id);
          void UpdateAdd(TEntity entity);

          void Insert(TEntity entity);
          Task InsertAsync(TEntity entity);

          void InsertGraph(TEntity entity);
          Task InsertGraphAsync(TEntity entity);

          void Update(TEntity entity);
          Task UpdateAsync(TEntity entity);

          TEntity Update(TEntity dbEntity, TEntity entity);

          void Delete(object id);
          void Delete(TEntity entity);
          Task DeleteAsync(object id);
          Task DeleteAsync(TEntity entity);

          void LoadNavigations(TEntity entity);

          void ChangeEntityState<T>(T entity, ObjectState state) where T : class;
          void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class;
          RepositoryQuery<TEntity, TContext> Query();
          void Dispose();

          void SaveChanges();
          Task SaveChangesAsync();

        void InsertCollection(List<TEntity> entityCollection);
        Task UpdateCollection(List<TEntity> entityCollection);
        Task DeleteCollection(List<TEntity> entityCollection);
        void UpdateEntityState(List<TEntity> entityCollection);

        Task<SPRequestOutcome> ExecuteSPAsync(KeyValuePair<string, Dictionary<string, object>> spContent);
    }
}
