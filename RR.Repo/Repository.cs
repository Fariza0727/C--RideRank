using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RR.Repo
{
    public class Repository<TEntity, TContext> : IRepository<TEntity, TContext>
       where TEntity : class
       where TContext : DbContext, new()
    {
        internal DbContext Context;
        internal DbSet<TEntity> DbSet;

        public Repository(TContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<TEntity> FindByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual void InsertGraph(TEntity entity)
        {

            try
            {
                DbSet.Add(entity);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
            }

        }

        public virtual async Task InsertGraphAsync(TEntity entity)
        {
            DbSet.Add(entity);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }

        }

        public virtual void InsertCollection(List<TEntity> entityCollection)
        {
            try
            {
                entityCollection.ForEach(e =>
                {
                    DbSet.AddAsync(e);
                });
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public virtual async Task UpdateCollection(List<TEntity> entityCollection)
        {
            entityCollection.ForEach(e =>
            {
                Context.Entry(e).State = EntityState.Modified;
            });
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteCollection(List<TEntity> entityCollection)
        {
            entityCollection.ForEach(e =>
            {
                Context.Entry(e).State = EntityState.Deleted;
            });
            await Context.SaveChangesAsync();
        }

        public virtual void UpdateEntityState(List<TEntity> entityCollection)
        {
            entityCollection.ForEach(e =>
            {
                Context.Entry(e).State = EntityState.Modified;
            });
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        public virtual void UpdateAdd(TEntity entity)
        {
            DbSet.Add(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public TEntity Update(TEntity dbEntity, TEntity entity)
        {
            Context.Entry(dbEntity).CurrentValues.SetValues(entity);
            Context.Entry(dbEntity).State = EntityState.Modified;
            return dbEntity;
        }

        public virtual void Delete(object id)
        {
            var entity = DbSet.Find(id);
            Context.Entry(entity).State = EntityState.Deleted;
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {

            DbSet.Attach(entity);
            DbSet.Remove(entity);
            Context.SaveChanges();


        }
        public virtual async Task DeleteAsync(object id)
        {

            var entity = DbSet.Find(id);
            Context.Entry(entity).State = EntityState.Deleted;
            await DeleteAsync(entity);


        }
        public virtual async Task DeleteAsync(TEntity entity)
        {

            DbSet.Attach(entity);
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();


        }



        public virtual void Insert(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            try
            {

                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Added;
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }

        public void LoadNavigations(TEntity entity)
        {
            try
            {
                var entityType = typeof(TEntity);
                var navigationProperties = Context.Model.FindEntityType(typeof(TEntity)).GetNavigations().Select(np => np.Name).ToList();

                foreach (var navigationProperty in navigationProperties)
                {
                    try
                    {
                        var dbReferenceProperty = Context.Entry(entity).Reference(navigationProperty);
                        dbReferenceProperty.Load();
                    }
                    catch
                    {
                        var dbReferenceProperty = Context.Entry(entity).Collection(navigationProperty);
                        dbReferenceProperty.Load();
                    }
                }
            }
            catch { }
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        public virtual RepositoryQuery<TEntity, TContext> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity, TContext>(this);

            return repositoryGetFluentHelper;
        }

        public void ChangeEntityState<T>(T entity, ObjectState state) where T : class
        {
            Context.Entry(entity).State = ConvertState(state);
        }

        public void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class
        {
            foreach (T entity in entityCollection.ToList())
            {
                Context.Entry(entity).State = ConvertState(state);
            }
        }

        internal IQueryable<TEntity> Get( 
            Expression<Func<TEntity, bool>> filter = null,
            bool trackingEnabled = false,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (includeProperties != null)
                query = includeProperties(query);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip(page.Value * pageSize.Value)
                    .Take(pageSize.Value);

            return (trackingEnabled ? query : query.AsNoTracking());
        }

        private EntityState ConvertState(ObjectState state)
        {
            switch (state)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                case ObjectState.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }

        public async Task<SPRequestOutcome> ExecuteSPAsync(KeyValuePair<string, Dictionary<string, object>> spContent)
        {
            string spName = spContent.Key;
            Dictionary<string, object> spParams = spContent.Value != null ? spContent.Value : null;
            string connectionString = Context.Database.GetDbConnection().ConnectionString;
            try
            {
                switch (spName)
                {
                    case "sp_getridersdata":
                        using (IDbConnection con = new SqlConnection(connectionString))
                        {
                            if (con.State == ConnectionState.Closed)
                                con.Open();

                            var data = await con.QueryAsync<RiderRankDto>("sp_getridersdata");

                            return new SPRequestOutcome
                            {
                                Data = data,
                                IsSuccess = true
                            };
                        }
                    case "proc_UpdateUserRole":
                        using (IDbConnection con = new SqlConnection(connectionString))
                        {
                            if (con.State == ConnectionState.Closed)
                                con.Open();

                            var data = await con.QueryAsync<string>("proc_UpdateUserRole", spParams, commandType: CommandType.StoredProcedure);

                            return new SPRequestOutcome
                            {
                                Data = data,
                                IsSuccess = true
                            };
                        }
                }
            }
            catch (Exception ex)
            {
            }

            return new SPRequestOutcome
            {
                Data = null,
                IsSuccess = true
            };
        }
    }
}