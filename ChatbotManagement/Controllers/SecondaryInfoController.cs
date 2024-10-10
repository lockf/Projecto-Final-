using ChatbotManagement.Common;
using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Models.out_VM;
using ChatbotManagement.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatbotManagement.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class SecondaryInfoController : Controller
    {
        private readonly MessageService _messageService;
        private readonly SolrService _solrService;
        private readonly AppDbContext _context;
        private readonly Others _others;
        private readonly UserService _userService;

        public SecondaryInfoController(MessageService messageService, SolrService solrService, AppDbContext context, Others others, UserService userService)
        {
            _context = context;
            _solrService = solrService;
            _messageService = messageService;
            _others = others;
            _userService = userService;
        }

        /// <summary>
        /// Shows message complete info
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <returns>The view with the message details.</returns>
        [HttpGet("ShowMessage/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ShowMessage(int id)
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                MessageVM? mess = null;
                if (_messageService.MessagesExist(id))
                {
                    var idMessage = await _context.Messages.FindAsync(id);
                    mess = new(idMessage!.Text, idMessage.Date, idMessage.ChannelId, idMessage.ConversationId);
                }

                return View(mess);
            }
            else
            {
                return RedirectToAction("LoginManagement", "Login");
            }
        }

        /// <summary>
        /// Shows a list of messages and a list of keywords
        /// </summary>
        /// <returns>The view with the list of messages and a list of keywords</returns>
        [HttpGet("SecondaryInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SecondaryInfo()
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                SecondaryInfoVM? secInfo = null;

                List<Message2VM> message2VMs = new();
                List<string> keywords = new();

                if (_messageService.MessagesExist() || _solrService.KeywordExistsInDB())
                {
                    var messages = await _context.Messages.ToListAsync();

                    foreach (var mess in messages)
                    {
                        Message2VM mess2 = new(mess.Id, mess.ChannelId, mess.ConversationId, mess.Date);
                        message2VMs.Add(mess2);
                    }

                    var kWords = await _context.Keywords.ToListAsync();

                    foreach (var kWord in kWords)
                    {
                        keywords.Add(kWord.Word);
                    }

                    secInfo = new(message2VMs, keywords);
                }

                return View(secInfo);
            }
            else
            {
                return RedirectToAction("LoginManagement", "Login");
            }
        }
    }
}
