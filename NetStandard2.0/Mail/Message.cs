using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.H.GraphAPI.Mail
{
    public class Message
    {

        #region properties
        private bool disposedValue;
        public string Subject { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
        public bool SaveToSentItems { get; set; }
        public Func<string> GetAccessTokenDelegate { get; set; }
        public List<string> To { get; private set; } = new List<string>();
        public List<string> Cc { get; private set; } = new List<string>();
        public List<string> Bcc { get; private set; } = new List<string>();
        public string GraphApiBaseAddress { get; set; } = "https://graph.microsoft.com/v1.0";


        /// <summary>
        /// Default: false
        /// </summary>
        public bool DisableHtmlBody { get; set; }

        public MailAttachmentCollection Attachments { get; set; } = new MailAttachmentCollection();

        #endregion


        #region send
        public async Task<HttpResponseMessage> SendAsync(string accessToken = null, CancellationToken? token = null)
        {
            if (this.disposedValue) throw new ObjectDisposedException("Message");
            if (this.From == null) throw new MissingFieldException(nameof(this.From));
            if (!this.From.IsEmail()) throw new FormatException($"{this.From} is not a well formed email");

            this.To = this.To.Where(x => x.IsEmail()).ToList();
            this.Cc = this.Cc.Where(x => x.IsEmail()).ToList();
            this.Bcc = this.Bcc.Where(x => x.IsEmail()).ToList();

            //if (this.To == null) throw new MissingFieldException(nameof(this.To));
            if ((this.To == null || this.To.Count < 1)
                && (this.Cc == null || this.Cc.Count < 1)
                && (this.Bcc == null || this.Bcc.Count < 1)
                ) throw new MissingFieldException($"At least one valid email should be set for either To, Cc, or Bcc");

            if (!string.IsNullOrEmpty(this.Subject))
                this.Subject = this.Subject
                    .Replace("\r\n", " ")
                    .Replace("\n\r", " ")
                    .Replace("\r", " ")
                    .Replace("\n", " ");


            string toRecipientsJsonString =
                string.Join($",{Environment.NewLine}",
                    this.To.Select(
                        item =>
                        {
                            return string.Format("{{\"emailAddress\":{{\"address\":\"{0}\"}}}}", item);
                        }
                ));
            string ccRecipientsJsonString =
                string.Join($",{Environment.NewLine}",
                    this.Cc.Select(
                        item =>
                        {
                            return string.Format("{{\"emailAddress\":{{\"address\":\"{0}\"}}}}", item);
                        }
                ));
            string bccRecipientsJsonString =
                string.Join($",{Environment.NewLine}",
                    this.Bcc.Select(
                        item =>
                        {
                            return string.Format("{{\"emailAddress\":{{\"address\":\"{0}\"}}}}", item);
                        }
                ));


            string attachementsJsonString =
                string.Join( $",{Environment.NewLine}",
                    this.Attachments.List?
                        .Select(
                            item =>
                            {
                                return
                                    string.Format("{{\"@odata.type\":\"#microsoft.graph.fileAttachment\",\"name\":\"{0}\",\"contentBytes\":\"{1}\"}}",
                                    item.FileName, item.OData);
                            }
                        ));

            string emailJson = $@"
                {{
                    ""message"": {{
                        ""subject"": ""{this.Subject}"",
                        ""body"": {{
                            ""contentType"": ""{(this.DisableHtmlBody ? "text" : "html")}"",
                            ""content"": ""{this.Body}""
                        }},
                        ""toRecipients"": [
                            {toRecipientsJsonString}
                        ],
                        ""ccRecipients"": [
                            {ccRecipientsJsonString}
                        ],
                        ""bccRecipients"": [
                            {bccRecipientsJsonString}
                        ],
                        ""attachments"": [
                            {attachementsJsonString}
                        ],
                        }},
                    ""saveToSentItems"": ""{this.SaveToSentItems.ToString().ToLower()}""
                    
                }}
                
            ";

            if (string.IsNullOrWhiteSpace(accessToken))
                accessToken = this.GetAccessTokenDelegate?.Invoke();

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentNullException(nameof(accessToken), "accessToken is null or empty. Please provide a valid access token.");
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await httpClient.PostAsync($"{this.GraphApiBaseAddress}/users/{this.From}/sendMail", new StringContent(emailJson, System.Text.Encoding.UTF8, "application/json"));

        }

        public HttpResponseMessage Send(string accessToken = null, CancellationToken? token = null)
        {
            return this.SendAsync(accessToken, token).GetAwaiter().GetResult();
        }


        #endregion

    }
}
