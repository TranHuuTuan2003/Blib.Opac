using KMS.Shared.DTOs.Search;
using KMS.Web.ViewModels.Shared.Pages.Chatbot;

namespace KMS.Web.Services.Chatbot
{
    public interface IService
    {
        Task<ChatbotViewModel> SearchDocBotAsync(SearchRequest searchRequest, bool hasFacetFilter);
    }
}