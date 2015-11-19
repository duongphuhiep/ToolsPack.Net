using System;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ToolsPack.Log4net;
using log4net;
using System.Text;
using RestSharp;
using System.IO;
using System.Diagnostics;
using RestSharp.Authenticators;

namespace DiversDoNet.Tests
{
	[TestClass]
	public class SendHttpRequestTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(SendHttpRequestTests));
		private static readonly string chromeExe = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod]
		public void Send_Post_Form()
		{
			var client = new RestClient("http://localhost/notifx/?gk1=gv1&gk2=gv2");

			//client.Authenticator = new HttpBasicAuthenticator("hiep", "lemonway");
			client.Authenticator = new SimpleAuthenticator("hiepkey", "hiep", "lemonwaykey", "lemonway");

			var request = new RestRequest() {
				Method = Method.POST
			};
			request.AddParameter("pk1", "pv1");
			request.AddParameter("pk2", "pv2");
			request.AddHeader("hk1", "hv1");
			request.AddHeader("hk2", "hv2");

			var response = client.Execute(request);


			Log.InfoFormat("ResponseStatus: {0}, {1}", response.ResponseStatus, (int)response.ResponseStatus);
			Log.InfoFormat("StatusCode: {0}, {1}", response.StatusCode, (int)response.StatusCode);
			Log.InfoFormat("StatusDescription: {0}", response.StatusDescription);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				using (var f = new StreamWriter("response_post_form.html"))
				{
					f.Write(response.Content);
				}
				Process.Start(chromeExe, "response_post_form.html");
			}
        }

		[TestMethod]
		public void Send_Post_Text()
		{
			var client = new RestClient("http://localhost/notif/?gk1=gv1&gk2=gv2");

			client.Authenticator = new HttpBasicAuthenticator("hiep", "lemonway");

			var request = new RestRequest()
			{
				Method = Method.POST
			};
			request.AddHeader("hk1", "hv1");
			request.AddHeader("hk2", "hv2");

			request.AddParameter("text/plain", "je suis text, not json", ParameterType.RequestBody);
			//request.AddParameter("pk1", "pv1");
			//request.AddParameter("pk2", "pv2");


			var response = client.Execute(request);
			Log.Info(response.StatusCode);

			using (var f = new StreamWriter("response_post_text.html"))
			{
				f.Write(response.Content);
			}

			Process.Start(chromeExe, "response_post_text.html");
		}

		[TestMethod]
		public void Send_Get()
		{
			var client = new RestClient("http://localhost/notif/?gk1=gv1&gk2=gv2");

			client.Authenticator = new HttpBasicAuthenticator("hiep", "lemonway");

			var request = new RestRequest()
			{
				Method = Method.POST
			};
			request.AddParameter("pk1", "pv1");
			request.AddParameter("pk2", "pv2");
			request.AddHeader("hk1", "hv1");
			request.AddHeader("hk2", "hv2");

			var response = client.Execute(request);
			Log.Info(response.StatusCode);

			using (var f = new StreamWriter("response_get.html"))
			{
				f.Write(response.Content);
			}

			Process.Start(chromeExe, "response_get.html");
		}

	}
}
