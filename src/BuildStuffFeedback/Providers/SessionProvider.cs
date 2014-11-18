using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BuildStuffFeedback.Models;
using BuildStuffFeedback.Models.Admin;
using Dapper;

namespace BuildStuffFeedback.Providers
{
    public class SessionProvider : ISessionProvider
    {
        public IEnumerable<Session> GetAllSessions()
        {
            using (IDbConnection connection = OpenConnection())
            {
                String sql = "select * from Sessions";

                return connection.Query<Session>(sql);
            }
        }

        public Session GetSession(string id)
        {
            using (IDbConnection connection = OpenConnection())
            {
                return connection.Query<Session>("SELECT * FROM Sessions WHERE Id = @Id",
                    new {Id = id}).SingleOrDefault();
            }
        }

        public IEnumerable<Feedback> GetSessionFeedbackSummary(string id)
        {
            using (IDbConnection connection = OpenConnection())
            {
                return connection.Query<Feedback>("SELECT Rating, Comments FROM Feedbacks WHERE SessionId = @SessionId",
                    new { SessionId = id });
            }
            
        }

        public AddFeedbackResult AddFeedback(Feedback feedback)
        {
            var sqlQuery = "INSERT INTO Feedbacks (SessionId, Rating, Comments, FullName,Email) " +
                           "VALUES(@SessionId, @Rating, @Comments, @FullName, @Email);";

            var sessionExistsQuery =
                "SELECT Count(*) FROM Feedbacks WHERE SessionId = @SessionId AND Email = @Email";

            using (IDbConnection connection = OpenConnection())
            {
                if (connection.Query<int>(sessionExistsQuery, feedback).First() > 0)
                    return new FeedbackAlreadyExisted();

               connection.Execute(sqlQuery, feedback);
                return new FeedbackAdded();
            }
        }

        public async Task AddBulkFeedback(int sessionId, Level level, int count)
        {
            var rating = (int)level;
            const string query = "INSERT INTO Feedbacks (SessionId, Rating, Comments) VALUES(@sessionId, @rating, @comments);";

            using (var connection = OpenConnection())
            {
                for (var i = 0; i < count; i++)
                {
                    await connection.ExecuteAsync(query, new
                    {
                        sessionId,
                        rating,
                        comments = ""
                    });
                }
            }
        }

        //public void AddSession(Session session)
        //{
        //    _database.Insert(session);
        //}

        private IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BuildStuffConnectionString"].ConnectionString);
            connection.Open();
            return connection;
        }
    }

    public interface ISessionProvider
    {
        IEnumerable<Session> GetAllSessions();
        Session GetSession(string Id);
        AddFeedbackResult AddFeedback(Feedback feedback);
    }
}