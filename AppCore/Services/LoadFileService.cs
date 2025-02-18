using Microsoft.Extensions.Hosting;
using System;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class LoadFileService
    {
        private IHostEnvironment environment;

        public LoadFileService(IHostEnvironment hostingEnvironment)
        {
            environment = hostingEnvironment;
        }
        public string LoadFileAsString(string fileName, string defaultContent = "")
        {
            var rootPath = environment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, fileName);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : defaultContent;
        }
    }
}
