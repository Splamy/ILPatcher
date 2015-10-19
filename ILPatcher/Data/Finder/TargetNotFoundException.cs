using System;

namespace ILPatcher.Data.Finder
{
	[Serializable]
	public class TargetNotFoundException : Exception
	{
		public TargetFinder FailedFinder { get; protected set; }

		public TargetNotFoundException(TargetFinder failedFinder) : base(failedFinder.Label)
		{
			if (failedFinder == null)
				throw new ArgumentNullException(nameof(failedFinder));

			FailedFinder = failedFinder;
		}
	}
}
