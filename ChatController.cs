using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using econest.Service;

namespace econest.Controllers
{
    public class ChatController : Controller
    {
        private readonly HuggingFaceService _huggingFaceService;

        public ChatController(HuggingFaceService huggingFaceService)
        {
            _huggingFaceService = huggingFaceService;
        }

        public IActionResult Index1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AskBot(string message)
        {
            var reply = await _huggingFaceService.GetChatbotResponse(message);
            return Json(new { reply });
        }
    }
}
