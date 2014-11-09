using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildStuffFeedback.Models;
using BuildStuffFeedback.Providers;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;

namespace BuildStuffFeedback.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            var _sessionProvider = new SessionProvider();
            Get["/"] = parameters =>
            {
                return View["Index.cshtml"];
            };

            Get["/sessions"] = parameters =>
            {
                var sessions = _sessionProvider.GetAllSessions();
                return View["Sessions.cshtml", sessions];
            };

            Get["/ThankYou"] = parameters =>
            {
                return View["ThankYou.cshtml"];
            };

            Get["/sessions/{id}"] = parameter =>
            {
                var session = _sessionProvider.GetSession(parameter.id);
                if (session == null)
                {
                    return Response.AsRedirect("/");
                }
                return View["Session.cshtml",session];
            };

            Post["/sessions/AddFeedback"] = parameter =>
            {
                var feedback = this.Bind<Feedback>();
                _sessionProvider.AddFeedback(feedback);
                return Response.AsRedirect("/thankyou");
            };

        }
    }
}
