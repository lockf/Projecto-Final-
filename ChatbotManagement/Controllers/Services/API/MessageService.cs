using ChatbotManagement.Common;
using ChatbotManagement.Models.VM;
using ChatbotManagement.Models;
using Microsoft.EntityFrameworkCore;
using ChatbotManagement.Models.out_VM;

namespace ChatbotManagement.Controllers.Services.API
{
    public class MessageService
    {
        private readonly AppDbContext _context;
        private readonly SolrService _solrService;

        public MessageService(AppDbContext context, SolrService solrService)
        {
            _context = context;
            _solrService = solrService;
        }

        /// <summary>
        /// Adds Messages to the DB
        /// </summary>
        /// <param name="Messages">List of messages to be stored in the DB</param>
        public async Task AddMessages(List<Message3VM> Messages)
        {
            if (Messages.Count > 0)
            {
                foreach (var message in Messages)
                {
                    var mess = new Message(message.Text, DateTime.Now, message.ChannelId, message.ConversationId, null!);
                    _context.Messages.Add(mess);
                }
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a Message to the DB
        /// </summary>
        /// <param name="Message">Message to be stored in the DB</param>
        public async Task AddMessage(Message3VM Message)
        {
            var mess = new Message(Message.Text, DateTime.Now, Message.ChannelId, Message.ConversationId, null!);
            _context.Messages.Add(mess);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a Message to the DB
        /// </summary>
        /// <param name="Message">Message to be stored in the DB</param>
        public async Task AddMessage(Message3VM Message, int fileId)
        {
            var mess = new Message(Message.Text, DateTime.Now, Message.ChannelId, Message.ConversationId, fileId);
            _context.Messages.Add(mess);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtains a List of the Messages from the DB based on the Channel Id and Conversation Id.
        /// </summary>
        /// <param name="ChannelId">Id of the channel</param>
        /// <param name="ConversationId">Id of the Conversation</param>
        /// <returns> A List of Message info</returns>
        /// <exception cref="Exception">If the Database is empty an exception is launched</exception>
        public async Task<List<MessageVM>> GetMessagesForChannel(string ChannelId, string ConversationId)
        {
            if (MessagesExist())
            {
                List<MessageVM> Messages = new();
                var messages = await _context.Messages.Where(e => e.ChannelId.Equals(ChannelId) && e.ConversationId.Equals(ConversationId)).ToListAsync();
                foreach (var message in messages)
                {
                    var mess = new MessageVM(message.Text, message.Date, message.ChannelId, message.ConversationId);
                    Messages.Add(mess);
                }
                return Messages;
            }
            throw new Exception("Empty DB");
        }

        /// <summary>
        /// Gets a list of the Ids of the files from the Solr Ids.
        /// </summary>
        /// <param name="SolrIds">Solr Ids of the stored files</param>
        /// <returns>A list of Ids</returns>
        /// <exception cref="Exception">If the Database is empty an exception is launched</exception>
        public async Task<List<int>> GetSuggestedFilesSolrId(List<string> SolrIds)
        {
            if (_solrService.StoredFilesExist())
            {
                List<int> sfIds = new();

                foreach (var solrId in SolrIds)
                {
                    var id = await _context.StoredFiles.Where(e => e.SolrId.Equals(solrId)).FirstOrDefaultAsync();
                    sfIds.Add(id!.Id);
                }
                return sfIds;
            }
            throw new Exception("Empty DB");
        }

        /// <summary>
        /// Sorts the files to suggest the most probable one wanted (considering existent feedback)
        /// </summary>
        /// <param name="Ids">A list of Stored File Ids to look for</param>
        /// <returns>A list with the File Id and the link for the file</returns>
        public List<StoredFileVM_out> FilesSuggestionSort(List<int> Ids)
        {
            List<StoredFileVM_out> res = new();
            List<SortSuggestVM> sortIt = new();

            // Calculates accuracy and adds it to a tupple
            foreach (var id in Ids)
            {
                var sf = _context.StoredFiles.Find(id);
                var acc = GetFeedback(id);
                SortSuggestVM sfOut = new(id, acc, sf!.Link!);
                sortIt.Add(sfOut);
            }

            var sorted = sortIt.OrderByDescending(e => e.Accuracy).ToList();

            foreach (var sf in sorted)
            {
                StoredFileVM_out sfOut = new(sf.Id, sf.Link);
                res.Add(sfOut);
            }

            return res;
        }

        /// <summary>
        /// Calculates the accuracy of the feedback by dividing de positive ones by the negative ones, special cases values defined.
        /// </summary>
        /// <param name="FileId">Id of the Stored File</param>
        /// <returns>The value of the accuracy calculated</returns>
        /// <exception cref="Exception">If the Database is empty an exception is launched</exception>
        public float GetFeedback(int FileId)
        {
            if (FeedbackExist())
            {
                var positiveFeedback = _context.Feedbacks.Where(e => e.StoredFileId.Equals(FileId) && e.Conclusion.Equals("Positive")).Count();
                var negativeFeedback = _context.Feedbacks.Where(e => e.StoredFileId.Equals(FileId) && e.Conclusion.Equals("Negative")).Count();

                if (negativeFeedback == 0)
                {
                    return (float)1.0;
                }
                else
                {
                    float fpositiveFeedback = positiveFeedback;
                    float fnegativeFeedback = negativeFeedback;
                    return positiveFeedback / negativeFeedback;
                }
            }
            else
            {
                return (float)0.0;
            }
        }

        /// <summary>
        /// Verifies if exists any Messages stored in the DB.
        /// </summary>
        /// <returns> Returns, true, if there is or, false, if not. </returns>
        public bool MessagesExist()
        {
            return _context.Messages.Any();
        }

        /// <summary>
        /// Verifies if exists any Messages stored in the DB.
        /// </summary>
        /// <returns> Returns, true, if there is or, false, if not. </returns>
        public bool MessagesExist(int id)
        {
            return _context.Messages.Any(e => e.Id.Equals(id));
        }

        /// <summary>
        /// Verifies if exists any Feedbacks stored in the DB.
        /// </summary>
        /// <returns> Returns, true, if there is or, false, if not. </returns>
        public bool FeedbackExist()
        {
            return _context.Feedbacks.Any();
        }
    }
}
