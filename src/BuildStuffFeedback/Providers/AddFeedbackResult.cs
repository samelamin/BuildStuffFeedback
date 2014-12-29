using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildStuffFeedback.Providers
{
    public class AddFeedbackResult
    {}

    public class FeedbackAdded : AddFeedbackResult
    {}

    public class FeedbackAlreadyExisted : AddFeedbackResult
    {}
}