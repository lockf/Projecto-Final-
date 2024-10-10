using ChatbotManagement.Common;
using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Models.VM;
using Microsoft.EntityFrameworkCore;

namespace ChatbotManagement.Controllers.Services.General
{
    public class ManagementService
    {
        private readonly AppDbContext _context;
        private readonly SolrService _solrService;

        public ManagementService(AppDbContext context, SolrService solrService)
        {
            _context = context;
            _solrService = solrService;
        }

        /// <summary>
        /// Verifies if exists any file in the DB
        /// </summary>
        /// <returns>True if exists any file, False if not.</returns>
        private bool FileExists()
        {
            return _context.StoredFiles.Any();
        }

        /// <summary>
        /// Get all files in the DB
        /// </summary>
        /// <returns>A list of files.</returns>
        public async Task<List<StoredFile3VM>?> GetAll()
        {
            if (FileExists())
            {
                List<StoredFile3VM> storedFile3VMs = new();

                var storedFiles = await _context.StoredFiles.ToListAsync();

                foreach (var sf in storedFiles)
                {
                    if (sf.FileName!.Equals(null))
                    {
                        sf.FileName = "No title";
                    }
                    StoredFile3VM mess = new(sf.SolrId, sf.FileName, sf.FileType, sf.Link!, sf.Available);
                    storedFile3VMs?.Add(mess);
                }
                return storedFile3VMs;
            }
            return null;
        }

        /// <summary>
        /// Get all files available to be suggested by the Chatbot
        /// </summary>
        /// <returns>List of files that can be suggested by the Chatbot.</returns>
        public async Task<List<StoredFile3VM>?> GetShown()
        {
            if (FileExists())
            {
                List<StoredFile3VM> storedFile3VMs = new();

                var storedFiles = await _context.StoredFiles.Where(e => e.Available.Equals(true)).ToListAsync();

                if (storedFiles != null)
                {
                    foreach (var sf in storedFiles)
                    {
                        if (sf.FileName!.Equals(null))
                        {
                            sf.FileName = "No title";
                        }
                        StoredFile3VM mess = new(sf.SolrId, sf.FileName, sf.FileType, sf.Link!, sf.Available);
                        storedFile3VMs?.Add(mess);
                    }
                    return storedFile3VMs;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all files not available to be suggested by the Chatbot
        /// </summary>
        /// <returns>List of files that cannot be suggested by the Chatbot.</returns>
        public async Task<List<StoredFile3VM>?> GetNotShown()
        {
            if (FileExists())
            {
                List<StoredFile3VM> storedFile3VMs = new();

                var storedFiles = await _context.StoredFiles.Where(e => e.Available.Equals(false)).ToListAsync();

                if (storedFiles != null)
                {
                    foreach (var sf in storedFiles)
                    {
                        if (sf.FileName!.Equals(null))
                        {
                            sf.FileName = "No title";
                        }
                        StoredFile3VM mess = new(sf.SolrId, sf.FileName, sf.FileType, sf.Link!, sf.Available);
                        storedFile3VMs?.Add(mess);
                    }
                    return storedFile3VMs;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all files that need to be approved by submitting their respective links
        /// </summary>
        /// <returns>List of files that need to be approved by submitting their respective links</returns>
        public async Task<List<StoredFile2VM>?> GetFilesToApprove()
        {
            if (FileExists())
            {
                List<StoredFile2VM> storedFile2VM = new();

                var files = await _context.StoredFiles.Where(e => e.Link == null || e.Link == string.Empty).ToListAsync();
                if (files != null)
                {
                    foreach (var f in files)
                    {
                        if (f.FileName!.Equals(null))
                        {
                            f.FileName = "No title";
                        }
                        StoredFile2VM sf = new(f.SolrId, f.FileName, f.FileType, f.Link!);
                        storedFile2VM.Add(sf);
                    }
                    return storedFile2VM;
                }
            }
            return null;
        }

        /// <summary>
        /// Insert the links of the files that need to be approved to the DB
        /// </summary>
        /// <param name="sf">Object with the information</param>
        public async Task PostFirstTable(List<StoredFile2VM> sf)
        {
            foreach (var ft in sf)
            {
                if (_solrService.UrlValidatorFailSafe(ft.Link))
                {
                    var storedFile = await _context.StoredFiles.Where(e => e.SolrId.Equals(ft.SolrId)).FirstOrDefaultAsync();
                    storedFile!.Link = ft.Link;
                    await _context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Chnage the links and/or availability of the files that are suggested by the Chatbot
        /// </summary>
        /// <param name="sf">Object with the information</param>
        public async Task PostSecondTable(List<StoredFile3VM> sf)
        {
            foreach (var st in sf)
            {
                var storedFile = await _context.StoredFiles.Where(e => e.SolrId.Equals(st.SolrId)).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(st.Link.Trim()))
                {
                    storedFile!.Link = st.Link;
                }
                storedFile!.Available = st.Show;
                await _context.SaveChangesAsync();
            }
        }
    }
}
