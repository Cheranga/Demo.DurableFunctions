using System.Net.Http;
using System.Threading.Tasks;

namespace Demo.DurableFunctions.Services
{
    public interface IHttpRequestBodyReader
    {
        Task<TModel> GetModelAsync<TModel>(HttpRequestMessage request) where TModel : class;
    }
}