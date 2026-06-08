using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;
public interface IModEvent
{
	bool IsEnabled(ServerConfig config);
	void StartEvent();
}