using thebrokenscript.Core;
namespace thebrokenscript.Content.Events;
public interface IModEvent
{
	bool IsEnabled(ServerConfig config);
	void StartEvent();
}