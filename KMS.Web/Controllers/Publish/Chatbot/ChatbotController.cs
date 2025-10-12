using Microsoft.AspNetCore.Mvc;

using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;
using KMS.Web.Services.Chatbot;
using KMS.Web.ViewModels.Shared.Pages.Chatbot;

namespace KMS.Web.Controllers.Publish.Chatbot
{
    public class ChatbotController : Controller
    {
        private readonly ILogger<ChatbotController> _logger;
        private readonly IService _service;

        public ChatbotController(ILogger<ChatbotController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        [Route("chatbot")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendRequestAsync([FromBody] SearchRequest searchRequest, bool hasFacetFilter)
        {
            try
            {
                var searchResult = await _service.SearchDocBotAsync(searchRequest, hasFacetFilter);
                return PartialView("Chatbot/_BotMessageContainer", searchResult);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return PartialView("Chatbot/_BotMessageContainer", new ChatbotViewModel());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}