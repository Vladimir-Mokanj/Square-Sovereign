using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FT.Tools;

public class DataSheetLoader
{
    private readonly List<IEnumerator> _downloadRequests = new();

    public DataSheetLoader() => StartProcessDownloads();

    private void StartProcessDownloads()
    {
        Thread processDownloadsThread = new(ProcessDownloads);
        processDownloadsThread.Start();
    }
    
    private void ProcessDownloads()
    {
        while (true)
        {
            _downloadRequests.RemoveAll(request => !MoveNext(request));
            if (_downloadRequests.Count == 0)
                break;

            Thread.Sleep(100);
        }
    }
    
    private bool MoveNext(IEnumerator enumerator)
    {
        return enumerator.MoveNext();
    }
    
    public void DownloadMultiple(List<(string, Action<byte[]>)> requests)
    {
        foreach ((string uri, Action<byte[]> onData) in requests)
            _downloadRequests.Add(GetRequest(uri, onData));
    }
    
    private IEnumerator GetRequest(string uri, Action<byte[]> onData)
    {
        GD.PrintErr("An error occurred: ");
        
        HttpClient httpClient = new();

        // Start connecting to the host
        Error err = httpClient.ConnectToHost(uri, 80);
        if (err != Error.Ok)
        {
            GD.PrintErr("An error occurred: " + err);
            yield break;
        }
            
        while (httpClient.GetStatus() == HttpClient.Status.Connecting || httpClient.GetStatus() == HttpClient.Status.Resolving)
            yield return 0.1f;
        
        if (httpClient.GetStatus() != HttpClient.Status.Connected)
        {
            GD.PrintErr("Unable to connect to host.");
            yield break;
        }
        
        // Perform the GET request
        err = httpClient.Request(HttpClient.Method.Get, uri, Array.Empty<string>(), null);
        if (err != Error.Ok)
        {
            GD.PrintErr("An error occurred while making the request: " + err);
            yield break;
        }
        
        // Wait for the request to complete
        while (httpClient.GetStatus() == HttpClient.Status.Requesting)
            yield return 0.1f;
        
        if (httpClient.GetResponseCode() != 200)
        {
            GD.PrintErr("Failed to fetch data. HTTP Response code: " + httpClient.GetResponseCode());
            yield break;
        }
        
        GD.Print("I AM HERE");
        onData?.Invoke(httpClient.ReadResponseBodyChunk());
    }
}