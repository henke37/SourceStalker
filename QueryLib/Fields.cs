using System;

namespace Henke37.Valve.Source.ServerQuery {
	[Flags]
	public enum Fields {
		None = 0,
		Info = 1,
		Rules = 2,
		Players = 4,
		All = Info | Rules | Players
	}
}
