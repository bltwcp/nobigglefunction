namespace yournamespace
{
    internal class SkypeNotification
    {
        public void Notify(string message)
        {
            var client = new RestClient(ConfigHelper.SkypeNotificationAPIUrl);

            foreach (var agent in ConfigHelper.SkypeNotificationList)
            {
                var request = new RestRequest("/skype", Method.POST);
                // Json to post.
                NotifyModel msg = new NotifyModel();
                msg.conversationID = agent;
                msg.text = message;


                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(msg), ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;
                
                try
                {
                    var response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.Created)
                    {
                        // OK
                        this.OnSuccess("done");
                    }
                    else
                    {
                        // NOK
                        this.OnError($"Skype failure Http code:{response.StatusCode} content:{response.Content} error:{response.ErrorMessage} ");
                    }
                }
                catch (Exception error)
                {
                    // Log
                    this.OnError(error.Message + error.StackTrace);
                }
            }


        }
        public void OnError(string message)
        {
            if (this.Error != null)
            {
                this.Error(this, new NotifyArg { Message = message });
            }
        }
        public void OnSuccess(string message)
        {
            if (this.Success != null)
            {
                this.Success(this, new NotifyArg { Message = message });
            }
        }
        public event EventHandler<NotifyArg> Error;
        public event EventHandler<NotifyArg> Success;
        private string GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
        public string DefaultMessage => $"[{ConfigHelper.AppName} IP:({GetIP()})] {{0}} ";
    }
    public class NotifyModel
    {
        public string conversationID { get; set; }
        public string text { get; set; }
    }
}
