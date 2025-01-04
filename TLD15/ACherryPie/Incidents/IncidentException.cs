using System;

namespace ACherryPie.Incidents;

/// <summary> Exceptional situation <see cref="Exception"/> described with a <see cref="IncidentCode"/> </summary>
/// <remarks> Create an exceptional situation <see cref="Exception"/> </remarks>
/// <param name="code"> Code of an exceptional situation </param>
public sealed class IncidentException(IncidentCode code) : Exception(string.Empty)
{
    /// <summary> Code of a exceptional situation </summary>
    public IncidentCode Code { get; } = code;
}
