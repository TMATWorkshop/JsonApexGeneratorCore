using System;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using JsonApexGeneratorCore.Models;
using JsonApexGeneratorCore.Helper;
namespace JsonApexGeneratorCore.Controllers
{
	[Route("api/[controller]")]
	public class ApexController : ControllerBase
	{
		[HttpPost]
		public IActionResult GenerateFiles(String className, String namedCredential, String calloutMethod, String requestJSON, String responseJSON) {
			Console.WriteLine("Params: " + className);
			Console.WriteLine("Params: " + namedCredential);
			Console.WriteLine("Params: " + calloutMethod);
			Console.WriteLine("Params: " + requestJSON);
			Console.WriteLine("Params: " + responseJSON);
			Generator gen = new Generator(className, namedCredential, calloutMethod, requestJSON, responseJSON);
			List<Models.FileModel> fileModels = gen.generateFiles();

			//Compress to single Zip
			using (var compressedFileStream = new MemoryStream())
			{
				//Create an archive and store the stream in memory.
				using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false)) {
					foreach (var fileModel in fileModels) {
						//Create a zip entry for each attachment
						var zipEntry = zipArchive.CreateEntry(fileModel.name);

						//Get the stream of the attachment
						using (var originalFileStream = new MemoryStream(fileModel.body))
						using (var zipEntryStream = zipEntry.Open()) {
							//Copy the attachment stream to the zip entry stream
							originalFileStream.CopyTo(zipEntryStream);
						}
					}
				}

				return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = className + ".zip" };
			}
		}


		[HttpPost]
		[Route("wrapper")]
		public IActionResult GenerateWrapper(String className, String namedCredential, String requestJSON, String responseJSON)
		{



			Byte[] pdfBytes = System.Text.Encoding.ASCII.GetBytes("public class test { }");
			return new FileContentResult(pdfBytes, "text/plain");
		}
	}
}