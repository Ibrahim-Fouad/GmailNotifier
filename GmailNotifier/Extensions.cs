using Google.Apis.Gmail.v1.Data;
using System;
using System.Linq;

namespace GmailNotifier
{
    public static class Extensions
    {
        public static DateTime? GetDate( this Message message )
        {
            DateTime date = DateTime.Now;
            try
            {
                var header = message.Payload.Headers.SingleOrDefault( h => h.Name.ToLower() == "date" );
                if ( header != null )
                {
                    date = DateTime.Parse( header.Value );
                    return date;
                }
                return null;
            }
            catch ( Exception )
            {
                return null;
            }
        }
    }
}
