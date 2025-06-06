using Elsa.Api.Client.Extensions;
using Elsa.Api.Client.Resources.ActivityDescriptors.Contracts;
using Elsa.Api.Client.Resources.ActivityDescriptors.Models;
using Elsa.Api.Client.Resources.ActivityDescriptors.Requests;
using Elsa.Api.Client.Resources.ActivityDescriptors.Responses;
using Elsa.Studio.Contracts;
using Elsa.Studio.Workflows.Domain.Contracts;

namespace Elsa.Studio.Workflows.Domain.Services;

/// <summary>
/// An activity registry provider that uses a remote backend to retrieve activity descriptors.
/// </summary>
public class RemoteActivityRegistryProvider(IBackendApiClientProvider remoteBackendApiClientProvider) : IActivityRegistryProvider
{
    /// <inheritdoc />
    public async Task<IEnumerable<ActivityDescriptor>> ListAsync(CancellationToken cancellationToken = default)
    {
        var api = await remoteBackendApiClientProvider.GetApiAsync<IActivityDescriptorsApi>(cancellationToken);
        var request = new ListActivityDescriptorsRequest
        {
            Refresh = true
        };
        var response = await api.ListAsync(request, cancellationToken);

        var categoriasParaRemover = new[]
        {
            "Composition", 
            "Console", 
            "Download File", 
            "Fork (flow)", 
            "HTTP Endpoint",
            "HTTP File Response", 
            "HTTP Request (flow)", 
            "Looping", 
            "MassTransit",
            "Scripting", 
            "SQL", 
            "Storage", 
            "Switch (flow)", 
            "Web"
        };

        var filteredItems = response.Items
            .Where(item => !categoriasParaRemover.Contains(item.Category) && !categoriasParaRemover.Contains(item.DisplayName))
            .ToList();

        return filteredItems;
    }
}