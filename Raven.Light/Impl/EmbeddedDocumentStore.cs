﻿using System;
using Raven.Light.Conventions;
using Raven.Light.Listeners;
using Raven.Light.Persistence;
using Raven.Munin;

namespace Raven.Light.Impl
{
	public class EmbeddedDocumentStore : IDisposable
	{
		private readonly TableStorage storage;
		private readonly IPersistentSource persistentSource;

		internal TableStorage Storage
		{
			get { return storage; }
		}

		public DocumentConvention Conventions { get; private set; }

		public EmbeddedDocumentStore() : this("default")
		{
		}

		public EmbeddedDocumentStore(string name)
		{
			try
			{
				Conventions = new DocumentConvention
				{
					DocumentKeyGenerator = entity =>
					{
						var typeTagName = Conventions.GetTypeTagName(entity.GetType());
						if (typeTagName == null)
							return Guid.NewGuid().ToString();
						return typeTagName + "/" + Guid.NewGuid();
					}
				};

				persistentSource = new IsolatedStoragePersistentSource(name, "ravenlight");
				storage = new TableStorage(persistentSource);
				storage.Initialze();
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public IEmbeddedSession OpenSession()
		{
			return new EmbeddedDocumentSession(this, new IDocumentStoreListener[0], new IDocumentDeleteListener[0]);
		}


		public void Dispose()
		{
			if(storage != null)
				storage.Dispose();
			if(persistentSource != null)
				persistentSource.Dispose();
		}
	}
}
