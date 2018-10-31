﻿using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Repositories
{
    internal abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceContext userDefinedSpaceContext;

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName, SpaceContext userDefinedSpaceContext) 
            : base(repository, collectionLinkName)
        {
            ValidateThatICanUseACustomSpaceContext();
            this.userDefinedSpaceContext = userDefinedSpaceContext;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                var spaceContext = GetCurrentSpaceContext();

                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    [MixedScopeConstants.QueryStringParameterIncludeSystem] = spaceContext.IncludeSystem,
                    [MixedScopeConstants.QueryStringParameterSpaces] = spaceContext.ApplySpaceSelection<object>(spaces => spaces, 
                        () => MixedScopeConstants.AllSpacesQueryStringParameterValue)
                };

            }
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return userDefinedSpaceContext ?? Repository.Scope.Apply(SpaceContext.SpecificSpace, 
                       SpaceContext.SystemOnly, 
                       SpaceContext.AllSpacesAndSystem);
        }

        void ValidateThatICanUseACustomSpaceContext()
        {
            var scopeIsSpecified = Repository.Scope.Apply(_ => true, () => true, () => false);
            if (scopeIsSpecified)
            {
                throw new SpaceContextSwitchException();
            }
        }

        protected override void EnrichSpaceId(TMixedScopeResource resource)
        {
            base.EnrichSpaceId(resource);

            if (resource is IHaveSpaceResource spaceResource 
                && userDefinedSpaceContext != null)
            {
                spaceResource.SpaceId = userDefinedSpaceContext.ApplySpaceSelection(spaceIds =>
                {
                    if (spaceIds.Count == 1 && !userDefinedSpaceContext.IncludeSystem)
                    {
                        return spaceIds.Single();
                    }

                    if (spaceIds.Count == 0 && userDefinedSpaceContext.IncludeSystem)
                    {
                        // This assumes that the resource we are sending can actually apply at the system level.
                        // This is not always true, for example Tasks that can only apply at the space level.
                        // In those specific cases, we should perform separate pre-condition checks to ensure that you are not in a system context
                        // Usually this involves calling `EnsureSingleSpaceContext`
                        return null;
                    }

                    return spaceResource.SpaceId;
                }, () => spaceResource.SpaceId);
            }
        }

        protected void EnsureSingleSpaceContext()
        {
            Repository.Scope.Apply(_ => {},
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => 
                {
                    if (userDefinedSpaceContext == null)
                    {
                        return; // Assumes the default space
                    }

                    userDefinedSpaceContext.ApplySpaceSelection(spaces =>
                    {
                        var numberOfSpaces = spaces.Count;
                        if (numberOfSpaces == 0)
                        {
                            // We must be in a system context
                            throw new SpaceScopedOperationInSystemContextException();
                        }

                        if (numberOfSpaces > 1)
                        {
                            throw new SingleSpaceOperationInMultiSpaceContextException();
                        }
                    }, () => throw new SingleSpaceOperationInMultiSpaceContextException());
                });
        }
    }
}