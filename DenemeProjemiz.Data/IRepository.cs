using Dapper;
using DenemeProjemiz.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DenemeProjemiz.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        int Insert(T entity);
        int InsertRange(IEnumerable<T> list);
        int Update(T entity);
        int Delete(int id, bool? realDelete = null);
        T Get(int id);
        IEnumerable<T> GetList(Expression<Func<T, bool>> filter = null);
        IEnumerable<T> GetCustomQuery(string customQuery, DynamicParameters parameters, CommandType commandType);
        IEnumerable<T> GetAllIds(string fieldName, List<int> Id);
        long GetList_Count(Expression<Func<T, bool>> filter = null);
        string GetCustomQueryToString(string customQuery, DynamicParameters parameters, CommandType commandType);
    }
}
