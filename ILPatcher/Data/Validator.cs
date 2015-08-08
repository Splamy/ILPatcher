using System;
using ILPatcher.Utility;

namespace ILPatcher.Data
{
	class Validator
	{
		public bool Ok { get; protected set; }
		public Action OnErrorEvent { get; set; }

		public Validator()
		{
			Ok = true;
		}

		public bool ValidateSet(object validateObject, Func<string> errEessage)
		{
			if (validateObject != null) return true;
			SetError(errEessage());
			return false;
		}

		public bool ValidateTrue(bool result, Func<string> errEessage)
		{
			if (result) return true;
			SetError(errEessage());
			return false;
		}

		public Validator(Action OnErrorEvent) : this()
		{
			this.OnErrorEvent = OnErrorEvent;
		}

		protected void SetError(string message)
		{
			Ok = false;
			Log.Write(Log.Level.Error, message);
			OnErrorEvent?.Invoke();
		}
	}
}
