namespace ACherryPie.Incidents;

/// <summary> Public representation of an exceptional situation </summary>
/// <param name="code"> Incident code </param>
public sealed class Incident(IncidentCode code)
{
    /// <summary> Incident code </summary>
    public IncidentCode Code { get; } = code;

    /// <summary> Incident description </summary>
    public string Description
    {
        get
        {
            return Code.GetDescription();
        }
    }
}
