using ChatbotManagement.Common;
using ChatbotManagement.Models.out_VM;
using Microsoft.EntityFrameworkCore;

namespace ChatbotManagement.Controllers.Services.General
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Determines if exists any keywords in the database
        /// </summary>
        /// <returns>True if it exist any keywords, false if it doesn't</returns>
        public bool ExistsKeywords()
        {
            return _context.Keywords.Any();
        }

        /// <summary>
        /// Calculates the total number of keywords in the database
        /// </summary>
        /// <returns>The total number of keywords in the database</returns>
        public async Task<int> TotalKeywordsInDB()
        {
            var totalKeywords = await _context.Keywords.CountAsync();
            return totalKeywords;
        }

        /// <summary>
        /// Calculates the total number of files in the database
        /// </summary>
        /// <returns>The total number of files in the database</returns>
        public async Task<int> TotalFilesInDB()
        {
            var totalFiles = await _context.StoredFiles.CountAsync();
            return totalFiles;
        }

        /// <summary>
        /// Calculates the total number of feedbacks in the database
        /// </summary>
        /// <returns>The total number of feedbacks in the database</returns>
        public async Task<int> TotalFeedbackInDB()
        {
            var totalFeedbacks = await _context.Feedbacks.CountAsync();
            return totalFeedbacks;
        }

        /// <summary>
        /// Calculates the total number of messages in the database
        /// </summary>
        /// <returns>The total number of messages in the database</returns>
        public async Task<int> TotalMessagesInDB()
        {
            var totalMessages = await _context.Messages.CountAsync();
            return totalMessages;
        }

        /// <summary>
        /// Calculates the total number of feedbacks in the database with the conclusion indicated
        /// </summary>
        /// <param name="conclusion">The value use to restrict the results, possible values are: Positive, Neutral, Negative</param>
        /// <returns>The total number of feedbacks in the database with the conclusion indicated</returns>
        public async Task<int> TotalFeedbackInDB(string conclusion)
        {
            var totalFeedbacks = await _context.Feedbacks.Where(e => e.Conclusion.Equals(conclusion)).CountAsync();
            return totalFeedbacks;
        }

        /// <summary>
        /// Calculates the total number of files in the database with the indicated restriction, if the files are suggested or not to the user
        /// </summary>
        /// <param name="shown">The boolean value used to restrict the results based on whether or not the files are suggested to the user</param>
        /// <returns>The total number of files in the database with the indicated restriction, if the files are suggested or not to the user</returns>
        public async Task<int> TotalFilesInDB(bool shown)
        {
            var totalFiles = await _context.StoredFiles.Where(e => e.Available.Equals(shown)).CountAsync();
            return totalFiles;
        }

        /// <summary>
        /// Obtains all the info needed for the Dashboard page
        /// </summary>
        /// <returns>The object with all the results</returns>
        public async Task<DashboardVM> GetDashboardInfo()
        {
            // General Totals
            var totalKeywords = await TotalKeywordsInDB();
            var totalFiles = await TotalFilesInDB();
            var totalFeedbacks = await TotalFeedbackInDB();
            var totalMessages = await TotalMessagesInDB();

            // Feedback Totals
            var totalFeedbacksNeg = await TotalFeedbackInDB("Negative");
            var totalFeedbacksNeutral = await TotalFeedbackInDB("Neutral");
            var totalFeedbacksPos = await TotalFeedbackInDB("Positive");
            decimal tFeedbacksNegDec = Convert.ToDecimal(totalFeedbacksNeg);
            decimal tFeedbacksNeutralDec = Convert.ToDecimal(totalFeedbacksNeutral);
            decimal tFeedbacksPosDec = Convert.ToDecimal(totalFeedbacksPos);


            var totalFeedbacksNegPer = 0.00m;
            var totalFeedbacksNeutralPer = 0.00m;
            var totalFeedbacksPosPer = 0.00m;

            if (totalFeedbacks != 0)
            {
                totalFeedbacksNegPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(tFeedbacksNegDec, totalFeedbacks), 100m), 2);
                totalFeedbacksNeutralPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(tFeedbacksNeutralDec, totalFeedbacks), 100m), 2);
                totalFeedbacksPosPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(tFeedbacksPosDec, totalFeedbacks), 100m), 2);
            } 

            // Files Totals
            var totalFilesShown = await TotalFilesInDB(true);
            var totalFilesNotShown = await TotalFilesInDB(false);
            decimal totalFilesShownDec = Convert.ToDecimal(totalFilesShown);
            decimal totalFilesNotShownDec = Convert.ToDecimal(totalFilesNotShown);

            var totalFilesShownPer = 0.00m;
            var totalFilesNotShownPer = 0.00m;

            if (totalFiles != 0)
            {
                totalFilesShownPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(totalFilesShownDec, totalFiles), 100m), 2);
                totalFilesNotShownPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(totalFilesNotShownDec, totalFiles), 100m), 2);
            }

            DashboardVM dashboard = new(totalKeywords, totalFiles, totalFeedbacks, totalMessages, totalFeedbacksNeg, totalFeedbacksNeutral, totalFeedbacksPos,
                totalFeedbacksNegPer, totalFeedbacksNeutralPer, totalFeedbacksPosPer, totalFilesNotShown, totalFilesShown, totalFilesNotShownPer, totalFilesShownPer);

            return dashboard;
        }
    }
}
