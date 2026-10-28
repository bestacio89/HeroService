using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Executes JSON GET requests for the Unity client while keeping transport
/// behavior consistent across service-specific API clients.
/// Must be called from Unity's main thread because UnityWebRequest is a Unity API.
/// </summary>
public static class UnityHttpClient
{
    private const int DefaultTimeoutSeconds = 15;

    public static async Task<string> GetJsonAsync(
        string baseUrl,
        string relativePath,
        string clientName,
        int timeoutSeconds = DefaultTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("A service base URL is required.", nameof(baseUrl));
        }

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException("A relative request path is required.", nameof(relativePath));
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ArgumentException("A client name is required.", nameof(clientName));
        }

        if (timeoutSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), "Timeout must be greater than zero.");
        }

        if (!Uri.TryCreate(baseUrl.TrimEnd('/') + "/", UriKind.Absolute, out Uri baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps) ||
            !string.IsNullOrEmpty(baseUri.Query) ||
            !string.IsNullOrEmpty(baseUri.Fragment))
        {
            throw new ArgumentException(
                "The service base URL must be an absolute HTTP or HTTPS URL without a query or fragment.",
                nameof(baseUrl));
        }

        string normalizedPath = relativePath.TrimStart('/');
        if (normalizedPath.Contains('\\') ||
            normalizedPath.Contains('#') ||
            Uri.TryCreate(normalizedPath, UriKind.Absolute, out _))
        {
            throw new ArgumentException("The request path must be relative and must not contain a fragment or backslash.", nameof(relativePath));
        }

        string pathWithoutQuery = normalizedPath.Split('?')[0];
        foreach (string segment in pathWithoutQuery.Split('/'))
        {
            string decodedSegment = Uri.UnescapeDataString(segment);
            if (decodedSegment == "." || decodedSegment == ".." || decodedSegment.Contains('\\'))
            {
                throw new ArgumentException("The request path must not contain traversal segments.", nameof(relativePath));
            }
        }

        if (!Uri.TryCreate(baseUri, normalizedPath, out Uri requestUri) ||
            !string.Equals(requestUri.Scheme, baseUri.Scheme, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(requestUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase) ||
            requestUri.Port != baseUri.Port)
        {
            throw new ArgumentException("The request path must resolve within the configured service origin.", nameof(relativePath));
        }

        using var request = UnityWebRequest.Get(requestUri.AbsoluteUri);
        request.timeout = timeoutSeconds;

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new InvalidOperationException(
                $"[{clientName}] GET request failed with HTTP status {request.responseCode}: {request.error}. Path: {relativePath}");
        }

        return request.downloadHandler.text;
    }
}
