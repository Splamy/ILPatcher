using System;

namespace ILPatcher.Data.Finder
{
	public class TargetNotFoundException : Exception
	{
		public TargetFinder FailedFinder { get; protected set; }

		public TargetNotFoundException(TargetFinder failedFinder) : base(failedFinder.Label)
		{
			FailedFinder = failedFinder;
		}
	}
}
