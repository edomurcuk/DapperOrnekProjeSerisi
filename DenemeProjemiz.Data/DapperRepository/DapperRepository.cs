﻿using Dapper;
using DenemeProjemiz.Core;
using Dommel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DenemeProjemiz.Data.DapperRepository
{
    public class DapperRepository<T> : IRepository<T> where T : BaseEntity
    {
        string _tableName;

        private SqlConnection SqlConnection()
        {
            IConfiguration configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", true, true)
           .Build();

            string connectionString = configuration.GetSection("CustomApplicationSettings").GetSection("connectionString").Value;

            return new SqlConnection(connectionString);
        }

        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        public int Insert(T t)
        {
            _tableName = typeof(T).Name;

            var insertQuery = GenerateInsertQuery_ScopeIdentity();

            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<int>(insertQuery, t);
            }
        }

        public int InsertRange(IEnumerable<T> list)
        {
            _tableName = typeof(T).Name;

            var inserted = 0;
            var query = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                inserted += connection.Execute(query, list);
            }

            return inserted;
        }

        public int Update(T t)
        {
            _tableName = typeof(T).Name;

            var updateQuery = GenerateUpdateQuery();

            using (var connection = CreateConnection())
            {
                return connection.Execute(updateQuery, t);
            }
        }

        public int Delete(int id, bool? realDelete = null)
        {
            _tableName = typeof(T).Name;

            using (var connection = CreateConnection())
            {
                if (realDelete.HasValue && realDelete.Value)
                    return connection.Execute($"DELETE FROM {_tableName} WHERE id = @id", new { id = id });
                else
                    return connection.Execute($"UPDATE {_tableName} SET silindi = 1 WHERE id = @id", new { id = id });
            }
        }

        public T Get(int id)
        {
            _tableName = typeof(T).Name;

            using (var connection = CreateConnection())
            {
                var result = connection.QuerySingleOrDefault<T>($"SELECT * FROM {_tableName} WHERE id=@id", new { id = id });
                if (result == null)
                    return null;

                return result;
            }
        }

        public IEnumerable<T> GetList(Expression<Func<T, bool>> filter = null)
        {
            _tableName = typeof(T).Name;

            using (var connection = CreateConnection())
            {
                DommelMapper.SetTableNameResolver(new CustomTableNameResolver());

                return filter == null
                    ? connection.Query<T>($"SELECT * FROM {_tableName}")
                    : connection.Select(filter);
            }
        }

        public long GetList_Count(Expression<Func<T, bool>> filter = null)
        {
            _tableName = typeof(T).Name;

            using (var connection = CreateConnection())
            {
                DommelMapper.SetTableNameResolver(new CustomTableNameResolver());

                return filter == null
                    ? connection.QueryFirstOrDefault<long>($"SELECT COUNT(1) FROM {_tableName}")
                    : connection.Count<T>(filter);
            }
        }

        public IEnumerable<T> GetAllIds(string fieldName, List<int> Id)
        {
            _tableName = typeof(T).Name;

            string ids = string.Empty;
            foreach (var id in Id)
                ids += string.Format("{0},", id);

            string query = string.Format("SELECT * FROM {0} WHERE {1} IN ({2})", _tableName, fieldName, ids.TrimEnd(','));

            using (var connection = CreateConnection())
            {
                return connection.Query<T>(query);
            }
        }

        public IEnumerable<T> GetCustomQuery(string customQuery, DynamicParameters parameters, CommandType commandType)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(customQuery, parameters, commandType: commandType);
            }
        }

        public string GetCustomQueryToString(string customQuery, DynamicParameters parameters, CommandType commandType)
        {
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<string>(customQuery, parameters, commandType: commandType);
            }
        }

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where (attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore") && prop.Name != "id"
                    select prop.Name).ToList();
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        private string GenerateInsertQuery_ScopeIdentity()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            insertQuery = new StringBuilder(string.Format("{0}; SELECT SCOPE_IDENTITY();", insertQuery, _tableName));

            return insertQuery.ToString();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals("id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" WHERE id=@id");

            return updateQuery.ToString();
        }

        public class CustomTableNameResolver : DommelMapper.ITableNameResolver
        {
            public string ResolveTableName(Type type)
            {
                // Every table has prefix 'tbl'.
                return $"{type.Name}";
            }
        }
    }
}
