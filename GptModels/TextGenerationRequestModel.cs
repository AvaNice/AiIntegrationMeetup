using Newtonsoft.Json;

namespace AiIntegrationMeetup.GptModels;

public class TextGenerationRequestModel
{
	[JsonProperty("model")]
	public string Model { get; set; }

	[JsonProperty("messages")]
	public ICollection<TextGenerationMessageModel> Messages { get; set; }

	[JsonProperty("stream")]
	public bool Stream { get; set; } = false;
}
