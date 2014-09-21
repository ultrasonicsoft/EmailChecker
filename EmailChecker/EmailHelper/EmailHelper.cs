using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using EmailChecker.Model;
using EmailChecker.ViewModel;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace EmailChecker.EmailHelper
{
    internal class EmailHelper
    {
        private Pop3Client pop3Client;
        private MediaPlayer mediaPlayer = new MediaPlayer();

        internal void InitializePop3Client(string pop3Server, string pop3ServerPortNumber, bool useSSLForPop3Server,
            string UserName, string Password)
        {
            try
            {
                string mesage =
                    string.Format("Connecting to server with details: Pop3Server:{0}, PortNumber:{1}, useSSL:{2}",
                        pop3Server, pop3ServerPortNumber, useSSLForPop3Server);
                LogHelper.WriteMessage(mesage);
                pop3Client = new Pop3Client();
                pop3Client.Connect(pop3Server, int.Parse(pop3ServerPortNumber), useSSLForPop3Server);
                pop3Client.Authenticate(UserName, Password);

                LogHelper.WriteMessage("Connection Succeeded.");
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("InitializePop3Client", exception);
                throw ;
            }
        }

        public void FetchUnseenMessages(List<string> seenUids, ObservableCollection<MessageEntity> allEmails)
        {
            try
            {
                string message = string.Format("Fetching new message...");
                // Fetch all the current uids seen
                var uids = pop3Client.GetMessageUids();

                // Create a list we can return with all new messages
                List<Message> newMessages = new List<Message>();

                // All the new messages not seen by the POP3 client
                for (int messageIndex = 0; messageIndex < uids.Count; messageIndex++)
                {
                    if (messageIndex > 10)
                        break;

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


                        string messageText = string.Format("Found unread messae with subject: {0}", mailMessage.Subject);
                        LogHelper.WriteMessage(messageText);

                        var newMail = new MessageEntity();
                        newMail.Number = messageIndex;

                        newMail.Body = mailMessage.Body;
                        //TODO: Test data
                        if (messageIndex % 2 == 0)
                        {
                            newMail.Subject = mailMessage.Subject;
                            newMail.OrderDetails = FillOrderDetails(mailMessage.Body);
                        }
                        else
                        {
                            newMail.Subject = mailMessage.Subject.Replace("70567", "80588");
                            newMail.Body = mailMessage.Body.Replace("70567", "80588");
                            newMail.OrderDetails = FillOrderDetails(mailMessage.Body.Replace("70567", "80588"));
                        }

                        newMail.DateSent = unseenMessage.Headers.DateSent;

                        CheckForSimilarity(allEmails, newMail);

                        allEmails.Add(newMail);

                        // Add the message to the new messages
                        newMessages.Add(unseenMessage);

                        // Add the uid to the seen uids, as it has now been seen
                        seenUids.Add(currentUidOnServer);

                        if (File.Exists(EmailServerSettingViewModel.CurrentServerSettings.AlarmFilePath))
                        {
                            mediaPlayer.Open(new Uri(EmailServerSettingViewModel.CurrentServerSettings.AlarmFilePath));
                            mediaPlayer.Play();
                        }
                    }
                }

                foreach (MessageEntity messageEntity in allEmails)
                {
                    messageEntity.RecordCompareTimer.Start();
                }
                // Return our new found messages
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("FetchUnseenMessages",exception);
                throw exception;
            }
        }

        private void CheckForSimilarity(IEnumerable<MessageEntity> allEmails, MessageEntity newMail)
        {
            try
            {
                LogHelper.WriteMessage("Checking messages for similarity...");
                var data = allEmails.Where(x => x.OrderDetails.AccountNumber == newMail.OrderDetails.AccountNumber);
                foreach (MessageEntity messageEntity in data)
                {
                    if (messageEntity.Subject.Contains("Test message"))
                        continue;
                    LogHelper.WriteMessage(string.Format("Similar account found. Marking accounts as Compared: {0}.", messageEntity.OrderDetails.AccountNumber.ToString()));
                    messageEntity.Status = MessageStatus.Comapred;
                }
                data = allEmails.Where(x => x.OrderDetails.AccountNumber != newMail.OrderDetails.AccountNumber);
                foreach (MessageEntity messageEntity in data)
                {
                    if (messageEntity.Subject.Contains("Test message"))
                        continue;
                    LogHelper.WriteMessage(string.Format("Marking accounts as Different: {0}.", messageEntity.OrderDetails.AccountNumber.ToString()));
                    messageEntity.Status = MessageStatus.Different;
                }
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("CheckForSimilarity", exception);
                throw exception;
            }
        }

        private OrderEntity FillOrderDetails(string messageBody)
        {
            LogHelper.WriteMessage("Extracting Order details...");
            OrderEntity entity = new OrderEntity();
            try
            {
                if (messageBody.Contains("test message"))
                    return entity;

                string[] bodyPartStrings = messageBody.Split(' ');
                entity.AccountNumber = bodyPartStrings[0];
                entity.Separator = bodyPartStrings[1];
                entity.Number = bodyPartStrings[2];
                entity.State = bodyPartStrings[3];
                entity.OpenDate = DateTime.Parse(bodyPartStrings[4]);
                entity.OpenTime = TimeSpan.Parse(bodyPartStrings[5]);
                entity.Direction = bodyPartStrings[6];

                string messageText =
                    string.Format(
                        "Account Number:{0}, Separator:{1}, Number:{2}, State:{3}, OpenDate:{4}, OpenTime:{5}, Direction:{6}",
                        entity.AccountNumber.ToString(), entity.Separator, entity.Number, entity.State, entity.OpenDate,
                        entity.OpenTime, entity.Direction);
                LogHelper.WriteMessage(messageText);

                string[] subPartStrings = bodyPartStrings[7].Split(',');
                entity.Size = subPartStrings[0];
                entity.Symbol = subPartStrings[1];
                entity.OpenPrice = subPartStrings[2];
                entity.StopLoss = subPartStrings[3];
                entity.Profit = subPartStrings[4];

                messageText =
                    string.Format(
                        "Size:{0}, Symbol:{1}, OpenPrice:{2}, StopLoss:{3}, Profit:{4}",
                        entity.Size.ToString(),entity.Symbol, entity.OpenPrice, entity.StopLoss, entity.Profit);
                LogHelper.WriteMessage(messageText);

                if (entity.State == "C")
                {
                    entity.CloseDate = DateTime.Parse(subPartStrings[5]);
                    string[] closeDetails = bodyPartStrings[8].Split(',');
                    entity.ClosedProfit = closeDetails[0];
                    entity.Swap = closeDetails[2];
                    entity.Commission = closeDetails[3];
                    entity.FinalProfit = closeDetails[4];

                    messageText =
                    string.Format(
                        "CloseDate:{0}, ClosedProfit:{1}, Swap:{2}, Commission:{3}, FinalProfit:{4}",
                        entity.CloseDate.ToString(), entity.ClosedProfit, entity.Swap, entity.Commission, entity.FinalProfit);
                    LogHelper.WriteMessage(messageText);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return entity;
        }
    }
}
