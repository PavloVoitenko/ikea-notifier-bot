﻿namespace IkeaNotifier.Options;

public class MongoOptions
{
	public string ConnectionString { get; set; }
	public string DatabaseName { get; set; }
	public string UserCollectionName { get; set; }
	public string LastUpdatedCollectionName { get; set; }
}