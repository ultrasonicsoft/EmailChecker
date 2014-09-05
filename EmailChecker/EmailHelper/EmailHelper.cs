using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailChecker.Model;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace EmailChecker.EmailHelper
{
    internal class EmailHelper
    {
        private Pop3Client pop3Client;

        internal void InitializePop3Client(string pop3Server, string pop3ServerPortNumber, bool useSSLForPop3Server,
            string UserName, string Password)
        {
            pop3Client = new Pop3Client();
            pop3Client.Connect(pop3Server, int.Parse(pop3ServerPortNumber), useSSLForPop3Server);
            pop3Client.Authenticate(UserName, Password);

        }

        public void FetchUnseenMessages(List<string> seenUids, ObservableCollection<MessageEntity> allEmails)
        {
            // Fetch all the current uids seen
            var uids = pop3Client.GetMessageUids();

            // Create a list we can return with all new messages
            List<Message> newMessages = new List<Message>();

            // All the new messages not seen by the POP3 client
            for (int messageIndex = 0; messageIndex < uids.Count; messageIndex++)
            {
                string currentUidOnServer = uids[messageIndex];
                if (!seenUids.Contains(currentUidOnServer))
                {
                    // We have not seen this message before.
                    // Download it and add this new uid to seen uids

                    // the uids list is in messageNumber order - meaning that the first
                    // uid in the list has messageNumber of 1, and the second has 
                    // messageNumber 2. Therefore we can fetch the message using
                    // i + 1 since messageNumber should be in range [1, messageCount]
                    Message unseenMessage = pop3Client.GetMessage(messageIndex + 1);

                    var mailMessage = unseenMessage.ToMailMessage();
                    allEmails.Add(new MessageEntity
                    {
                        Number = messageIndex,
                        Subject = mailMessage.Subject,
                        Body = mailMessage.Body,
                        DateSent = unseenMessage.Headers.DateSent
                    });

                    // Add the message to the new messages
                    newMessages.Add(unseenMessage);

                    // Add the uid to the seen uids, as it has now been seen
                    seenUids.Add(currentUidOnServer);
                }
            }
            // Return our new found messages
        }
    }
}
