using Entities.DomainEntities;
using Interface;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Interface.Services.DomainServices;
using Extensions;
using AppDbContext;
using Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Interface.DbContext;
using System.Data.Common;
using System.Threading;

namespace Service.Services.DomainServices
{
    public abstract class DomainService<E, T> : IDomainService<E, T> where E : Entities.DomainEntities.DomainEntities, new() where T : BaseSearch, new() 
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected IQueryable<E> Queryable
        {
            get
            {
                return unitOfWork.Repository<E>().GetQueryable();//.AsNoTracking();
                //return unitOfWork.Repository<E>().GetQueryable().AsNoTracking();
            }
        }
        public DomainService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public virtual void LoadReferences(IList<E> items)
        {
        }

        public virtual bool Save(E item)
        {
            return Save(new List<E> { item });
        }
        public virtual bool Save(IList<E> items)
        {
            foreach (var item in items)
            {
                var exists = Queryable
                .Where(e =>
                e.id == item.id
                && e.deleted == false)
                .FirstOrDefault();
                if (exists != null)
                {
                    if (item.deleted == false)
                    {
                        Delete(item.id);
                    }
                    else
                    {
                        exists = mapper.Map<E>(item);
                        unitOfWork.Repository<E>().SetEntityState(exists, EntityState.Modified);
                    }
                }
                else
                {
                    unitOfWork.Repository<E>().Create(item);
                }
            }
            unitOfWork.Save();
            return true;
        }

        public virtual bool IsSafeDelete(int id)
        {
            return true;
        }

        public virtual async Task Validate(E model)
        {
            await Task.Run(() => { });
        }

        public virtual async Task<E> AddItemWithResponse(E model)
        {
            //new 
            await AddItem(model);
            return model;
        }

        public virtual async Task AddItem(E model)
        {
            await Validate(model);
            await CreateAsync(model);
        }

        public virtual async Task<E> UpdateItemWithResponse(E model)
        {
            await UpdateItem(model);
            return model;
        }
        public virtual async Task UpdateItem(E model)
        {
            await Validate(model);
            await UpdateAsync(model);
        }
        public virtual async Task DeleteItem(Guid id)
        {
            await DeleteAsync(id);
        }

        public virtual bool Delete(Guid id)
        {
            var exists = Queryable
                .Where(e => e.id == id)
                .FirstOrDefault();
            if (exists != null)
            {
                exists.deleted = true;
                unitOfWork.Repository<E>().Update(exists);
                unitOfWork.Save();
                return true;
            }
            else
            {
                throw new Exception(id + " not exists");
            }
        }


        public virtual IList<E> GetAll()
        {
            return GetAll(null);
        }
        public virtual IList<E> GetAll(Expression<Func<E, E>> select)
        {
            var query = Queryable.Where(e => e.deleted == false);
            if (select != null)
            {
                query = query.Select(select);
            }
            return query.ToList();
        }

        public virtual async Task<PagedList<E>> GetPagedListData(T baseSearch)
        {
            PagedList<E> pagedList = new PagedList<E>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<E>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }

        /// <summary>
        /// Lấy thông tin tên procedure cần exec
        /// </summary>
        /// <returns></returns>
        protected virtual string GetStoreProcName()
        {
            return string.Empty;
        }

