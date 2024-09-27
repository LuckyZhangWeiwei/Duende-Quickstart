// See https://aka.ms/new-console-template for more information
//https://docs.duendesoftware.com/identityserver/v7/quickstarts/1_client_credentials/

using IdentityModel.Client;
using System.Text.Json;

var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

if (!disco.IsError)
{
    Console.WriteLine(disco.Error);
    Console.WriteLine(disco.Exception);
}

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    Console.WriteLine(tokenResponse.ErrorDescription);
}

Console.WriteLine(tokenResponse.AccessToken);

// call api
//var apiClient = new HttpClient();
//var disco = await apiClient.GetDiscoveryDocumentAsync("https://localhost:5001");
//var tokenResponse = await apiClient.RequestClientCredentialsTokenAsync(
//    new ClientCredentialsTokenRequest
//    {
//        Address = disco.TokenEndpoint,
//        ClientId = "client",
//        ClientSecret = "secret",
//        Scope = "api1",
//    }
//);
client.SetBearerToken(tokenResponse.AccessToken!); // AccessToken is always non-null when IsError is false

var response = await client.GetAsync("https://localhost:6001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}

var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
Console.WriteLine(
    JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true })
);

Console.ReadLine();
