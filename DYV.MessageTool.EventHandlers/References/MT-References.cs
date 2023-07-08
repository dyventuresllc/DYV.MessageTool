using System;

namespace DYV.MessageTool.EventHandlers.References
{
    public class MT_References
    { 
        private const string _name = "C7234921-5877-4F27-9DEB-169957DCBFEC";
        private const string _messageTo_allUsers = "7F226C58-1F11-4D70-B966-6706CA9B2CF0";
        private const string _messageTo_allUsersEnabled = "4E5C9F71-7945-478D-BDF5-38F493D60987";
        private const string _messageTo_allUsersActive = "1EC9002E-096F-4C54-85AA-50BDD6B4750F";
        private const string _messageBody = "53F971ED-700D-438B-A16C-6E9C6589350B";
        private const string _messageSubject = "5539BFC2-C09D-4386-8732-103583D46246";
        private const string _lastSentBy = "740A9C65-865A-46BB-B327-F4D535E90A03";
        private const string _lastSentDate = "FBB75560-BD9E-49E4-8C6E-358243184759";
        private const string _status = "A11E547B-195A-4F30-9B3A-696A04AB3FBA";
        private const string _submittedToQueue = "B4908124-12B0-44D7-B377-2623795DF203";
        private const string _sendingInProgess = "6F9747D1-4D24-400D-96BA-2BB3B3DD8363";
        private const string _sendingComplete = "C0B134FB-944B-4CF0-8CBF-1C2B1995E222";
        private const string _statusDetail = "4B7B0ED5-485B-4E3E-A06B-7CF598F31F5F";
        public Guid MessageTo_AllUsers => new Guid(_messageTo_allUsers);
        public Guid MessageTo_AllUsersEnabled => new Guid(_messageTo_allUsersEnabled);
        public Guid MessageTo_AllUsersActive => new Guid(_messageTo_allUsersActive);

        public Guid MessageBody = new Guid(_messageBody);

        public Guid MessageSubject = new Guid(_messageSubject);

        public Guid LastSentBy = new Guid(_lastSentBy);

        public Guid LastSentDate = new Guid(_lastSentDate);

        public Guid Status = new Guid(_status);
        
        public Guid StatusDetail = new Guid(_statusDetail);

        public Guid SubmittedToQueue = new Guid(_submittedToQueue);

        public Guid SendingInProgess = new Guid(_sendingInProgess);

        public Guid SendingComplete = new Guid(_sendingComplete);
    }
}
