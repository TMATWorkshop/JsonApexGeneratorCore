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

		/*
		[HttpPost]
		public PostResponse Post(String className, String namedCredential, String requestJSON, String responseJSON)
		{
			PostResponse response = new PostResponse();
			response.handler = "public class className {}";
			return response;
		}
		*/
		[HttpPost]
		public String Post(String className, String namedCredential, String requestJSON, String responseJSON)
		{
			return "public class className {}";
		}

		//Generate Apex Code
		public class PostResponse {
			public String handler;
			public String handlerTest;
			public String mock;
			public String wrapper;
			public String wrapperTest;
		}
	}
}