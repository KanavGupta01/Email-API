using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using EmailSenderV5.Models;

namespace EmailSenderV5.Controllers
{
    public class MailController : Controller
    {
        private static string apikey = Environment.GetEnvironmentVariable("sendgridapi");
        private static SendGridClient client = new SendGridClient(apikey);

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                var response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "templates?generations=dynamic");

                if (!response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(response.StatusCode);
                }

                var templates = JsonConvert.DeserializeObject<GetTemplates>(response.Body.ReadAsStringAsync().Result);
                return View(templates);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Template(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "templates/" + id);
                var templates = JsonConvert.DeserializeObject<ViewTemplate>(response.Body.ReadAsStringAsync().Result);

                if (!response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(response.StatusCode);
                }

                return Content(templates.versions[0].html_content);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Send(FormCollection formCollection)
        {
            try
            {
                var request = new Dictionary<string, string>();
                foreach (var key in formCollection.AllKeys)
                {
                    var value = formCollection[key];
                    request.Add(key, value);
                }

                if (request["id"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var status = await SendTemplateMail(request);

                return new HttpStatusCodeResult(status);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<HttpStatusCode> SendTemplateMail(Dictionary<string, string> request)
        {
            var from = new EmailAddress("kanav.dip.19@ctuniversity.in", "CT University");
            List<EmailAddress> tos = new List<EmailAddress>();
            string[] toList = request["to"].Replace(" ", string.Empty).Split(',');

            foreach (string recipient in toList)
            {
                tos.Add(new EmailAddress(recipient));
            }

            var templatedata = new DynamicObject
            {
                subject = request["subject"],
                body = request["body"]
            };

            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, tos, request["id"], templatedata);
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }
    }
}