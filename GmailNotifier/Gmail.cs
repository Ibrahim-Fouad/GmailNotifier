using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GmailNotifier
{
    class Gmail
    {
        public static Gmail Instance
        {
            get
            {
                if ( _this == null )
                {
                    _this = new Gmail();
                }
                return _this;
            }
        }

        private static Gmail _this;

        static string[] Scopes = { GmailService.Scope.GmailReadonly };

        static string ApplicationName = "Gmail Notifier";

        public GmailService Service { get; private set; }

        public List<Message> Messages { get; set; } = new List<Message>();
        private Gmail()
        {
            UserCredential credential;
            using ( var stream =
                new FileStream( "client_secret.json", FileMode.Open, FileAccess.Read ) )
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal );
                credPath = Path.Combine( credPath, ".credentials/gmail-dotnet-quickstart.json" );

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load( stream ).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore( credPath, true ) ).Result;
            }

            // Create Gmail API service.
            var service = new GmailService( new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            } );
            this.Service = service;
        }


    }
}
