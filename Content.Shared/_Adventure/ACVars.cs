using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Adventure.ACVar
{
	// ReSharper disable once InconsistentNaming
	[CVarDefs]
	public sealed class ACVars : CVars
	{
		    public static readonly CVarDef<string> SponsorApiUrl =
        CVarDef.Create("sponsor.api_url", "", CVar.SERVERONLY);

	}
}
