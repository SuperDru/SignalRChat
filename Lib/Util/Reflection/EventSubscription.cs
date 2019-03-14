using System;

namespace Qoden.Util
{
	public class EventSubscription : IDisposable
	{
		public EventSubscription (RuntimeEvent @event, Delegate action, object sender)
		{
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (sender == null) throw new ArgumentNullException(nameof(sender));
			Event = @event;
			Action = action;
			Sender = sender;
			@event.AddEventHandler(sender, action);
		}	

		public RuntimeEvent Event { get; private set; }
		public Delegate Action { get; private set; }
		public object Sender { get; private set; }

		public void Dispose ()
		{
			if (Event != null) {
				Event.RemoveEventHandler (Sender, Action);
				Event = null;
			}
		}
	}
	
}