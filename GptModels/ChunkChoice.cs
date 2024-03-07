namespace AiIntegrationMeetup.GptModels;

public class ChunkChoice
{
	public int Index { get; set; }
	public Delta Delta { get; set; }
	public object Logprobs { get; set; }
	public object FinishReason { get; set; }
}