using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace JsonApexGeneratorCore.Controllers
{
	[Route("api/[controller]")]
	public class ApexController : ControllerBase
	{
		// GET: api/values
		[HttpGet]
		public IEnumerable<string> Get()
		{
		    Console.WriteLine(Request.GetDisplayUrl());
		    Console.WriteLine(Request.GetEncodedUrl());

			return new[] { "value1", "value2" };
		}

		[HttpPost]
		public string Post()
		{
			
			return "value";
		}

		//Generate Apex Code
	}
}