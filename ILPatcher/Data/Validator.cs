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

		public bool ValidateSet(object validateObject, string errMessage)
		{
			if (validateObject != null) return true;
			SetError(errMessage);
			return false;
		}

		public bool ValidateSet(object validateObject, Func<string> errMessage)
		{
			if (validateObject != null) return true;
			SetError(errMessage());
			return false;
		}

		public bool ValidateStringSet(string validateString, string errMessage)
		{
			if (!string.IsNullOrEmpty(validateString)) return true;
			SetError(errMessage);
			return false;
		}

		public bool ValidateStringSet(string validateString, Func<string> errMessage)
		{
			if (!string.IsNullOrEmpty(validateString)) return true;
			SetError(errMessage());
			return false;
		}

		public bool ValidateTrue(bool result, string errMessage)
		{
			if (result) return true;
			SetError(errMessage);
			return false;
		}

		public bool ValidateTrue(bool result, Func<string> errMessage)
		{
			if (result) return true;
			SetError(errMessage());
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
