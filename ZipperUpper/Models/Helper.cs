using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ionic.Zip;

namespace ZipperUpper.Models
{
    public class Helper
    {
        //Self Explanatory
        public async Task<ZipFile> CreateZipFormUrls(List<string> urlList)
        {
            using (var zip = new ZipFile())
            {
                //Download the files, and ensure file names are unique as a ZipFile is a directory
                //and will throw an exception if two files with the same name are added
                var files = EnsureUniqueFileNames(await ReturnFileDataAsync(urlList));

                foreach (var file in files)
                {
                    zip.AddEntry(file.FileName, file.FileData);
                }

                return zip;
            }
        }

        //Super Badass Async File Downloader. Simple easy to read code
        private Task<FileMeta[]> ReturnFileDataAsync(IEnumerable<string> urls)
        {
            var client = new HttpClient();
            return Task.WhenAll(urls.Select(async url => new FileMeta
            {
                FileName = Path.GetFileName(url),
                FileData = await client.GetByteArrayAsync(url),
            }));
        }

        //Ensure all filenames are unique, and append incremental (#) to each duplicate file name
        private IEnumerable<FileMeta> EnsureUniqueFileNames(IEnumerable<FileMeta> fileMetas)
        {
            var returnList = new List<FileMeta>();
            foreach (var file in fileMetas)
            {
                int count = 0;
                string originalFileName = file.FileName;
                while (returnList.Any(fileMeta => fileMeta.FileName == file.FileName))
                {
                    string fileNameOnly = Path.GetFileNameWithoutExtension(originalFileName);
                    string extension = Path.GetExtension(file.FileName);
                    file.FileName = string.Format("{0}({1}){2}", fileNameOnly, count, extension);
                    count++;
                }

                returnList.Add(file);
            }
            return returnList;
        }

        //Generate a number of specific length
        public string GenerateNumber()
        {
            var random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString(CultureInfo.InvariantCulture);
            }
            return r;
        }
    }
}