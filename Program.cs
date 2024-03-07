using AiIntegrationMeetup;
using AiIntegrationMeetup.GptModels;
using Newtonsoft.Json;
using System.Text;

class Program
{
	public const string Token = "";

	public static async Task Main()
	{
		var promt = new TextGenerationRequestModel()
		{
			Model = "gpt-3.5-turbo",
			Messages = new List<TextGenerationMessageModel>() {
				new TextGenerationMessageModel()
				{
					Content = $"Need a placeholder for an article. The answer should be in the format: {Article.Format}",
					Role = "system"
				},
				new TextGenerationMessageModel()
				{
					Content = "Footbal",
					Role = "user"
				},
			}
		};

		GenerateText(promt);

		Console.ReadLine();

		GenerateTextAsStream(promt);

		Console.ReadLine();
	}

	public static async void GenerateTextAsStream(TextGenerationRequestModel promt)
	{
		promt.Stream = true;
		Console.WriteLine("Response AsStream");

		var stream = HttpStreamingRequest(promt);
		var enumerator = stream.GetAsyncEnumerator();
		await enumerator.MoveNextAsync();
	
		int counter = 0;
		do
		{
			counter++;
			Console.WriteLine($"{counter} : {enumerator.Current}");
		} while (await enumerator.MoveNextAsync());
	}

	public static async void GenerateText(TextGenerationRequestModel promt)
	{
		HttpResponseMessage response = await HttpRawRequest(promt);

		var responseContentJson = await response.Content.ReadAsStringAsync();

		Console.WriteLine("Response AsString");
        Console.WriteLine(responseContentJson);

		var result = JsonConvert.DeserializeObject<GptResponseModel>(responseContentJson);
		var article = JsonConvert.DeserializeObject<Article>(result!.Choices.First()!.Message.Content);

		Console.WriteLine("Response Deserialized");
		Console.WriteLine($"Title: {article!.Title}");
		Console.WriteLine($"Description: {article.Description}");
	}

	public static async IAsyncEnumerable<string> HttpStreamingRequest(TextGenerationRequestModel postData)
	{
		var response = await HttpRawRequest(postData);

		string resultAsString = "";

		using (var stream = await response.Content.ReadAsStreamAsync())
		using (StreamReader reader = new StreamReader(stream))
		{
			string line;
			while ((line = await reader.ReadLineAsync()) != null)
			{
				resultAsString += line + Environment.NewLine;

				if (line.StartsWith("data:")) 
					line = line.Substring("data:".Length);

				line = line.TrimStart();

				if (line == "[DONE]")
				{
					yield break;
				}
				else if (!string.IsNullOrWhiteSpace(line))
				{
					var result = JsonConvert.DeserializeObject<ChatCompletionChunk>(line);

					yield return result.Choices.First()!.Delta.Content;
				}
			}
		}
	}

	public static async Task<HttpResponseMessage> HttpRawRequest(TextGenerationRequestModel promt)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
		string jsonContent = JsonConvert.SerializeObject(promt);
		request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		return await client.SendAsync(request);
	}
}