namespace AiIntegrationMeetup.GptModels;

public class ChatCompletionChunk
{
	public string Id { get; set; }
	public string Object { get; set; }
	public long Created { get; set; }
	public string Model { get; set; }
	public string SystemFingerprint { get; set; }
	public ChunkChoice[] Choices { get; set; }
}