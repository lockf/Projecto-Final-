using ChatbotManagement.Common;
using ChatbotManagement.Models;
using ChatbotManagement.Models.VM;
using Microsoft.EntityFrameworkCore;

namespace ChatbotManagement.Controllers.Services.API
{
    public class FeedbackService
    {
        private readonly AppDbContext _context;
        private readonly SolrService _solrService;

        public FeedbackService(AppDbContext context, SolrService solrService)
        {
            _context = context;
            _solrService = solrService;
        }

        /// <summary>
        /// Verifies if exists any feedback in the database
        /// </summary>
        /// <returns>True if it exists or false if it doesn't.</returns>
        public bool FeedBacksExist()
        {
            return _context.Feedbacks.Any();
        }

        /// <summary>
        /// Gets the feedbacks in the database
        /// </summary>
        /// <returns>Object with the values to be sent if it is successfull or throws exception if it fails</returns>
        /// <exception cref="Exception">Throws an exception if the database if empty (no feedbacks)</exception>
        public async Task<List<FeedbackVM>> GetFeedbacks()
        {
            if (FeedBacksExist())
            {
                List<FeedbackVM> list = new();
                var feedbacks = await _context.Feedbacks.ToListAsync();

                foreach (var feedback in feedbacks)
                {
                    string filename = _solrService.StoredFileName(feedback.StoredFileId);
                    FeedbackVM feedbackVM = new(feedback.Opinion, feedback.Conclusion, feedback.StoredFileId, filename);
                    list.Add(feedbackVM);
                }
                return list;
            }
            throw new Exception("Empty database table");
        }

        /// <summary>
        /// Gets the feedbacks in the database that have the indicated FileId
        /// </summary>
        /// <param name="FileId">Base value used to search</param>
        /// <returns>An object with the search result, or throws an Exception if the database has no feedbacks</returns>
        /// <exception cref="Exception">Exception is thrown if the database has no feedbacks</exception>
        public async Task<List<FeedbackVM>> GetFeedbacks(int FileId)
        {
            if (FeedBacksExist())
            {
                List<FeedbackVM> list = new();
                var feedbacks = await _context.Feedbacks.Where(e => e.StoredFileId.Equals(FileId)).ToListAsync();

                foreach (var feedback in feedbacks)
                {
                    string filename = _solrService.StoredFileName(feedback.StoredFileId);
                    FeedbackVM feedbackVM = new(feedback.Opinion, feedback.Conclusion, feedback.StoredFileId, filename);
                    list.Add(feedbackVM);
                }
                return list;
            }
            throw new Exception("Empty database table");
        }

        /// <summary>
        /// Adds user feedback to the database and determines if the feedback opinion is Positive, Neutral or Negative using machine learning
        /// </summary>
        /// <param name="feedback">Feedback given by the user</param>
        /// <param name="id">Id of the File to which the feedback belongs to</param>
        /// <returns>A string with the result of the machine learning opinion: Positive, Neutral or Negative </returns>
        public async Task<string> AddFeedback(Feedback2VM feedback, int id)
        {
            //Load sample data
            var sampleData = new MLModel.ModelInput()
            {
                Opinion_Text = feedback.Opinion,
            };

            //Load model and predict output
            var result = MLModel.Predict(sampleData);
            var feed = new Feedback(feedback.Opinion, result.PredictedLabel, id);
            _context.Feedbacks.Add(feed);

            await _context.SaveChangesAsync();

            return result.PredictedLabel;
        }
    }
}