        protected virtual SqlParameter[] GetSqlParameters(object baseSearch)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (PropertyInfo prop in baseSearch.GetType().GetProperties())
            {
                sqlParameters.Add(new SqlParameter($"@{prop.Name}", prop.GetValue(baseSearch, null)));
            }
            SqlParameter[] parameters = sqlParameters.ToArray();
            return parameters;
        }

        protected virtual string GenerateParamsString(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            string paramString = string.Join(", ",
                properties
                    .Where(prop => prop.GetValue(obj) != null) // Skip null values
                    .Select(prop =>
                    {
                        object propValue = prop.GetValue(obj);
                        string formattedValue;

                        if (prop.PropertyType == typeof(string))
                        {
                            formattedValue = propValue != null ? $"N'{propValue}'" : "NULL";
                        }
                        else if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                        {
                            formattedValue = propValue != null ? $"'{propValue}'" : "NULL";
                        }
                        else
                        {
                            formattedValue = propValue.ToString();
                        }

                        return $"@{prop.Name}={formattedValue}";
                    })
            );

            return paramString;
        }


        public virtual E GetById(Guid id)
        {
            return GetById(id, (IConfigurationProvider)null);
        }
        public virtual E GetById(Guid id, Expression<Func<E, E>> select)
        {
            var query = Queryable.Where(e => e.deleted == false);
            if (select != null)
            {
                query = query.Select(select);
            }
            return query
                 .AsNoTracking()
                 .Where(e => e.id == id)
                 .FirstOrDefault();
        }

        public IList<E> Get(Expression<Func<E, bool>> expression)
        {
            return Get(new Expression<Func<E, bool>>[] { expression });
        }

        public IList<E> Get(Expression<Func<E, bool>> expression, Expression<Func<E, E>> select)
        {
            return Get(new Expression<Func<E, bool>>[] { expression }, select);
        }
        public IList<E> Get(Expression<Func<E, bool>> expression, IConfigurationProvider mapperConfiguration)
        {
            if (mapperConfiguration == null)
            {
                return Get(expression);
            }
            else
            {
                return Get(new Expression<Func<E, bool>>[] { expression }, mapperConfiguration);
            }
        }
        public virtual async Task<bool> SaveAsync(E item)
        {
            return await SaveAsync(new List<E> { item });
        }

        public virtual async Task<bool> CreateAsync(E item)
        {
            return await CreateAsync(new List<E> { item });
        }

        public virtual async Task<bool> CreateAsync(IList<E> items)
        {
            foreach (var model in items)
            {
                try
                {
                    foreach (PropertyInfo item in model.GetType().GetProperties())
                    {
                        var value = item.GetValue(model);
                        var typeofItem = item.PropertyType.GenericTypeArguments.FirstOrDefault();
                        if (typeofItem == typeof(Boolean))
                        {
                            item.SetValue(model, value ?? false);
                        }
                        else if (typeofItem == typeof(Int32) || item.PropertyType == typeof(Double))
                        {
                            item.SetValue(model, value ?? 0);
                        }
                        else if (item.PropertyType == typeof(String))
                        {
                            item.SetValue(model, value ?? "");
                        }
                    }

                }
                catch (Exception ex)
                {
                    return true;
                }
                model.created = Timestamp.Now();
                model.deleted = false;
                if (LoginContext.Instance.CurrentUser != null)
                {
                    model.createdBy = LoginContext.Instance.CurrentUser.userId;
                }
                await unitOfWork.Repository<E>().CreateAsync(model);
            }
            await unitOfWork.SaveAsync();
            return true;
        }
        public virtual async Task<bool> UpdateAsync(E item)
        {
            return await UpdateAsync(new List<E> { item });
        }

        public async Task<bool> UpdateAsync(IList<E> items)
        {
            foreach (var model in items)
            {
                var entity = await Queryable
                 .Where(e => e.id == model.id && e.deleted == false)
                 .FirstOrDefaultAsync();

                if (entity != null)
                {
                    foreach (PropertyInfo item_old in entity.GetType().GetProperties())
                    {
                        foreach (PropertyInfo item_new in model.GetType().GetProperties())
                        {
                            if (item_old.Name == item_new.Name)
                            {
                                var value_old = item_old.GetValue(entity);
                                var value_new = item_new.GetValue(model);
                                if (value_old != value_new)
                                {
                                    item_old.SetValue(entity, value_new ?? value_old);
                                }
                                break;
                            }
                        }
                    }
                    entity.updatedBy = LoginContext.Instance.CurrentUser == null ? entity.updatedBy : LoginContext.Instance.CurrentUser.userId;
                    entity.updated = Timestamp.Now();
                    unitOfWork.Repository<E>().Update(entity);
                }
                else
                {
                    throw new AppException("Item không tồn tại");
                }
            }
            await unitOfWork.SaveAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật theo field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFieldAsync(E item, params Expression<Func<E, object>>[] includeProperties)
        {
            return await UpdateFieldAsync(new List<E> { item }, includeProperties);
        }

        /// <summary>
        /// Cập nhật theo field
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFieldAsync(IList<E> items, params Expression<Func<E, object>>[] includeProperties)
        {
            foreach (var item in items)
            {
                var exists = await Queryable
                 .Where(e => e.id == item.id && e.deleted == false)
                 .FirstOrDefaultAsync();

                if (exists != null)
                {
                    exists = mapper.Map<E>(item);
                    unitOfWork.Repository<E>().UpdateFieldsSave(exists, includeProperties);
                }
            }
            await unitOfWork.SaveAsync();
            return true;
        }



        public async Task<bool> SaveAsync(IList<E> items)
        {
            foreach (var item in items)
            {
                var exists = await Queryable
                 .Where(e => e.id == item.id && e.deleted == false)
                 .FirstOrDefaultAsync();

                if (exists != null)
                {
                    exists = mapper.Map<E>(item);
                    unitOfWork.Repository<E>().Update(exists);
                }
                else
                {
                    await unitOfWork.Repository<E>().CreateAsync(item);
                }
            }
            await unitOfWork.SaveAsync();
            return true;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var exists = Queryable
                .FirstOrDefault(e => e.id == id);
            if (exists != null)
            {
                exists.deleted = true;
                unitOfWork.Repository<E>().Update(exists);
                await unitOfWork.SaveAsync();
                return true;
            }
            else
            {
                throw new AppException(id + " not exists");
            }
        }

        public async Task<IList<E>> GetAllAsync()
        {
            return await Queryable.AsNoTracking().ToListAsync();
        }

        public virtual async Task<E> GetByIdAsync(Guid id)
        {
            return await Queryable.Where(e => e.id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task<E> GetByIdAsync(Guid id, Expression<Func<E, E>> select)
        {
            var query = unitOfWork.Repository<E>()
               .GetQueryable()
               .Where(e => e.deleted == false)
               .AsNoTracking();
            if (select != null)
            {
                query = query.Select(select);
            }
            return await query.FirstOrDefaultAsync(e => e.id == id);
        }

        public virtual async Task<IList<E>> GetAllAsync(Expression<Func<E, E>> select)
        {
            return await Queryable
                .Select(select)
                .ToListAsync();

        }

        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression, Expression<Func<E, E>> select)
        {
            return await unitOfWork.Repository<E>()
                 .GetQueryable()
                 .Where(expression)
                 .Select(select)
                 .ToListAsync();
        }

        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression)
        {
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .Where(expression)
                .ToListAsync();
        }


        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression, bool useProjectTo)
        {
            if (useProjectTo)
                return await unitOfWork.Repository<E>()
                .GetQueryable()
                .ProjectTo<E>(mapper.ConfigurationProvider)
                .Where(expression)
                .ToListAsync();
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .ProjectTo<E>(mapper.ConfigurationProvider)
                .Where(expression)
                .ToListAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>> expression, Expression<Func<E, E>> select)
        {
            return await unitOfWork.Repository<E>()
                 .GetQueryable()
                 .Where(expression)
                 .Select(select)
                 .FirstOrDefaultAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>> expression)
        {
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>> expression, bool useProjectTo)
        {
            if (useProjectTo)
                return await unitOfWork.Repository<E>()
                .GetQueryable()
                .ProjectTo<E>(mapper.ConfigurationProvider)
                .Where(expression)
                .FirstOrDefaultAsync();
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .ProjectTo<E>(mapper.ConfigurationProvider)
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public E GetById(Guid id, IConfigurationProvider mapperConfiguration)
        {
            var queryable = Queryable.Where(e => e.deleted == false && e.id == id);
            if (mapperConfiguration != null)
                queryable = queryable.ProjectTo<E>(mapperConfiguration);
            return queryable.AsNoTracking().FirstOrDefault();
        }

        public virtual async Task<E> GetByIdAsync(Guid id, IConfigurationProvider mapperConfiguration)
        {
            var queryable = Queryable.Where(e => e.deleted == false && e.id == id);
            if (mapperConfiguration != null)
                queryable = queryable.ProjectTo<E>(mapperConfiguration);
            return await queryable.AsNoTracking().FirstOrDefaultAsync();
        }

        public IList<E> Get(Expression<Func<E, bool>>[] expressions, Expression<Func<E, E>> select)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            if (select != null)
                queryable = queryable.Select(select);
            return queryable.ToList();
        }

        public IList<E> Get(Expression<Func<E, bool>>[] expressions)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            return queryable.ToList();
        }

        public IList<E> Get(Expression<Func<E, bool>>[] expressions, IConfigurationProvider mapperConfiguration)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            queryable = queryable
                .ProjectTo<E>(mapperConfiguration);
            return queryable.ToList();
        }

        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expressions, Expression<Func<E, E>> select)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            if (select != null)
            {
                queryable = queryable.Select(select);
            }
            return await queryable.ToListAsync();
        }

        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expressions)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }

            return await queryable.ToListAsync();
        }

        public async Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expressions, bool useProjectTo)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            if (useProjectTo)
            {
                queryable = queryable.ProjectTo<E>(mapper.ConfigurationProvider);
            }
            return await queryable.ToListAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>>[] expressions, Expression<Func<E, E>> select)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            if (select != null)
            {
                queryable = queryable.Select(select);
            }
            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>>[] expressions)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<E> GetSingleAsync(Expression<Func<E, bool>>[] expressions, bool useProjectTo)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }
            if (useProjectTo)
            {
                queryable = queryable.ProjectTo<E>(mapper.ConfigurationProvider);
            }
            return await queryable.FirstOrDefaultAsync();
        }

        public virtual async Task<string> GetExistItemMessage(E item)
        {
            return await Task.Run(() =>
            {
                List<string> messages = new List<string>();
                string result = string.Empty;
                if (messages.Any())
                    result = string.Join(" ", messages);
                return result;
            });
        }
        public async Task<bool> AnyAsync(Expression<Func<E, bool>> expression)
        {
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .Where(expression)
                .AnyAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<E, bool>>[] expressions)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }

            return await queryable.AnyAsync();
        }
        public async Task<int> CountAsync(Expression<Func<E, bool>> expression)
        {
            return await unitOfWork.Repository<E>()
                .GetQueryable()
                .Where(expression)
                .CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<E, bool>>[] expressions)
        {
            var queryable = Queryable.Where(e => e.deleted == false);
            foreach (var expression in expressions)
            {
                queryable = queryable.Where(expression);
            }

            return await queryable.CountAsync();
        }
    }
}
