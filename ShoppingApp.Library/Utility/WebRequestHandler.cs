using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApp.Library.Utilities
{
    public class WebRequestHandler
    {
        private string host = "localhost";
        private string port = "5057";
        private HttpClient Client { get; }

        public WebRequestHandler()
        {
            Client = new HttpClient();
        }

        public async Task<string?> Get(string url)
        {
            var fullUrl = $"http://{host}:{port}{url}";
            try
            {
                var response = await Client.GetStringAsync(fullUrl).ConfigureAwait(false);
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Received an empty response from the server.");
                }
                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HttpRequestException in Get method: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Get method: {e.Message}");
            }
            return null;
        }

        public async Task<string?> Delete(string url)
        {
            var fullUrl = $"http://{host}:{port}{url}";
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl))
                {
                    using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                        Console.WriteLine($"Delete request failed with status code: {response.StatusCode}");
                        return "ERROR";
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HttpRequestException in Delete method: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Delete method: {e.Message}");
            }
            return null;
        }

        public async Task<string?> Post(string url, object obj)
        {
            var fullUrl = $"http://{host}:{port}{url}";
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, fullUrl))
                    {
                        request.Content = stringContent;
                        using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                return await response.Content.ReadAsStringAsync();
                            }
                            Console.WriteLine($"Post request failed with status code: {response.StatusCode}");
                            return "ERROR";
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HttpRequestException in Post method: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Post method: {e.Message}");
            }
            return null;
        }

        public async Task UploadFileAsync(string filePath)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await client.PostAsync($"http://localhost:7072/Inventory/Import", form);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File uploaded successfully.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
        }
    }
}
