using System;
using System.Collections.Generic;
using System.Linq;
using BuildStuffFeedback.Models;
using BuildStuffFeedback.Models.Admin;
using BuildStuffFeedback.Providers;
using BuildStuffFeedback.ViewModels;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Security;

namespace BuildStuffFeedback.Modules
{
    public class AdminModule : NancyModule
    {
        public class AdminRegistrations : IRegistrations
        {
            private readonly SessionProvider _provider;

            public AdminRegistrations()
            {
                _provider = new SessionProvider();
            }

            public IEnumerable<TypeRegistration> TypeRegistrations
            {
                get { yield break; }
            }

            public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
            {
                get { yield break; }
            }

            public IEnumerable<InstanceRegistration> InstanceRegistrations
            {
                get
                {
                    yield return new InstanceRegistration(typeof (GetSessions),
                        new GetSessions(() => _provider.GetAllSessions().Select(session => new SessionSummary
                        {
                            Id = session.Id,
                            Speaker = session.Speaker,
                            Title = session.Title
                        })));
                    yield return new InstanceRegistration(typeof (GetSession),
                        new GetSession((string id) =>
                        {
                            var session = _provider.GetSession(id);
                            var feedback = _provider.GetSessionFeedbackSummary(id).ToList();
                            var ratings = feedback.ToLookup(x => x.Rating, x => new {x.Rating});
                            return new SessionDetail
                            {
                                Id = session.Id,
                                Speaker = session.Speaker,
                                Title = session.Title,
                                Email = session.Email,
                                Greens = ratings[(int) Level.Green].Count(),
                                Yellows = ratings[(int) Level.Yellow].Count(),
                                Reds = ratings[(int) Level.Red].Count(),
                                Comments = (from f in feedback
                                            where false == String.IsNullOrWhiteSpace(f.Comments) 
                                            select f.Comments).ToList()
                            };
                        }));
                }
            }
        }

        public AdminModule(GetSessions getAllSessions, GetSession getSessionDetail, SessionProvider provider)
            : base("/admin")
        {
            this.RequiresAuthentication();

            Get["/sessions"] = _ => Negotiate.WithModel(new AdminSessionsViewModel(getAllSessions()));
            Get["/session/{id}"] = p => Negotiate.WithModel(getSessionDetail((string)p.Id));
            Post["/session/{id}/bulk-feedback", runAsync: true] = async (p, token) =>
            {
                int sessionId = p.id;
                Level rating = Enum.Parse(typeof(Level), Request.Form.rating);
                
                await provider.AddBulkFeedback(sessionId, rating, Request.Form.count);

                return Response.AsRedirect("~/admin/session/" + sessionId);
            };

            Post["/session/{id}/feedback", runAsync: true] = async (p, token) =>
            {
                int sessionId = p.id;
                Level rating = Enum.Parse(typeof(Level), Request.Form.rating);
                string comments = Request.Form.comments;

                if (String.IsNullOrWhiteSpace(comments))
                    throw new InvalidOperationException("Need comments.");

                provider.AddFeedback(new Feedback
                {
                    Comments = comments,
                    Rating = (int) rating,
                    SessionId = sessionId
                });

                return Response.AsRedirect("~/admin/session/" + sessionId);
            };
        }
    }
}