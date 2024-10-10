using ChatbotManagement.Models;
using SolrNet;
using ChatbotManagement.Common;
using ChatbotManagement.Models.VM;
using Microsoft.EntityFrameworkCore;
using LightVx;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace ChatbotManagement.Controllers.Services.API
{
    public class SolrService
    {
        private readonly ISolrReadOnlyOperations<MySolrModel> _solr;
        private readonly AppDbContext _context;

        public SolrService(ISolrReadOnlyOperations<MySolrModel> solr, AppDbContext context)
        {
            _solr = solr;
            _context = context;
        }

        /// <summary>
        /// Verifies if exists any file info stored in the DB.
        /// </summary>
        /// <returns> Returns, true, if there is or, false, if not. </returns>
        public bool StoredFilesExist()
        {
            return _context.StoredFiles.Any();
        }

        /// <summary>
        /// Verifies if exists a file info stored in the DB with indicated SolrId.
        /// </summary>
        /// <param name="SolrId"> File SolrId to search for in the DB</param>
        /// <returns>Returns, true, if there is or, false, if not.</returns>
        public bool StoredFileExist(string SolrId)
        {
            return _context.StoredFiles.Any(e => e.SolrId.Equals(SolrId));
        }

        /// <summary>
        /// Verifies if exists a file info stored in the DB with indicated Id.
        /// </summary>
        /// <param name="id"> File Id to search for in the DB</param>
        /// <returns>Returns, true, if there is or, false, if not.</returns>
        public bool StoredFileExist(int id)
        {
            return _context.StoredFiles.Any(e => e.Id.Equals(id));
        }

        /// <summary>
        /// Searches for the name of the file based on the Id.
        /// </summary>
        /// <param name="id">Of the file name</param>
        /// <returns>The name of the file or an empty string.</returns>
        public string StoredFileName(int id)
        {
            if (StoredFileExist(id))
            {
                return _context.StoredFiles.First(e => e.Id.Equals(id)).FileName!;
            }
            return string.Empty;
        }

        /// <summary>
        /// Verifies which files are to be added to the DB from the Solr instance
        /// </summary>
        /// <param name="StoredFiles"> A List of info from each file to add to DB </param>
        /// <returns> A List of specific info from the files to be added to the DB </returns>
        public List<StoredFileVM> CheckStoredFilesToAddToDB(List<MySolrModel> StoredFiles)
        {
            List<StoredFileVM> toReturn = new();
            if (StoredFilesExist())
            {
                foreach (var SolrItem in StoredFiles)
                {
                    if (!StoredFileExist(SolrItem.SolrId))
                    {
                        StoredFileVM toAdd = new(SolrItem.SolrId, SolrItem.Keywords);
                        toReturn.Add(toAdd);
                    }
                }
            }
            else
            {
                foreach (var SolrItem in StoredFiles)
                {
                    StoredFileVM toAdd = new(SolrItem.SolrId, SolrItem.Keywords);
                    toReturn.Add(toAdd);
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Obtains a List of the Solr Id of the files in the DB.
        /// </summary>
        /// <returns> A List of strings</returns>
        /// <exception cref="Exception"> If the Database is empty an exception is launched</exception>
        public async Task<List<string>> GetSolrIdList()
        {
            if (StoredFilesExist())
            {
                return await _context.StoredFiles.Select(n => n.SolrId).ToListAsync();
            }
            throw new Exception("Empty DB");
        }

        /// <summary>
        /// Obtains a List with the info of the stored files that have a link in the DB
        /// </summary>
        /// <param name="available">Indicates wether to return the files available or not (if the link was already inserted or not).</param>
        /// <returns>A List with the info of the stored files</returns>
        /// <exception cref="Exception">If the Database is empty an exception is launched</exception>
        public async Task<List<StoredFile>> GetListOfFiles(bool available)
        {
            if (StoredFilesExist())
            {
                return await _context.StoredFiles.Where(n => n.Available.Equals(available)).ToListAsync();
            }
            throw new Exception("Empty DB");
        }

        /// <summary>
        /// 2nd url validator for confirmation.
        /// </summary>
        /// <param name="url">To validate</param>
        /// <returns>True, if is valid, false, if it's not.</returns>
        public bool UrlValidatorFailSafe(string url)
        {
            Regex validateDateRegex = new("https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)");

            if (validateDateRegex.IsMatch(url))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates and adds the link to the stored file information in the DB and allows the file to be suggested.
        /// </summary>
        /// <param name="link">The link to add to the file information in the DB</param>
        /// <param name="StoredFileId">Id of the file to be added the link.</param>
        /// <exception cref="Exception">An exception is launched if there is no element with th e Id provided.</exception>
        public async Task EditFileLinkAndAvailability(string link, int StoredFileId)
        {
            if (StoredFileExist(StoredFileId))
            {
                var storedFile = await _context.StoredFiles.FindAsync(StoredFileId);
                bool linkIsValid = false;

                // 1st validator
                Validator.Eval(link).IsUrl().Success(() => { linkIsValid = true; });

                // 2nd validator
                // Failsafe in case the previous Validator Fails
                if (!linkIsValid)
                {
                    linkIsValid = UrlValidatorFailSafe(link);
                }

                if (storedFile != null && linkIsValid)
                {
                    storedFile.Link = link;
                    storedFile.Available = true;

                    await _context.SaveChangesAsync();
                }
            }
            throw new Exception("There is no file with this Id in the DB");
        }

        /// <summary>
        /// Makes a query to the Solr server
        /// </summary>
        /// <param name="searchString">Query for the Solr server</param>
        /// <returns>The query results</returns>
        public async Task<List<MySolrModel>> Search(string searchString)
        {
            try
            {
                if (searchString.Equals("*:*"))
                {
                    var results = await _solr.QueryAsync(SolrQuery.All);
                    return results;
                }
                else
                {
                    var solrQuery = new SolrQuery($"keywords:{searchString}");
                    var results = await _solr.QueryAsync(solrQuery);

                    return results;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Solr search error: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Adds File Info from Solr instance to DB except the Keywords.
        /// </summary>
        /// <param name="mySolrList">A List of info from each file to add to DB</param>
        /// <exception cref="Exception">If the Database is empty an exception is launched</exception>
        public async Task SolrFilesToDB(List<MySolrModel> mySolrList)
        {
            if (StoredFilesExist())
            {
                foreach (var SolrItem in mySolrList)
                {
                    if (!StoredFileExist(SolrItem.SolrId))
                    {
                        // Adds file if not already not in the DB
                        string fileType = SolrItem.FileType[0].Replace("application/", "");
                        if (SolrItem.FileName.IsNullOrEmpty())
                        {
                            SolrItem.FileName = "No title";
                        }
                        var SolrInfo = new StoredFile(SolrItem.SolrId, SolrItem.FileName, fileType);
                        _context.StoredFiles.Add(SolrInfo);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else if (!StoredFilesExist())
            {
                foreach (var SolrItem in mySolrList)
                {
                    string fileType = SolrItem.FileType[0].Replace("application/", "");
                    if (SolrItem.FileName.IsNullOrEmpty())
                    {
                        SolrItem.FileName = "No title";
                    }
                    var SolrInfo = new StoredFile(SolrItem.SolrId, SolrItem.FileName, fileType);
                    _context.StoredFiles.Add(SolrInfo);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Error SolrFilesToDB");
            }
        }

        /// <summary>
        /// Adds the Keywords to DB from the files info added to the DB
        /// </summary>
        /// <param name="mySolrList">A List of info from each file to add to DB</param>
        public async Task SolrKeywordsToDB(List<StoredFileVM> mySolrList)
        {
            if (mySolrList.Count > 0)
            {
                foreach (var FileKeywordsToAdd in mySolrList)
                {
                    string[] Keywords = Regex.Split(FileKeywordsToAdd.Keyword, @";|,\s*");
                    string[] KeywordsP = Array.Empty<string>();
                    foreach (string keyword in Keywords)
                    {
                        KeywordsP = KeywordsP.Append(keyword.ToLower()).ToArray();
                    }
                    foreach (string keyword in KeywordsP)
                    {
                        if (!KeywordExistsInDB(keyword))
                        {
                            var kWord = new Keyword(keyword);
                            _context.Keywords.Add(kWord);

                            await _context.SaveChangesAsync();
                        }
                        // Adds the foreign keys to FileKeyword table
                        int kId = KeywordId(keyword);
                        int sfId = StoredFileId_(FileKeywordsToAdd.SolrId);
                        var FileKeywordent = new FileKeyword(kId, sfId);
                        _context.FileKeywords.Add(FileKeywordent);

                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Searches the Keyword Id from the DB based on the input
        /// </summary>
        /// <param name="word">Keyword from which the Id is wanted</param>
        /// <returns>The Id of Keyword</returns>
        public int KeywordId(string word)
        {
            return _context.Keywords.First(c => c.Word.Equals(word)).Id;
        }

        /// <summary>
        /// Searches the File Info Id from de DB based on the input
        /// </summary>
        /// <param name="SolrId">Solr Id of the file from whhich the Id is wanted</param>
        /// <returns>The Id of the File Info</returns>
        public int StoredFileId_(string SolrId)
        {
            return _context.StoredFiles.First(c => c.SolrId.Equals(SolrId)).Id;
        }

        /// <summary>
        /// Verifies if Keyword exists in the DB
        /// </summary>
        /// <param name="word">Keyword to search for in the DB</param>
        /// <returns>Returns, true, if there is or, false, if not.</returns>
        public bool KeywordExistsInDB(string word)
        {
            return _context.Keywords.Any(e => e.Word.Equals(word));
        }

        /// <summary>
        /// Verifies if Keywords exist in the DB
        /// </summary>
        /// <returns>Returns, true, if there is or, false, if not.</returns>
        public bool KeywordExistsInDB()
        {
            return _context.Keywords.Any();
        }
    }
}
