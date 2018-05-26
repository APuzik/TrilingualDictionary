using System.Data.SqlServerCe;

namespace MultiDictionaryCore.DataLayer.Interfaces
{
    interface IDBEntityFactory<T> where T : new()
    {
        T CreateDBEntity(SqlCeDataReader reader);
    }
}