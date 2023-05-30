using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Mvc;
using MyAutodeskAPS.Models;

namespace MyAutodeskAPS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly Aps aps;

        public record BucketObject(string name, string urn);

        public ModelsController(Aps aps)
        {
            this.aps = aps;
        }

        [HttpGet()]
        public async Task<IEnumerable<BucketObject>> GetModels()
        {
            var objects = await aps.GetObjects();

            return
                from obj in objects
                select new BucketObject(obj.ObjectKey, Aps.Base64Encode(obj.ObjectId));
        }

        [HttpGet("{urn}/status")]
        public async Task<TranslationStatus> GetModelStatus(string urn)
        {
            try
            {
                var status = await aps.GetTranslationStatus(urn);
                return status;
            }
            catch (ApiException e)
            {
                if (e.ErrorCode == 404)
                {
                    return new TranslationStatus("n/a", "", new List<string>());
                }
                else
                {
                    throw;
                }
            }
        }

        public class UploadModelForm
        {
            [FromForm(Name = "model-zip-entrypoint")]
            public string? Entrypoint { get; set; }

            [FromForm(Name ="model-file")]
            public IFormFile File { get; set; }
        }

        [HttpPost()]
        public async Task<BucketObject> UploadAndTranslateModel([FromForm] UploadModelForm form)
        {
            using (var stream = new MemoryStream())
            {
                await form.File.CopyToAsync(stream);
                stream.Position = 0;
                var obj = await aps.UploadModel(form.File.FileName, stream);
                var job = await aps.TranslateModel(obj.ObjectId, form.Entrypoint);

                return new BucketObject(obj.ObjectKey, job.Urn);
            }
        }
    }
}
