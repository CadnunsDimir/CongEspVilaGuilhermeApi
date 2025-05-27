using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

        public T? LoadFileAsJson<T>(string fileName)
        {
            var jsonData = LoadFileAsString(fileName);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        internal void SaveFileAsJson(string fileName, object data)
        {
            //gravar o arquivo json
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

            var rootPath = environment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, fileName);

            //verifica se o arquivo existe, se não existir cria um novo
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
            }
            //grava o arquivo json
            using (var writer = new StreamWriter(fullPath, false))
            {
                writer.Write(jsonData);
            }
        }
    }
}
