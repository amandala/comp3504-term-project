﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using SQLite;

namespace beer_me
{
	public class BreweryDataService
	{
		public BreweryDataService()
		{
		}


		public IEnumerable<TableBrewery> queryBreweries(string path, string id)
		{

			var db = new SQLiteConnection(path);
			return db.Query<TableBrewery>("select * from TableBrewery where breweryId = ?", id);
		}

		// REST 

		public async Task<JsonValue> FetchDataAsync(string url)
		{
			Console.WriteLine("Fetching Data");
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			using (WebResponse response = await request.GetResponseAsync())
			{
				using (Stream stream = response.GetResponseStream())
				{
					JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
					return jsonDoc;
				}
			}
		}


		// SQLite

		public string createDatabase(string path)
		{
			try
			{
				var connection = new SQLiteAsyncConnection(path);
				connection.CreateTableAsync<TableBrewery>().ContinueWith(t =>
				{
					Console.WriteLine("-----Tables created" + path);
				});

				return "Database created";

			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public string insertUpdateData(TableBrewery data, string path)
		{
			try
			{

				var db = new SQLiteAsyncConnection(path);
				var r = db.QueryAsync<TableBrewery>("select * from TableBrewery where breweryId = ?", data.ID);

				Console.Write(r);
				var res = db.InsertAsync(data);
				Console.WriteLine("Insert", res);
				return "Data added/updated";



			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}



	}
}
