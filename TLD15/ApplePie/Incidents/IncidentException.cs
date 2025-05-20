using System;

namespace ApplePie.Incidents;

public sealed class IncidentException(IncidentCode code) : Exception
{
    public IncidentCode Code { get; } = code;
}
