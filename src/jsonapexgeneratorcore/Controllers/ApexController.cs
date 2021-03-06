using System;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using JsonApexGeneratorCore.Helper;
namespace JsonApexGeneratorCore.Controllers
{
	[Route("api/[controller]")]
	public class ApexController : ControllerBase
	{

        private String ASSETPATH = "../src/jsonapexgeneratorcore/Assets/"; //Heroku
        //private String ASSETPATH = "../jsonapexgeneratorcore/Assets/"; //Local

		[HttpPost]
		public IActionResult GenerateFiles([FromBody] RequestParams requestParams) {
			Generator gen = new Generator(ASSETPATH, requestParams.className, requestParams.namedCredential, requestParams.urlExtension, requestParams.calloutMethod, requestParams.requestJSON, requestParams.responseJSON);
			List<Models.FileModel> fileModels = gen.generateFiles();

			//Compress to single Zip
			using (var compressedFileStream = new MemoryStream())
			{
				Byte[] metaTemplate = System.IO.File.ReadAllBytes(ASSETPATH + "meta.txt");
				//Create an archive and store the stream in memory.
				using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false)) {
					foreach (var fileModel in fileModels) {
						//Create a zip entry for class
						var zipEntry = zipArchive.CreateEntry(fileModel.name + ".cls");
						//Get the stream of the attachment
						using (var originalFileStream = new MemoryStream(fileModel.body))
						using (var zipEntryStream = zipEntry.Open()) {
							//Copy the attachment stream to the zip entry stream
							originalFileStream.CopyTo(zipEntryStream);
						}
						
						//Create a zip entry for meta of class
						var zipEntryMeta = zipArchive.CreateEntry(fileModel.name + ".cls-meta.xml");
						//Get the stream of the attachment
						using (var originalFileStream = new MemoryStream(metaTemplate))
						using (var zipEntryStream = zipEntryMeta.Open()) {
							//Copy the attachment stream to the zip entry stream
							originalFileStream.CopyTo(zipEntryStream);
						}
					}
				}

				return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = requestParams.className + ".zip" };
			}
		}

		public class RequestParams {
			public String className { get; set; }
			public String namedCredential { get; set; }
			public String calloutMethod { get; set; }
			public String requestJSON { get; set; }
			public String responseJSON { get; set; }
			public String urlExtension { get; set; }
		}
	}
}