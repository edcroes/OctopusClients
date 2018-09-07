using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IScopedUserRoleRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>,
        ICanIncludeSpaces<IScopedUserRoleRepository>
    {
    }

    class ScopedUserRoleRepository : MixedScopeBaseRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusAsyncClient client)
            : base(client, "ScopedUserRoles")
        {
        }

        ScopedUserRoleRepository(IOctopusAsyncClient client, SpaceContext spaceContext)
            : base(client, "ScopedUserRoles")
        {
            SpaceContext = spaceContext;
        }

        public IScopedUserRoleRepository Including(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Client, Client.SpaceContext.Union(spaceContext));
        }
    }
}