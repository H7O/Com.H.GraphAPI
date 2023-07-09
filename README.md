# Com.H.GraphAPI

Simplified way to use Microsoft Graph API in .NET applications. In its current initial release, it supports authenticating and sending emails only.
This library supports all .NET releases, .NET 4.x frameworks (.NET 4.6 to 4.8.x) and also .NET Core / 5 / 6 / 7 / 8+ 

## Installation

### Nuget

```bash
Install-Package Com.H.GraphAPI
```

### .NET CLI

```bash
dotnet add package Com.H.GraphAPI
```

## Usage

### Sample 1

Authenticating and receiving an access token then sending an email using the account to which the access token is issued.

```csharp
string clientId = "Your Client ID";
string clientSecret = "Your Client Secret";
string tenantId = "Your Tenant ID";

var accessToken =
    Com.H.GraphAPI.Identity.GIExtensions
    .GetAccessToken(clientId, clientSecret, tenantId); // use the "GetAccessTokenAsync" method to request the token asynchronously.
                

Com.H.GraphAPI.Mail.Message msg = new Com.H.GraphAPI.Mail.Message();

msg.From = "yourName@yourCompany.com";
msg.To.Add("yourName@yourCompany.com");
msg.Subject = "Testing MS Graph API";
msg.Body = "This is a test email sent via <strong>MS Graph API</strong>";

// read from a file path
msg.Attachments.Add(@"c:\temp\email\test1.txt");
msg.Attachments.Add(@"c:\temp\email\test2.txt");

// add content directly
msg.Attachments.AddContent("direct content test", "test3.txt");

var result = msg.Send(accessToken); // use the "SendAsync" method to send the email asynchronously.

if (result.IsSuccessStatusCode)
{
    Console.WriteLine("Email sent successfully");
}
else
{
    Console.WriteLine("Email failed to send");
}
```

### Sample 2

Authenticating and receiving an access token then sending an email using the account to which the access token is issued.
Also, the sample shows how to re-use the same access token for multiple requests while it is still valid (i.e, hasn't yet expired).
And also shows how to attach a file from a stream.

```csharp
            string clientId = "Your Client ID";
            string clientSecret = "Your Client Secret";
            string tenantId = "Your Tenant ID";


            Com.H.GraphAPI.Mail.Message msg = new Com.H.GraphAPI.Mail.Message();

            // the following delegate gets called, under the hood by the Send() method, before sending the email to obtain an access token.
            msg.GetAccessTokenDelegate = () =>
            {
                // you can return your previously issued access token here if it's still valid
                // otherwise, you can issue a new access token here.

                // you can use the following code sample to issue a new access token
                var accessTokenWithExpiryInfo =
                    Com.H.GraphAPI.Identity.GIExtensions
                    .GetAccessTokenWithExpiryDate(clientId, clientSecret, tenantId); // async version is also available

                if (string.IsNullOrWhiteSpace(accessTokenWithExpiryInfo.AccessToken))
                {
                    Console.WriteLine("Error obtaining access token");
                    // your logic to log the error goes here

                    // the following exception will be thrown when calling msg.Send()
                    throw new Exception("Error obtaining access token");
                }

                Console.WriteLine($"access token expires on {accessTokenWithExpiryInfo.ExpiresOn}");
                return accessTokenWithExpiryInfo.AccessToken;

            }; ;


            msg.From = "yourName@yourCompany.com";
            msg.To.Add("yourName@yourCompany.com");
            msg.Subject = "Testing MS Graph API";
            msg.Body = "This is a test email sent via <strong>MS Graph API</strong>";

            // read from a file path
            msg.Attachments.Add(@"c:\temp\email\test1.txt");
            msg.Attachments.Add(@"c:\temp\email\test2.txt");

            // add content directly
            msg.Attachments.AddContent("direct content test", "test3.txt");

            // read from an IO Stream
            using (var fs = new FileStream(@"c:\temp\email\test4.txt", FileMode.Open, FileAccess.Read))
            {
                msg.Attachments.Add(fs, "test4.txt");
                // the stream can be safely closed here as it will be copied to a memory stream under the hood.
            }

            var result = msg.Send();
            
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent successfully");
            }
            else
            {
                Console.WriteLine("Email failed to send");
            }


```
