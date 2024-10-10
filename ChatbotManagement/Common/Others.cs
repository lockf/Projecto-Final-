using ChatbotManagement.Models;
using SolrNet;

namespace ChatbotManagement.Common
{
    public class Others
    {
        private readonly ISolrOperations<MySolrModel> _solr;

        public Others(ISolrOperations<MySolrModel> solr)
        {
            _solr = solr;
        }

        /// <summary>
        /// Checks if there is a connection with the Apache Solr Server
        /// </summary>
        /// <returns>True if there is a connection, False if there isn't.</returns>
        public bool CheckSolrConnection()
        {
            try
            {
                // Just sending a ping request to verify connection
                _solr.Ping();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while connecting to Solr: {ex.Message}");
                return false;
            }
        }
    }
}
