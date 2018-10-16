using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;

namespace Website.Security
{
    public class NaiveSessionCache : TokenCache
    {
        private static ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly string userObjectId;
        private readonly string cacheId;
        private readonly HttpContext currentContext;

        public NaiveSessionCache(HttpContext context, string userId)
        {
            userObjectId = userId;
            cacheId = $"{userObjectId}_TokenCache";
            currentContext = context;

            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
            Load();
        }

        public void Load()
        {
            sessionLock.EnterReadLock();
            Deserialize(currentContext.Session.Get(cacheId));
            sessionLock.ExitReadLock();
        }

        public void Persist()
        {
            sessionLock.EnterWriteLock();

            HasStateChanged = false;

            currentContext.Session.Set(cacheId, Serialize());
            sessionLock.ExitWriteLock();
        }

        public override void Clear()
        {
            base.Clear();
            currentContext.Session.Remove(cacheId);
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (HasStateChanged)
            {
                Persist();
            }
        }
    }
}