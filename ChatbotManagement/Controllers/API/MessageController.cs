using ChatbotManagement.Controllers.Services.API;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using ChatbotManagement.Models.VM;
using ChatbotManagement.Models.in_VM;

namespace ChatbotManagement.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;
        private readonly FeedbackService _feedbackService;

        public MessageController(MessageService messageService, FeedbackService feedbackService)
        {
            _messageService = messageService;
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Adds the message to the database
        /// </summary>
        /// <param name="message">Message created by the Chatbot or the user</param>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /Todo
        ///         {
        ///             "Text": "BlahBlahBlah",
        ///             "ChannelId": "d1as89d1sad1a96s8d",
        ///             "ConversationId": "sgdfh4951gdfg894fdg"
        ///         }
        /// </remarks>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPost("createMessage")]
        [SwaggerResponse(200, "Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Create Message", Description = "Create Message on DataBase")]
        public async Task<IActionResult> PostMessage([FromBody]Message3VM message)
        {
            try
            {
                await _messageService.AddMessage(message);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Adds the user feedback on the file suggested to the database
        /// </summary>
        /// <param name="post">Object with the values to be added</param>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /Todo
        ///         {
        ///             "Message3VM":   {
        ///                                 "Text": "BlahBlahBlah",     
        ///                                 "ChannelId": "d1as89d1sad1a96s8d",
        ///                                 "ConversationId": "sgdfh4951gdfg894fdg"
        ///                             },
        ///             "Feedback2VM":  {
        ///                                 "Opinion": "BlahBlahBlah",     
        ///                                 "FileId": "d1as89d1sad1a96s8d",
        ///                             }
        ///         }
        /// </remarks>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPost("createMessageAndFeedback")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Create Message and Feedback", Description = "Create Message and Feedback on DataBase")]
        public async Task<IActionResult> PostMessageAndFeedback([FromBody]PostFeedMessVM post)
        {
            try
            {
                var id = int.Parse(post.Feedback2VM.FileId);

                await _messageService.AddMessage(post.Message3VM, id);
                var feedback = await _feedbackService.AddFeedback(post.Feedback2VM, id);

                return Ok(feedback);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
