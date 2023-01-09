using Amazon.DynamoDBv2.DataModel;

namespace Dynamo.Common
{
    public interface IDynamoRepository
    {
        //Task<T> GetItemAsync<T>(long key);
        Task<T> GetItemAsync<T>(long pk, long sk);
        Task<T> GetItemAsync<T>(string pk, long sk);
        Task<IEnumerable<T>> GetAllItemsAsync<T>(IEnumerable<ScanCondition> scanConditions, DynamoDBOperationConfig config);
        Task SaveItemAsync<T>(T item);
        Task DeleteItemAsync<T>(string pk, long sk);
    }
}