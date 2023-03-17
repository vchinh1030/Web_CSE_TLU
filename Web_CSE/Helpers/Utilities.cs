
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web_CSE.Helpers
{
    public static class Utilities
    {
        public static int PAGE_SIZE = 20;
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "img/upload", sDirectory, newname);
                string path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "img/upload", sDirectory);
                if (!System.IO.Directory.Exists(path2))
                {
                    System.IO.Directory.CreateDirectory(path2);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if(!supportedTypes.Contains(fileExt.ToLower())) 
                {
                    return null;
                }
                else
                {
                    using (var stream=new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
