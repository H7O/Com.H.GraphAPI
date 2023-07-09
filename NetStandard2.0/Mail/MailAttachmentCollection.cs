using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Com.H.GraphAPI.Mail
{
    public class MailAttachment 
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string OData { get; set; }
    }
    public class MailAttachmentCollection
    {
        private List<MailAttachment> _list = new List<MailAttachment>();
        public List<MailAttachment> List
        {
            get
            {
                return this._list;
            }
        }

        public void Add(string filePath, string fileName = null)
        {
            if (!string.IsNullOrWhiteSpace(fileName) && fileName.ContainsInvalidFileNameChars())
                throw new ArgumentException($"Invalid file name: {fileName}", nameof(fileName));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = null;

            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
            this.List.Add(new MailAttachment()
            {
                OData = Convert.ToBase64String(File.ReadAllBytes(filePath)),
                FileName = fileName ?? Path.GetFileName(filePath),
                FilePath = filePath
            });
        }

        public void Add(Stream stream, string fileName)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            if (fileName.ContainsInvalidFileNameChars())
                throw new ArgumentException($"Invalid file name: {fileName}", nameof(fileName));

            this.List.Add(new MailAttachment()
            {
                FileName = fileName,
                OData = Convert.ToBase64String(stream.ReadFully())
            });
        }

        public void AddContent(string content, string fileName)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            if (fileName.ContainsInvalidFileNameChars())
                throw new ArgumentException($"Invalid file name: {fileName}", nameof(fileName));

            this.List.Add(new MailAttachment()
            {
                FileName = fileName,
                OData = Convert.ToBase64String(Encoding.UTF8.GetBytes(content))
            });
        }

        public void Remove(string fileNameOrFilePath)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrFilePath))
                return;
            var toBeRemoved = this.List.FirstOrDefault(
                x => x.FileName?.EqualsIgnoreCase(fileNameOrFilePath) == true
                || x.FilePath?.EqualsIgnoreCase(fileNameOrFilePath) == true);
            if (toBeRemoved == null) return;
            this.List.Remove(toBeRemoved);
        }


    }


}
