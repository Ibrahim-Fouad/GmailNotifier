using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Message = Google.Apis.Gmail.v1.Data.Message;

namespace GmailNotifier
{
    public partial class FrmNotification : Form
    {
        private List<Message> messages;
        public FrmNotification( int count )
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            lblCount.Text = $@"You have new {count} mail(s).";
            CenterToFormH( lblCount );
            messages = Gmail.Instance.Messages;
            foreach ( var message in messages )
            {
                listSubjects.Items.Add( message.Payload.Headers.First( h => h.Name == "From" ).Value );
            }
        }

        private void CenterToFormH( Control control )
        {
            var contorlWidth = control.Size.Width;
            var formWidth = this.Size.Width;
            var newWidth = ( formWidth - contorlWidth ) / 2;
            control.Location = new Point( newWidth, control.Location.Y );
        }

        public void Notify( byte timeoutInSeconds )
        {
            this.Show();
            this.Location = new Point( Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height + 20 );
            for ( int i = 0; i < this.Height + 20; i++ )
            {
                this.Location = new Point( this.Location.X, this.Location.Y - 1 );
                Application.DoEvents();
            }
            new Thread( () =>
             {
                 Thread.Sleep( timeoutInSeconds * 1000 );
                 this.Dismiss();
             } ).Start();
        }
        public void Dismiss()
        {
            for ( int i = 0; i < this.Height + 20; i++ )
            {
                this.Location = new Point( this.Location.X, this.Location.Y + 1 );
                Application.DoEvents();
            }
            this.Close();
        }

    }
}
