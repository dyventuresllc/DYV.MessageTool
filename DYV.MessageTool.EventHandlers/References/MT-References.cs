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

        public Guid MessageTo_AllUsers => new Guid(_messageTo_allUsers);
        public Guid MessageTo_AllUsersEnabled => new Guid(_messageTo_allUsersEnabled);
        public Guid MessageTo_AllUsersActive => new Guid(_messageTo_allUsersActive);

        public Guid MessageBody = new Guid(_messageBody);

        public Guid MessageSubject = new Guid(_messageSubject);
    }
}
