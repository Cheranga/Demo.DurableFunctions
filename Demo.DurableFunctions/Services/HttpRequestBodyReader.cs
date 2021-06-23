using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Demo.DurableFunctions.Services
{
    public class HttpRequestBodyReader : IHttpRequestBodyReader
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Error = (sender, args) => args.ErrorContext.Handled = true
        };

        public async Task<TModel> GetModelAsync<TModel>(HttpRequestMessage request) where TModel : class
        {
            try
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<TModel>(requestBody, serializerSettings);

                return model;
            }
            catch
            {
                return default;
            }
        }
    }
}