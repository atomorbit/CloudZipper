using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipperUpper.Models
{
    //Simple class that stores metadata about a file as it's returned from the
    //HttpClient.GetByteArrayAsync()
    public class FileMeta
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}