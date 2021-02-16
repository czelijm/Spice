using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public class ModelUtilities
    {
        public static async Task<byte[]> FormFileToByteArrayAsync(IFormFile file) 
        {
            //image -> streamOfBytes
            byte[] p1 = null;
            using (var fs1 = file.OpenReadStream()) // start reading the file 
            {
                using (var ms1 = new MemoryStream())//memory stream
                {
                    await fs1.CopyToAsync(ms1);
                    p1 = ms1.ToArray();
                }
            }
            return p1;
        }
        public static async Task<byte[]> FileToByteArrayAsync(string filePath) 
        {
            //image -> streamOfBytes
            byte[] p1 = null;
            using (var fs1 = File.OpenRead(filePath)) // start reading the file 
            {
                using (var ms1 = new MemoryStream())//memory stream
                {
                    await fs1.CopyToAsync(ms1);
                    p1 = ms1.ToArray();
                }
            }
            return p1;
        }



    }
}
