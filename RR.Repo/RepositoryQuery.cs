using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RR.Repo
{
    public sealed class RepositoryQuery<TEntity, TContext>
       where TEntity : class
       where TContext : DbContext, new()
    {
        private readonly Repository<TEntity, TContext> _repository;
        private Expression<Func<TEntity, bool>> _filter;
        private Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> _orderByQuerable;
        private Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> _includeProperties;
        private int? _page;
        private int? _pageSize;
        private bool _trackingEnabled;

        public RepositoryQuery(Repository<TEntity, TContext> repository)
        {
            _repository = repository;
            _trackingEnabled = true;
        }

        public RepositoryQuery<TEntity, TContext> Filter(
            Expression<Func<TEntity, bool>> filter)
        {
            _filter = filter;
            return this;
        }

        public RepositoryQuery<TEntity, TContext> AsNoTracking()
        {
            _trackingEnabled = false;
            return this;
        }

        public RepositoryQuery<TEntity, TContext> OrderBy(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderByQuerable = orderBy;
            return this;
        }

        public RepositoryQuery<TEntity, TContext> Includes(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties)
        {
            _includeProperties = includeProperties;
            return this;
        }

        public IEnumerable<TEntity> GetPage(
            int page, int pageSize, out int totalCount)
        {
            _page = page;
            _pageSize = pageSize;
            totalCount = _repository.Get(_filter).Count();

            return _repository.Get(
                _filter,
                _trackingEnabled,
                _orderByQuerable,
                _includeProperties,
                _page,
                _pageSize);
        }

        public IEnumerable<TEntity> Get()
        {
            return _repository.Get(
                _filter,
                _trackingEnabled,
                _orderByQuerable,
                _includeProperties,
                _page,
                _pageSize);
        }

        public IQueryable<TEntity> GetQuerable()
        {
            return _repository.Get(
                _filter,
                _trackingEnabled,
                _orderByQuerable,
                _includeProperties,
                _page,
                _pageSize);
        }
    }
}
