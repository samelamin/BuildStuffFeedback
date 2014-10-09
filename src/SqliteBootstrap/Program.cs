﻿using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Data;
/*
	Program to smoke test the Sqlite assembly and create the badges database.
	Also seeds the database with a single record and gets the data via the PetaPoco ORM.
*/
using BuildStuffFeedback.Models;

namespace SqliteBootstrap
{
	class Program
	{
		static void Main(string[] args)
		{
            String fileName =  @"../../../BuildStuffFeedback\Db\buildstuff.db";
			// create the file in the documents folder
			// create the fil, table and sample if its not there
			SeedDatabase(fileName);
			// display the new record.
			QueryDatabaseOrm(fileName);
		}

		private static void QueryDatabaseOrm(string fileName)
		{
			// create a database "context" object t
			String connectionString =  String.Format("Data Source={0}", fileName);

			DbProviderFactory sqlFactory = new SQLiteFactory();
			PetaPoco.Database db = new PetaPoco.Database(connectionString, sqlFactory);
            
			// For Sql Server replace the lines above with the two lines below
			//String connectionString =  @"data source=[Server];initial catalog=[Database];user id=[USER];password=[******];";
			//PetaPoco.Database db = new PetaPoco.Database(connectionString, "System.Data.SqlClient");

			// load and array of POCO for Badges
			String sql = "select * from Sessions";
            foreach (Session session in db.Query<Session>(sql))
			{
                Console.WriteLine("{0} {1} {2} {3}", session.Id, session.Title, session.Speaker, session.Email);
			}


            sql = "select * from Feedbacks";
            foreach (Feedback feedback in db.Query<Feedback>(sql))
            {
                Console.WriteLine("{0} {1} {2} {3}", feedback.Id, feedback.SessionId, feedback.Rating, feedback.Comments);
            }
		}
		//
		// Create the SQLite database and file. 
		//
		private static void SeedDatabase(string fileName)
		{
			String dbConnection = String.Format("Data Source={0}", fileName);
			String sql = @"
DROP TABLE  if exists Sessions;
DROP TABLE if exists Feedbacks;

create table if not exists [Sessions] (
[Id] INTEGER PRIMARY KEY ASC,
[SessionId] varchar(20) ,
[Title] varchar(20) ,
[Speaker] varchar(20) ,
[Email] varchar(255));


create table if not exists [Feedbacks] (
[Id] INTEGER PRIMARY KEY ASC,
[SessionId] INTEGER ,
[Rating] INTEGER,
[Comments] varchar(255));

";

			ExecuteNonQuery(dbConnection, sql);
			Console.WriteLine("Created {0}", fileName);
			
			sql = @"
insert or replace into Sessions ([Id], [Title], [SessionId], [Speaker], [Email]) 
     values (1, 'Keynote: Gregory Young', 1, 'Greg Young', 'Gregyoung@gmail.com');


insert or replace into Feedbacks ([Id], [SessionId], [Rating], [Comments]) 
     values (1,1 , 4, 'amazing talk!');
" ;
			ExecuteNonQuery(dbConnection, sql);
			Console.WriteLine("Seeded database with a sample record");
		}

		// Helper extracted from SqliteHelper.cs
		public static int ExecuteNonQuery(string dbConnection, string sql)
		{
			SQLiteConnection cnn = new SQLiteConnection(dbConnection);
			try
			{
				cnn.Open();
				SQLiteCommand mycommand = new SQLiteCommand(cnn);
				mycommand.CommandText = sql;
				int rowsUpdated = mycommand.ExecuteNonQuery();
				return rowsUpdated;
			}
			catch (Exception fail)
			{
				Console.WriteLine(fail.Message);
				return 0;
			}
			finally
			{
				cnn.Close();
			}
		}
	}

	//
    // PetaPoco to SQL Class for table Badges
	// generated by DbViewSharp http://dbviewsharp.codeplex.com
	// 
	// Note the attributes are probably not all necessary as the table was designed
	// to be ORM-friendly. However they are included as a hint for how to use them
	// when creating an object mapping to a legacy table.
	// See Petapoco documentation for more help. http://www.toptensoftware.com/petapoco/
	// the class is partial so you can implement other behaviours separate from the Petapoco 
	// requirements.
	// Note: the Id type must be Int64 for self-generating keys in Sqlite.
	// 
}

