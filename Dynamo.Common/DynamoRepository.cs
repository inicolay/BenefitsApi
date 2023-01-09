using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

namespace Dynamo.Common
{
    public class DynamoRepository : IDynamoRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly DynamoDBContext _context;

        public DynamoRepository()
        {
        }

        public DynamoRepository(AmazonDynamoDBClient client)
        {
            _client = client;
            _context = new DynamoDBContext(_client);
        }

        public async Task<T> GetItemAsync<T>(long pk, long sk)
        {
            return await _context.LoadAsync<T>(pk, sk);
        }

        public async Task<T> GetItemAsync<T>(string pk, long sk)
        {
            return await _context.LoadAsync<T>(pk, sk);
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(IEnumerable<ScanCondition> scanConditions, DynamoDBOperationConfig config)
        {
            List<T> items = new List<T>();
            var scanOperation = _context.ScanAsync<T>(scanConditions, config);
            do
            {
                List<T> itemsRead = await scanOperation.GetNextSetAsync();
                items.AddRange(itemsRead);
            }
            while (!scanOperation.IsDone);
            return items;
        }

        public async Task SaveItemAsync<T>(T item)
        {
            await _context.SaveAsync<T>(item);
        }

        public async Task DeleteItemAsync<T>(string pk, long sk)
        {
            await _context.DeleteAsync<T>(pk, sk);
        }
    }
}