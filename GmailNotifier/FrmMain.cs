using Google.Apis.Gmail.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thread = System.Threading.Thread;

namespace GmailNotifier
{
    public partial class FrmMain : Form
    {
        private Gmail _gmail = Gmail.Instance;
        private List<string> idsList;

        private bool _isRunning = false;

        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            idsList = new List<string>();
            LoadIds();
        }

        private async void btnStart_Click( object sender, System.EventArgs e )
        {
            _isRunning = true;
            await Check( ( byte )numericSeconds.Value );
        }

        private async Task Check( byte seconds )
        {
            await Task.Run( () =>
             {
                 while ( _isRunning )
                 {
                     try
                     {
                         //Get list of thread for current user
                         var request = _gmail.Service.Users.Messages.List( "me" );

                         //Set label to 'UNREAD' to get new mails which is unread
                         request.LabelIds = "UNREAD";


                         // Execute request and get list of threads
                         var messages = request.Execute().Messages;

                         // Check if threads is not null and its count is greater than zero
                         if ( messages != null && messages.Count > 0 )
                         {
                             var count = 0;

                             _gmail.Messages.Clear();
                             foreach ( var message in messages )
                             {
                                 if ( !idsList.Exists( id => message.Id == id ) )
                                 {
                                     var m = _gmail.Service.Users.Messages.Get( "me", message.Id );
                                     m.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                                     var finalMessage = m.Execute();
                                     _gmail.Messages.Add( finalMessage );
                                     idsList.Add( message.Id );
                                     count++;
                                 }

                             }
                             if ( count > 0 )
                             {
                                 this.Invoke( new MethodInvoker( () =>
                                   {
                                       new FrmNotification( count ).Notify( 10 );
                                   } ) );
                             }

                         }
                     }
                     catch ( Exception )
                     {
                     }

                     Thread.Sleep( seconds );
                 }
             } );
        }


        private void LoadIds()
        {
            var filePath = Path.Combine( Application.StartupPath, "ids.txt" );
            if ( File.Exists( filePath ) )
            {
                idsList.AddRange( new StreamReader( filePath ).ReadToEnd()
                    .Split( new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries ) );
            }
            else
            {
                File.WriteAllText( filePath, null );
            }
        }
    }
}
